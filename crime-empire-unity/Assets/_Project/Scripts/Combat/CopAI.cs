using UnityEngine;
using UnityEngine.AI;

namespace CrimeEmpire.Combat
{
    /// <summary>Walks to the nearest gang member or to the objective, stops and shoots when something is in range.</summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Health), typeof(AutoShooter))]
    public sealed class CopAI : MonoBehaviour
    {
        static readonly int SpeedHash = Animator.StringToHash("Speed");
        static readonly int DieHash = Animator.StringToHash("Die");

        [SerializeField] float aggroRange = 15f;
        [SerializeField] float repathInterval = 0.5f;
        [SerializeField] float corpseLifetime = 2f;
        [SerializeField] Animator animator;

        NavMeshAgent _agent;
        Health _health;
        AutoShooter _shooter;
        Transform _objective;
        float _repathTimer;

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
            _shooter = GetComponent<AutoShooter>();
            _health.Died += OnDied;
        }

        public void Init(Transform objective) => _objective = objective;

        void Update()
        {
            bool engaged = _shooter.Target != null;
            _agent.isStopped = engaged;
            _agent.updateRotation = !engaged;

            if (animator != null) animator.SetFloat(SpeedHash, _agent.velocity.magnitude / Mathf.Max(0.01f, _agent.speed));
            if (engaged) return;

            _repathTimer -= Time.deltaTime;
            if (_repathTimer > 0f) return;
            _repathTimer = repathInterval;

            var enemy = Health.FindNearestEnemy(_health.Team, transform.position, aggroRange);
            var goal = enemy != null ? enemy.transform : _objective;
            if (goal != null) _agent.SetDestination(goal.position);
        }

        void OnDied(Health _)
        {
            _agent.isStopped = true;
            _shooter.enabled = false;
            enabled = false;
            if (animator != null) animator.SetTrigger(DieHash);
            Destroy(gameObject, corpseLifetime);
        }
    }
}
