using UnityEngine;

namespace CrimeEmpire.Player
{
    /// <summary>Fixed-angle follow camera like in the references. Swap for Cinemachine if you need zones and shakes.</summary>
    public sealed class IsoCameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset = new(0f, 14f, -10f);
        [SerializeField] float smoothTime = 0.15f;

        Vector3 _velocity;

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        void Start()
        {
            transform.rotation = Quaternion.LookRotation(-offset);
            if (target != null) transform.position = target.position + offset;
        }

        void LateUpdate()
        {
            if (target == null) return;
            transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref _velocity, smoothTime);
        }
    }
}
