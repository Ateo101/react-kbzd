using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrimeEmpire.Combat
{
    public enum Team { Gang, Police, RivalGang }

    /// <summary>Hit points plus a registry of living units, so targeting never calls FindObjectsOfType.</summary>
    public sealed class Health : MonoBehaviour
    {
        static readonly List<Health> AliveList = new();

        [SerializeField] Team team;
        [SerializeField] float max = 100f;
        [SerializeField] Transform aimPoint;

        public static IReadOnlyList<Health> Alive => AliveList;

        public event Action<Health, float> Damaged;
        public event Action<Health> Died;

        public Team Team => team;
        public float Max => max;
        public float Current { get; private set; }
        public float Normalized => max > 0f ? Current / max : 0f;
        public bool IsDead => Current <= 0f;
        public Vector3 AimPoint => aimPoint != null ? aimPoint.position : transform.position + Vector3.up;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => AliveList.Clear();

        void OnEnable()
        {
            Current = max;
            if (!AliveList.Contains(this)) AliveList.Add(this);
        }

        void OnDisable() => AliveList.Remove(this);

        public void Init(Team newTeam, float maxHealth)
        {
            team = newTeam;
            max = Mathf.Max(1f, maxHealth);
            Revive();
        }

        public void Revive()
        {
            Current = max;
            if (isActiveAndEnabled && !AliveList.Contains(this)) AliveList.Add(this);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;

            Current = Mathf.Max(0f, Current - amount);
            Damaged?.Invoke(this, amount);
            if (!IsDead) return;

            AliveList.Remove(this);
            Died?.Invoke(this);
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            Current = Mathf.Min(max, Current + amount);
        }

        public static bool AreEnemies(Team a, Team b) => a != b;

        public static Health FindNearestEnemy(Team self, Vector3 from, float range)
        {
            Health best = null;
            float bestSqr = range * range;
            foreach (var candidate in AliveList)
            {
                if (!AreEnemies(self, candidate.team)) continue;
                float sqr = (candidate.transform.position - from).sqrMagnitude;
                if (sqr > bestSqr) continue;
                bestSqr = sqr;
                best = candidate;
            }
            return best;
        }
    }
}
