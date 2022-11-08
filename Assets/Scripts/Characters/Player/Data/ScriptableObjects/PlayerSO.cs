using UnityEngine;

namespace ThirdPersonMovementSystem
{
  [CreateAssetMenu(fileName = "Player", menuName = "Custom/Characters/Players")]
  public class PlayerSO : ScriptableObject
  {
    [field: SerializeField] public PlayerGroundedData GroundedData { get; private set; }
  }

}