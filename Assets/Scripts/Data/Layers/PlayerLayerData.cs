using System;
using UnityEngine;

namespace ThirdPersonMovementSystem
{
  [Serializable]
  public class PlayerLayerData
  {
    [field: SerializeField] public LayerMask GroundLayer { get; private set; }
  }
}
