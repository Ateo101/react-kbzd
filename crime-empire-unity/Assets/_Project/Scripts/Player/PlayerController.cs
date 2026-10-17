using UnityEngine;
#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace CrimeEmpire.Player
{
    /// <summary>Top-down movement relative to the camera. Joystick on device, WASD in the editor.</summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        public const string Tag = "Player";

        static readonly int SpeedHash = Animator.StringToHash("Speed");

        [SerializeField] VirtualJoystick joystick;
        [SerializeField] Transform cameraTransform;
        [SerializeField] Animator animator;
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float turnSpeed = 720f;
        [SerializeField] float gravity = -20f;

        CharacterController _controller;
        float _verticalVelocity;

        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = Mathf.Max(0f, value);
        }

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        }

        void Update()
        {
            var move = ToWorld(ReadInput());

            if (_controller.isGrounded && _verticalVelocity < 0f) _verticalVelocity = -1f;
            _verticalVelocity += gravity * Time.deltaTime;

            var velocity = move * moveSpeed;
            velocity.y = _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);

            if (move.sqrMagnitude > 0.0001f)
            {
                var target = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
            }

            if (animator != null) animator.SetFloat(SpeedHash, move.magnitude);
        }

        Vector2 ReadInput()
        {
            var input = joystick != null ? joystick.Value : Vector2.zero;
#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (input == Vector2.zero && keyboard != null)
            {
                input.x = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
                input.y = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
            }
#endif
            return Vector2.ClampMagnitude(input, 1f);
        }

        Vector3 ToWorld(Vector2 input)
        {
            if (cameraTransform == null) return new Vector3(input.x, 0f, input.y);

            var forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            var right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            return forward * input.y + right * input.x;
        }
    }
}
