using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonMovementSystem
{
  public class PlayerMovementState : IState
  {

    protected PlayerMovementStateMachine stateMachine;

    protected PlayerGroundedData movementData;


    public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
    {
      stateMachine = playerMovementStateMachine;

      movementData = stateMachine.Player.Data.GroundedData;

      InizializeData();

    }

    private void InizializeData()
    {
      stateMachine.ReusableData.TimeToReachTargetRotation = movementData.BaseRotationData.TargetRotationReachTime;
    }

    #region IState Methods

    public virtual void Enter()
    {
      Debug.Log("State: " + GetType().Name);

      AddInputActionsCallbacks();
    }

    public virtual void Exit()
    {
      RemoveInputActionsCallbacks();
    }

    public virtual void HandleInput()
    {
      ReadMovementInput();

    }

    public virtual void Update()
    {

    }

    public virtual void PhysicsUpdate()
    {
      Move();
    }

    public virtual void OnAnimationEnterEvent()
    {

    }

    public virtual void OnAnimationExitEvent()
    {

    }

    public virtual void OnAnimationTransitionEvent()
    {

    }


    #endregion

    #region Main Methods

    private void ReadMovementInput()
    {
      stateMachine.ReusableData.MovementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
    }

    private void Move()
    {
      if (stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0f)
      {
        return;
      }

      Vector3 movementDirection = GetMovementInputDirection();

      float targetRotationYAngle = Rotate(movementDirection);

      Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

      float movementSpeed = GetMovementSpeed();


      Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();

      stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
    }

    private float Rotate(Vector3 direction)
    {
      float directionAngle = UpdateTargetRotation(direction);

      RotateTowardsTargetRotation();

      return directionAngle;

    }

    #endregion

    #region Reusable Methods

    protected Vector3 GetPlayerHorizontalVelocity()
    {
      Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;
      playerHorizontalVelocity.y = 0f;
      return playerHorizontalVelocity;
    }

    protected Vector3 GetPlayerVerticalVelocity()
    {
      return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y, 0f);
    }

    protected Vector3 GetMovementInputDirection()
    {
      return new Vector3(stateMachine.ReusableData.MovementInput.x, 0f, stateMachine.ReusableData.MovementInput.y);
    }
    protected float GetMovementSpeed()
    {
      return movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier * stateMachine.ReusableData.MovementOnSlopesSpeedModifier;
    }

    protected void RotateTowardsTargetRotation()
    {
      float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;

      if (currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y)
      {
        return;
      }

      float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, stateMachine.ReusableData.CurrentTargetRotation.y, ref stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, stateMachine.ReusableData.TimeToReachTargetRotation.y - stateMachine.ReusableData.DampedTargetRotationPassedTime.y);

      stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

      Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

      stateMachine.Player.Rigidbody.MoveRotation(targetRotation);

    }


    private void UpdateTargetRotationData(float targetAngle)
    {
      stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;
      stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0f;
    }

    private float GetDirectionAngle(Vector3 direction)
    {
      float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
      if (directionAngle < 0f)
      {
        directionAngle += 360f;
      }
      return directionAngle;
    }

    private float AddCameraRotationToAngle(float directionAngle)
    {
      directionAngle += stateMachine.Player.MainCameraTransform.eulerAngles.y;

      if (directionAngle > 360f)
      {
        directionAngle -= 360f;
      }

      return directionAngle;
    }


    protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
    {
      float directionAngle = GetDirectionAngle(direction);

      if (shouldConsiderCameraRotation)
      {
        directionAngle = AddCameraRotationToAngle(directionAngle);
      }


      if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
      {
        UpdateTargetRotationData(directionAngle);
      }
      return directionAngle;
    }


    protected Vector3 GetTargetRotationDirection(float targetAngle)
    {
      return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    protected void ResetVelocity()
    {
      stateMachine.Player.Rigidbody.velocity = Vector3.zero;
    }


    protected virtual void AddInputActionsCallbacks()
    {
      stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
    }

    protected virtual void RemoveInputActionsCallbacks()
    {
      stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
    }

    #endregion

    #region Input Methods

    protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
      stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;
    }

    #endregion

  }
}
