using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to set Mentor isBlocking depending on the animation
public class HandleBlock : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Mentor.ActiveMentor.isBlocking = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Mentor.ActiveMentor.isBlocking = false;
    }
}
