using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Pursuer;

public class Pursuer : MonoBehaviour
{
    [Header("Navmesh Agent")]
    public NavMeshAgent agent;
    public LayerMask whatIsPlayer;
    public Transform player;
    public List<Transform> destinations;
    private Transform currentDest;
    private int randNum;
    public Animator animator;

    [Header("Chase & Walk")]
    public float walkSpeed = 0.5f, chaseSpeed = 2f;
    public float minIdleTime = 2f, maxIdleTime = 5f;
    public float minChaseTime = 3f, maxChaseTime = 5f; 
    public float chaseRange = 10f;
    private bool walking = true;
    private bool chasing = false;
    public bool canChase = true;
    private bool isChaseRoutineRunning = false;
    public bool isPlayerHiding = false;

    [Header("Comedy Settings")]
    public bool canTrip = false;
    public float tripChance = 0.5f;
    public float tripCheckInterval = 2f;
    private float tripTimer = 0f;
    private int lastTripIndex = -1;
    public string[] tripTriggers = { "trip1", "trip2", "trip3", "trip4" };
    public bool isInteracting = false;

    public static Pursuer ActivePursuer;
    public enum PursuerType
    {
        Default, //For Default & Ignore Pursuer
        Baby,
        Dog,
        Goofy,
        Fairy,
        Lost,
        SlowMo,
        Tiny
    }
    [SerializeField] private PursuerType pursuerType;

    ChasingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<ChasingAudioManager>();
    }

    void OnEnable()
    {
        ActivePursuer = this;
        PlayMusicForType(pursuerType);
    }

    void OnDisable()
    {
        if (ActivePursuer == this)
            ActivePursuer = null;
    }

    private void PlayMusicForType(PursuerType type)
    {
        switch (type)
        {
            case PursuerType.Baby:
                audioManager.playLongSFX2(audioManager.baby);
                break;
            case PursuerType.Dog:
                audioManager.playLongSFX2(audioManager.dog);
                break;
            case PursuerType.Goofy:
                audioManager.playLongSFX2(audioManager.goofy);
                break;
            case PursuerType.Fairy:
                audioManager.playLongSFX2(audioManager.fairy);
                break;
            case PursuerType.Default:
            case PursuerType.Lost:
            case PursuerType.SlowMo:
                // No audio for these types
                break;
            default:
                break;
        }
    }

    void Start()
    {
        ChooseNewDestination();

        Vector3 pos = transform.position; 
        pos.y = 0.093f;                   
        transform.position = pos;
    }

    void Update()
    {
        bool playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, whatIsPlayer);

        // Set Chasing & Walking
        if (isPlayerHiding)
        {
            chasing = false;
            walking = true; 
        }

        if (playerInChaseRange && canChase && !isPlayerHiding)
        {
            if (!isChaseRoutineRunning)
            {
                chasing = true;
                walking = false;
                StopAllCoroutines();
                StartCoroutine(ChaseRoutine());
            }
        }

        // Chasing & Walking Logic
        if (chasing)
        {
            ChasePlayer();
            
            if (canTrip)
            {
                tripTimer += Time.deltaTime;
                if (tripTimer >= tripCheckInterval) // Time to attempt a trip
                {
                    tripTimer = 0f;
                    TryTrip();
                }
            }
        }
        else if (walking)
        {
            Patrol();
        }
    }

    public void Patrol()
    {
        agent.destination = currentDest.position;
        agent.speed = walkSpeed;

        ResetAllTriggers();
        animator.SetTrigger("walk");
        PlayPatrolSFX();

        if (agent.remainingDistance <= agent.stoppingDistance) //Destination reached
        {
            ResetAllTriggers();
            animator.SetTrigger("idle");
            audioManager.stopLongSFX();

            agent.speed = 0;
            StartCoroutine(IdleRoutine());
            walking = false;
        }
    }

    void ChasePlayer()
    {
        float stoppingBuffer = 3f; //How close the pursuer gets
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stoppingBuffer)
        {
            agent.destination = player.position;
            agent.speed = chaseSpeed;

            ResetAllTriggers();
            animator.SetTrigger("sprint");
            PlayChaseSFX();
        }
        else
        {
            ResetAllTriggers();
            animator.SetTrigger("idle");
            audioManager.stopLongSFX();

            agent.speed = 0;
        }
    }
    IEnumerator IdleRoutine() //How long pursuer stays idle
    {
        float idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);
        ChooseNewDestination();
        walking = true;
    }

    IEnumerator ChaseRoutine() //How long pursuer chases player until he gives up and resumes patrolling
    {
        isChaseRoutineRunning = true;

        float chaseDuration = Random.Range(minChaseTime, maxChaseTime);
        yield return new WaitForSeconds(chaseDuration);

        chasing = false;
        walking = true;
        ChooseNewDestination();

        isChaseRoutineRunning = false;
    }


    void ChooseNewDestination() //Choose random destination for patrolling
    {
        if (destinations.Count == 0) return;

        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    public void ResetAllTriggers()
    {
        animator.ResetTrigger("sprint");
        animator.ResetTrigger("walk");
        animator.ResetTrigger("idle");

        if (canTrip)
        {
            foreach (var trigger in tripTriggers)
            {
                animator.ResetTrigger(trigger);
            }
        } 
    }

    void TryTrip()
    {
        if (Random.value < tripChance)
        {
            Trip();
        }
    }

    void Trip()
    {
        agent.speed = 0f;
        ResetAllTriggers();
        int nextIndex = GetNextTripIndex();
        animator.SetTrigger(tripTriggers[nextIndex]);
    }

    private int GetNextTripIndex()
    {
        int nextIndex = Random.Range(0, tripTriggers.Length);
    
        while (nextIndex == lastTripIndex)
        {
            nextIndex = Random.Range(0, tripTriggers.Length);
        }

        lastTripIndex = nextIndex;
        return nextIndex;
    }

    private void PlayPatrolSFX()
    {
        switch (pursuerType)
        {
            case PursuerType.Default:
                audioManager.playLongSFX(audioManager.walkingScary);
                break;
            case PursuerType.SlowMo:
                audioManager.playLongSFX(audioManager.walkingSlow);
                break;
            case PursuerType.Tiny:
                audioManager.playLongSFX(audioManager.walkingFast);
                break;
            case PursuerType.Lost:
                audioManager.playLongSFX(audioManager.walking);
                break;
        }
    }

    private void PlayChaseSFX()
    {
        switch (pursuerType)
        {
            case PursuerType.Default:
                audioManager.playLongSFX(audioManager.chasing);
                break;
            case PursuerType.Lost:
                audioManager.playLongSFX(audioManager.walking); // intentional
                break;
            case PursuerType.SlowMo:
                audioManager.playLongSFX(audioManager.chasingSlow);
                break;
            case PursuerType.Tiny:
                audioManager.playLongSFX(audioManager.chasingFast);
                break;
        }
    }


}

