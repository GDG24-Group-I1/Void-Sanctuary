using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComboScript : StateMachineBehaviour
{
    Player player;
    private bool attackClicked = false;
    private float comboWindow = 0f;

    /// <summary>
    /// Adjustment to the combo window in seconds, this is to account for the time it takes for the animation to transition
    /// </summary>
    [SerializeField] private float windowAdjustment;

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
        var actualDuration = stateInfo.length - windowAdjustment;
        comboWindow = (actualDuration * 0.75f) / stateInfo.length;
        DebugExt.LogCombo($"Window is {comboWindow * stateInfo.length} seconds");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(layerIndex)) { player.StopSwordGlowing(); return; };
        if (player.CanCombo == ComboState.PressedEarly || player.CanCombo == ComboState.Pressed) return;
        if (stateInfo.normalizedTime >= comboWindow && player.CanCombo == ComboState.NotPressed) {
            player.SetCanCombo();
        }
        if (attackClicked && player.CanCombo == ComboState.CanCombo)
        {
            player.AttackNumber++;
            player.CanCombo = ComboState.Pressed;
            attackClicked = false;
            DebugExt.LogCombo($"Combo from {player.AttackNumber - 1} to {player.AttackNumber}: hit", always: true);
        } else if (attackClicked)
        {
            player.CanCombo = ComboState.PressedEarly;
            attackClicked = false;
            DebugExt.LogCombo($"Combo from {player.AttackNumber} to {player.AttackNumber + 1}: too early!", always: true);
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
