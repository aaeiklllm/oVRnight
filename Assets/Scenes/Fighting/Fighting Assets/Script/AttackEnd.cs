using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to set Mentor isAttacking depending on the animation
public class AttackEnd : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Mentor.ActiveMentor.isAttacking = true;
        Mentor.ActiveMentor.animator.applyRootMotion = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Mentor.ActiveMentor.isAttacking = false;
        Mentor.ActiveMentor.lastAttacktime = Time.time;
        Mentor.ActiveMentor.animator.applyRootMotion = false;

        //Set position every after attack
        Vector3 newPosition = Mentor.ActiveMentor.transform.position;
        newPosition.y = 3.32f; 
        Mentor.ActiveMentor.transform.position = newPosition;
    }
}
