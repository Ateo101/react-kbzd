using System;
using UnityEngine;

namespace CrimeEmpire.Combat
{
    /// <summary>Picks the nearest enemy in range and shoots it. Hitscan: the tracer is only visual.</summary>
    public sealed class AutoShooter : MonoBehaviour
    {
        static readonly int ShootHash = Animator.StringToHash("Shoot");

        [SerializeField] Health self;
        [SerializeField] float damage = 10f;
        [SerializeField] float fireRate = 2f;
        [SerializeField] float range = 8f;
        [SerializeField] float retargetInterval = 0.25f;
        [SerializeField] bool faceTarget = true;
        [SerializeField] float turnSpeed = 720f;
        [SerializeField] Animator animator;
        [SerializeField] ParticleSystem muzzleFlash;

        public event Action<Health> Fired;

        public Health Target { get; private set; }
        public float Range => range;

        float _cooldown;
        float _retargetTimer;

        void Awake()
        {
            if (self == null) self = GetComponent<Health>();
        }

        public void Configure(float newDamage, float newFireRate, float newRange)
        {
            damage = newDamage;
            fireRate = Mathf.Max(0.01f, newFireRate);
            range = newRange;
        }

        void Update()
        {
            if (self == null || self.IsDead)
            {
                Target = null;
                return;
            }

            _cooldown -= Time.deltaTime;
            _retargetTimer -= Time.deltaTime;
            if (_retargetTimer <= 0f || !IsValid(Target))
            {
                _retargetTimer = retargetInterval;
                Target = Health.FindNearestEnemy(self.Team, transform.position, range);
            }

            if (Target == null) return;

            if (faceTarget)
            {
                var direction = Vector3.ProjectOnPlane(Target.transform.position - transform.position, Vector3.up);
                if (direction.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
            }

            if (_cooldown > 0f) return;
            _cooldown = 1f / fireRate;

            Target.TakeDamage(damage);
            if (muzzleFlash != null) muzzleFlash.Play();
            if (animator != null) animator.SetTrigger(ShootHash);
            Fired?.Invoke(Target);
        }

        bool IsValid(Health target) =>
            target != null && !target.IsDead &&
            (target.transform.position - transform.position).sqrMagnitude <= range * range;
    }
}
