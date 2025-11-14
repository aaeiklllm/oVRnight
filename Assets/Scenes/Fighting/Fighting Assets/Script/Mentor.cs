
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections.Generic;

public class Mentor : MonoBehaviour
{
    [Header("Navmesh Agent")]
    public NavMeshAgent agent;
    public LayerMask whatIsGround, whatIsPlayer;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    
    [Header("Animation")]
    public Animator animator;
    public string[] attackTriggers;
    public int randomIndex;

    [Header("Attack & Block")]
    public bool isAttacking;
    public bool isBlocking = false;
    private int lastAttackIndex = -1;
    public float rotationSpeed = 20f;
    public float lastAttacktime = 0f;
    public float timeBetweenAttacks = 3f;
    public float lastBlocktime = 0f;
    public float maxBlockduration = 1f;

    [Header("Flash")]
    public Renderer[] characterRenderers; // Array of Renderer components for the character
    public Material flashMaterial; // Reference to the red flash material
    public float flashDuration = 0.2f; // Duration of the flash effect in seconds
    private Material[] originalMaterials; // Array to store original materials of the character
    private float flashTimer; // Timer to track the duration of the flash effect
    private bool isFlashing; // Flag to indicate if the character is currently flashing

    [Header("Weapons & Effects")]
    public GameObject aoeFX;
    public GameObject aoeHitbox;

    [Header("Extras")]
    public Transform player; 
    public static Mentor ActiveMentor;

    FightingAudioManager audioManager;

    private void Start()
    {
        // Initialize the originalMaterials array with the same length as the characterRenderers array
        originalMaterials = new Material[characterRenderers.Length];

        // Store the original materials of each renderer
        for (int i = 0; i < characterRenderers.Length; i++)
        {
            originalMaterials[i] = characterRenderers[i].material;
        }
    }

    private void Awake()
    {
        //Attack Animations
        attackTriggers = new string[] { "Attacking1", "Attacking2", "Attacking3", "Attacking4", "Attacking5" };

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FightingAudioManager>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); //vector3 center, radius of sightrange, layermask 
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); //performs collision check

        if (isBlocking && Time.time >= lastBlocktime + maxBlockduration) 
        {
            animator.SetBool("isBlocking", false);
        }
        
        //Chase and Attack Player
        if (!playerInSightRange && !playerInAttackRange)
        {
            Idle();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }
        
        // Check if the character is currently flashing
        if (isFlashing)
        {
            // Update the flash timer
            flashTimer += Time.deltaTime;

            // If the flash duration has elapsed, stop flashing and restore original materials
            if (flashTimer >= flashDuration)
            {
                StopFlash();
            }
        }

        //Debug.Log("Mentor is attacking:" + isAttacking);
    }

    void OnEnable()
    {
        ActiveMentor = this;
    }

    void OnDisable()
    {
        if (ActiveMentor == this)
            ActiveMentor = null;
    }

    private void Idle()
    {
       animator.SetBool("isWalking", false);
    }

    private void ChasePlayer()
    {
        animator.applyRootMotion = false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0f;

        if (directionToPlayer.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        bool shouldMove = distanceToPlayer > agent.stoppingDistance;

        if (shouldMove)
        {
            agent.SetDestination(player.position);
            bool isWalking = agent.velocity.magnitude > 0.1f;
            animator.SetBool("isWalking", isWalking);

            if (isWalking)
            {
                PlayFootstep();
            }
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("isWalking", false);
            audioManager.stopLongSFX();
        }
    }

    private void AttackPlayer()
    {
        animator.SetBool("isWalking", false);
        audioManager.stopLongSFX();
        if (playerInAttackRange && !isBlocking)
        {
            Vector3 targetDirection = player.position - transform.position;
            targetDirection.y = 0f;  
            targetDirection.Normalize();  
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


            if (!isAttacking
            && Vector3.Dot(transform.forward, targetDirection) > 0
            && Time.time >= lastAttacktime + timeBetweenAttacks)
            {
                animator.SetBool("isBlocking", false);
                int nextIndex = GetNextAttackIndex();
                randomIndex = nextIndex;
                //Debug.Log(randomIndex);

                animator.SetTrigger(attackTriggers[randomIndex]);
                lastAttacktime = Time.time;
            }
        }
    }

    private int GetNextAttackIndex()//Ensures attacks don't repeat for a consecutive time
    {
        int nextIndex = Random.Range(0, attackTriggers.Length);
    
        while (nextIndex == lastAttackIndex)
        {
            nextIndex = Random.Range(0, attackTriggers.Length);
        }

        lastAttackIndex = nextIndex;
        return nextIndex;
    }

    public void TakeDamage()
    {
        if (!isBlocking)
        {
            StartFlash();

            if (!isAttacking)
            {
                // Keep blocking
                animator.SetBool("isBlocking", true);
                lastBlocktime = Time.time;
            }
        }
        else 
        {
            Debug.Log("Blocked");
        }
    }

    // Method to start the red flash effect
    private void StartFlash()
    {
        // Set the flash material to all renderers
        for (int i = 0; i < characterRenderers.Length; i++)
        {
            characterRenderers[i].material = flashMaterial;
        }

        // Initialize the flash timer and set the flashing flag
        flashTimer = 0f;
        isFlashing = true;
    }

    // Method to stop the red flash effect and restore original materials
    private void StopFlash()
    {
        // Restore the original materials
        for (int i = 0; i < characterRenderers.Length; i++)
        {
            characterRenderers[i].material = originalMaterials[i];
        }

        // Reset the flashing flag
        isFlashing = false;
    }

    public void spawnShockwave() 
    {
        GameObject fxClone = Instantiate(aoeFX, transform.position, Quaternion.identity);
        GameObject aoeClone = Instantiate(aoeHitbox, transform.position, Quaternion.identity);

        Destroy(fxClone, 2f);
        Destroy(aoeClone, 0.25f);
    }

    void PlayFootstep()
    {
        if (animator.speed == 0.4f)
        {
            audioManager.playLongSFX(audioManager.walkSlow);
        }
        else if (animator.speed == 2.2f)
        {
            audioManager.playLongSFX(audioManager.walkFast);
        }
        else
        {
            audioManager.playLongSFX(audioManager.walk);
        }
    }


    private void OnDrawGizmosSelected() //draws gizmos for visualization
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
  
}
