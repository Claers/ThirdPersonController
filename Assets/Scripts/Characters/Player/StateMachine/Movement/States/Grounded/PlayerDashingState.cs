using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonMovementSystem
{
  public class PlayerDashingState : PlayerGroundedState
  {

    private PlayerDashData dashData;

    private float startTime;

    private int consecutiveDashUsed;

    public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
      dashData = movementData.DashData;
    }
    #region IState Methods

    public override void Enter()
    {
      base.Enter();

      stateMachine.ReusableData.MovementSpeedModifier = dashData.SpeedModifier;

      AddForceOnTrasitionFromStationaryState();

      UpdateConsecutiveDashes();

      startTime = Time.time;
    }

    public override void OnAnimationTransitionEvent()
    {
      base.OnAnimationTransitionEvent();
      if (stateMachine.ReusableData.MovementInput == Vector2.zero)
      {
        stateMachine.ChangeState(stateMachine.IdlingState);
        return;
      }
      stateMachine.ChangeState(stateMachine.SprintingState);

    }
    #endregion

    #region Main Methods
    private void AddForceOnTrasitionFromStationaryState()
    {
      if (stateMachine.ReusableData.MovementInput != Vector2.zero)
      {
        return;
      }

      Vector3 characterRotationDirection = stateMachine.Player.transform.forward;

      characterRotationDirection.y = 0f;

      stateMachine.Player.Rigidbody.velocity = characterRotationDirection * GetMovementSpeed();
    }

    private void UpdateConsecutiveDashes()
    {
      if (!IsConsecutive())
      {
        consecutiveDashUsed = 0;
      }

      ++consecutiveDashUsed;

      if (consecutiveDashUsed == dashData.ConsecutiveDashLimit)
      {
        consecutiveDashUsed = 0;

        stateMachine.Player.Input.DisableActionFor(stateMachine.Player.Input.PlayerActions.Dash, dashData.DashCooldown);

      }

    }

    private bool IsConsecutive()
    {
      return Time.time < startTime + dashData.TimeToBeConsideredConsecutive;
    }
    #endregion

    #region Input Methods

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
    }

    protected override void OnDashStarted(InputAction.CallbackContext context)
    {

    }

    #endregion

  }
}
