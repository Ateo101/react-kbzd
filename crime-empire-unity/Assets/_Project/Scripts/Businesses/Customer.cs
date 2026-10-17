using System;
using UnityEngine;
using UnityEngine.AI;

namespace CrimeEmpire.Businesses
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class Customer : MonoBehaviour
    {
        static readonly int SpeedHash = Animator.StringToHash("Speed");

        [SerializeField] Animator animator;

        NavMeshAgent _agent;
        Action<Customer> _onLeft;
        bool _leaving;

        public bool HasArrived =>
            !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.05f;

        void Awake() => _agent = GetComponent<NavMeshAgent>();

        public void Spawn(Vector3 position, Action<Customer> onLeft)
        {
            _agent.Warp(position);
            _leaving = false;
            _onLeft = onLeft;
        }

        public void MoveTo(Vector3 position) => _agent.SetDestination(position);

        public void Leave(Vector3 exit)
        {
            _leaving = true;
            _agent.SetDestination(exit);
        }

        void Update()
        {
            if (animator != null) animator.SetFloat(SpeedHash, _agent.velocity.magnitude / Mathf.Max(0.01f, _agent.speed));
            if (!_leaving || !HasArrived) return;

            _leaving = false;
            _onLeft?.Invoke(this);
        }
    }
}
