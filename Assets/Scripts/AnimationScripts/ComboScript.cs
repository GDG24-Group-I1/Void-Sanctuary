using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComboScript : StateMachineBehaviour
{
    Player player;
    private bool attackClicked = false;
    private float comboWindow = 0f;

    [SerializeField] private float exitTransitionDuration;
    [SerializeField] private float exitTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackClicked = false;
        var animatorScript = animator.GetComponent<PlayerAnimator>();
        player = animatorScript.GetPlayer();
        player.OnPlayerAttack = () =>
        {
            attackClicked = true;
        };
        // var duration
        var actualDuration = stateInfo.length - exitTransitionDuration - exitTime;
        comboWindow = (actualDuration * 0.75f) / stateInfo.length;
        Debug.Log($"Combo window is {comboWindow}");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(layerIndex)) return;
        if (player.CanCombo == ComboState.PressedEarly || player.CanCombo == ComboState.Pressed) return;
        if (stateInfo.normalizedTime >= comboWindow && player.CanCombo == ComboState.NotPressed) player.CanCombo = ComboState.CanCombo;
        if (attackClicked && player.CanCombo == ComboState.CanCombo)
        {
            player.AttackNumber++;
            player.CanCombo = ComboState.Pressed;
            attackClicked = false;
            Debug.Log($"Combo from {player.AttackNumber - 1} to {player.AttackNumber}");
        } else if (attackClicked)
        {
            player.CanCombo = ComboState.PressedEarly;
            attackClicked = false;
            Debug.Log("Aww too early!");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.AttackAnimationEnded();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
