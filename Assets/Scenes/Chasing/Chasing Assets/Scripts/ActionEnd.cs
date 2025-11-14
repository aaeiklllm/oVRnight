using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEnd : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
   
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ActivePursuer is either Pursuer or PursuerFlee
        if (Pursuer.ActivePursuer != null)
        {
            Vector3 newPosition = Pursuer.ActivePursuer.transform.position;
            newPosition.y = 0.093f;
            Pursuer.ActivePursuer.transform.position = newPosition;
        }
        else if (PursuerFlee.ActivePursuer != null)
        {
            Vector3 newPosition = PursuerFlee.ActivePursuer.transform.position;
            newPosition.y = 0.093f;
            PursuerFlee.ActivePursuer.transform.position = newPosition;
        }
    }
}
