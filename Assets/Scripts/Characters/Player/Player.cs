using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonMovementSystem
{
  [RequireComponent(typeof(PlayerInput))]
  public class Player : MonoBehaviour
  {
    [field: Header("References")]
    [field: SerializeField] public PlayerSO Data { get; private set; }

    [field: Header("Collisions")]

    [field: SerializeField] public CapsuleColliderUtility ColliderUtility { get; private set; }
    [field: SerializeField] public PlayerLayerData LayerData { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    public Transform MainCameraTransform { get; private set; }

    public PlayerInput Input { get; private set; }

    private PlayerMovementStateMachine movementStateMachine;

    private void Awake()
    {
      Rigidbody = GetComponent<Rigidbody>();

      Input = GetComponent<PlayerInput>();

      MainCameraTransform = Camera.main.transform;

      movementStateMachine = new PlayerMovementStateMachine(this);
    }

    private void OnValidate()
    {

      ColliderUtility.Initialize(gameObject);
      ColliderUtility.CalculateCapsuleColliderDimensions();
    }

    // Start is called before the first frame update
    void Start()
    {
      movementStateMachine.ChangeState(movementStateMachine.IdlingState);
    }

    // Update is called once per frame
    void Update()
    {
      movementStateMachine.HandleInput();

      movementStateMachine.Update();
    }

    private void FixedUpdate()
    {
      movementStateMachine.PhysicsUpdate();
    }
  }
}
