using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAOE : StateMachineBehaviour
{
    private float startTime;
    public float timeToSpawn = 0.9f;
    private bool spawned = false;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        startTime = Time.time;
        spawned = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Mentor.ActiveMentor.isAttacking && Time.time > startTime + timeToSpawn && !spawned) 
        {
            spawned = true;
            Mentor.ActiveMentor.spawnShockwave();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
