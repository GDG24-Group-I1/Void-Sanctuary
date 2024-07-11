using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : StateMachineBehaviour
{
    EnemyAI enemy;
    [SerializeField] private float swordSolidThreshold;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponentInParent<EnemyAI>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= swordSolidThreshold)
        {
            enemy.SetSwordSolidity(true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.SetSwordSolidity(false);
    }
}
