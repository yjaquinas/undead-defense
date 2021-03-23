using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CScopeIdleEvent : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<CScope>().ScopeToggle(false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<CScope>().ScopeToggle(true);
    }
}
