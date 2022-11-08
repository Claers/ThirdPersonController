using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonMovementSystem
{
  public class PlayerMovingState : PlayerGroundedState
  {
    public PlayerMovingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {

    }

  }
}
