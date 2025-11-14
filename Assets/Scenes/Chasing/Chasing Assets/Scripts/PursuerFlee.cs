using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PursuerFlee : MonoBehaviour
{
    [Header("Navmesh Agent")]
    public NavMeshAgent agent;
    public LayerMask whatIsPlayer;
    public Transform player;
    public List<Transform> destinations;
    private Transform currentDest;
    private int randNum;
    public Animator animator;

    [Header("Patrol & Flee")]
    public float walkSpeed = 0.5f, fleeSpeed = 2f;
    public float minIdleTime = 2f, maxIdleTime = 5f;
    public float minFleeTime = 3f, maxFleeTime = 6f;
    public float fleeTriggerRange = 7f;
    public float fleeDistance = 10f;
    private bool isFleeing = false;
    private float fleeCooldown = 3f; 
    private float fleeTimer = 0f;

    private bool walking = true;
    private bool fleeing = false;

    public static PursuerFlee ActivePursuer;

    ChasingAudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<ChasingAudioManager>();
    }

    void OnEnable()
    {
        ActivePursuer = this;
    }

    void OnDisable()
    {
        if (ActivePursuer == this)
            ActivePursuer = null;
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
        bool playerNearby = Physics.CheckSphere(transform.position, fleeTriggerRange, whatIsPlayer);

        if (playerNearby)
        {
            fleeing = true;
            walking = false;
            StopAllCoroutines();
            StartCoroutine(FleeRoutine());
        }

        if (fleeing)
        {
            if (isFleeing)
            {
                fleeTimer -= Time.deltaTime;
                if (fleeTimer <= 0f)
                {
                    isFleeing = false;
                }
            }

            FleeFromPlayer();
        }
        else if (walking)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        agent.destination = currentDest.position;
        agent.speed = walkSpeed;

        ResetAllTriggers();
        animator.SetTrigger("walk");
        audioManager.playLongSFX(audioManager.walkingScary);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            ResetAllTriggers();
            animator.SetTrigger("idle");
            audioManager.stopLongSFX();
            agent.speed = 0;
            StartCoroutine(IdleRoutine());
            walking = false;
        }
    }

    void FleeFromPlayer()
    {
        if (isFleeing) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < fleeTriggerRange)
        {
            Vector3 dirToPlayer = transform.position - player.position;
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            Vector3 fleeDirection = (dirToPlayer + randomOffset).normalized;
            Vector3 fleePos = transform.position + fleeDirection * fleeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleePos, out hit, 20f, NavMesh.AllAreas))
            {
                float fleeDistanceFromAgent = Vector3.Distance(transform.position, hit.position);

                if (fleeDistanceFromAgent > 1f)
                {
                    NavMeshPath path = new NavMeshPath();
                    if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path) &&
                        path.status == NavMeshPathStatus.PathComplete)
                    {
                        Debug.Log("Flee position valid and reachable. Moving to: " + hit.position);

                        agent.SetPath(path);
                        agent.speed = fleeSpeed;
                        isFleeing = true;
                        fleeTimer = fleeCooldown;

                        ResetAllTriggers();
                        animator.SetTrigger("sprint");
                        audioManager.playLongSFX(audioManager.chasing);
                        return;
                    }
                }
            }

            // Fallback
            Debug.LogWarning("Flee position NOT valid on NavMesh. Trying fallback destinations...");

            Transform closest = GetClosestReachableDestination();
            if (closest != null)
            {
                Debug.Log("Fallback destination found. Moving to closest reachable destination: " + closest.position);
                agent.destination = closest.position;
                agent.speed = fleeSpeed;
                isFleeing = true;
                fleeTimer = fleeCooldown;

                ResetAllTriggers();
                animator.SetTrigger("sprint");
                audioManager.playLongSFX(audioManager.chasing);
            }
        }
    }

    IEnumerator IdleRoutine()
    {
        float idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);
        ChooseNewDestination();
        walking = true;
    }

    IEnumerator FleeRoutine()
    {
        float fleeDuration = Random.Range(minFleeTime, maxFleeTime);
        yield return new WaitForSeconds(fleeDuration);
        ChooseNewDestination();
        fleeing = false;
        walking = true;
    }

    void ChooseNewDestination()
    {
        if (destinations.Count == 0) return;

        randNum = Random.Range(0, destinations.Count);
        currentDest = destinations[randNum];
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeTriggerRange);
    }

    void ResetAllTriggers()
    {
        animator.ResetTrigger("sprint");
        animator.ResetTrigger("walk");
        animator.ResetTrigger("idle");
    }

    Transform GetClosestReachableDestination()
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 agentPos = transform.position;

        NavMeshPath path = new NavMeshPath();

        foreach (Transform destination in destinations)
        {
            float distance = Vector3.Distance(agentPos, destination.position);

            if (NavMesh.CalculatePath(agentPos, destination.position, NavMesh.AllAreas, path) &&
                path.status == NavMeshPathStatus.PathComplete &&
                distance < closestDistance)
            {
                closestDistance = distance;
                closest = destination;
            }
        }

        return closest;
    }
}
