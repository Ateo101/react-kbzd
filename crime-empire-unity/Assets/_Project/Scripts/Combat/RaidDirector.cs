using System;
using System.Collections;
using System.Collections.Generic;
using CrimeEmpire.Bosses;
using CrimeEmpire.Core;
using CrimeEmpire.Economy;
using UnityEngine;

namespace CrimeEmpire.Combat
{
    /// <summary>
    /// "Defend your turf": police come in waves at a building, recruited bosses hold the defense points.
    /// Win = all waves cleared before the objective falls.
    /// </summary>
    public sealed class RaidDirector : MonoBehaviour
    {
        [Serializable]
        public sealed class Wave
        {
            public CopAI copPrefab;
            public int count = 5;
            public float spawnInterval = 0.6f;
            public float delayBefore = 3f;
        }

        [SerializeField] Health objective;
        [SerializeField] Transform[] spawnPoints;
        [SerializeField] Wave[] waves;
        [SerializeField] Transform[] defenderPoints;
        [SerializeField] BossCatalog bossCatalog;
        [SerializeField] long winReward = 500;

        public event Action<int, int> WaveStarted; // index, total
        public event Action<bool, long> Finished;  // won, reward

        readonly List<Health> _cops = new();
        readonly List<GameObject> _defenders = new();

        public bool IsRunning { get; private set; }

        public void StartRaid()
        {
            if (IsRunning || waves.Length == 0 || spawnPoints.Length == 0) return;
            StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            IsRunning = true;
            objective.Revive();
            SpawnDefenders();

            for (int i = 0; i < waves.Length && !objective.IsDead; i++)
            {
                var wave = waves[i];
                WaveStarted?.Invoke(i, waves.Length);
                yield return new WaitForSeconds(wave.delayBefore);

                for (int n = 0; n < wave.count && !objective.IsDead; n++)
                {
                    SpawnCop(wave.copPrefab);
                    yield return new WaitForSeconds(wave.spawnInterval);
                }

                yield return new WaitUntil(() => objective.IsDead || AllCopsDown());
            }

            bool won = !objective.IsDead;
            Cleanup();

            long reward = 0;
            if (won) reward = GameServices.Instance.Wallet.Add(CurrencyType.DirtyCash, winReward);

            IsRunning = false;
            Finished?.Invoke(won, reward);
        }

        void SpawnCop(CopAI prefab)
        {
            var point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            var cop = Instantiate(prefab, point.position, point.rotation);
            cop.Init(objective.transform);
            _cops.Add(cop.GetComponent<Health>());
        }

        void SpawnDefenders()
        {
            var roster = GameServices.Instance.Bosses;
            int slot = 0;
            foreach (var bossId in roster.RecruitedIds)
            {
                if (slot >= defenderPoints.Length) break;
                var boss = bossCatalog.Find(bossId);
                if (boss == null || boss.Prefab == null) continue;

                var point = defenderPoints[slot++];
                var go = Instantiate(boss.Prefab, point.position, point.rotation);
                int level = roster.GetLevel(bossId);
                if (go.TryGetComponent(out Health health)) health.Init(Team.Gang, boss.Health(level));
                if (go.TryGetComponent(out AutoShooter shooter)) shooter.Configure(boss.Damage(level), boss.FireRate, boss.Range);
                _defenders.Add(go);
            }
        }

        bool AllCopsDown()
        {
            foreach (var cop in _cops)
                if (cop != null && !cop.IsDead) return false;
            return true;
        }

        void Cleanup()
        {
            foreach (var cop in _cops)
                if (cop != null) Destroy(cop.gameObject);
            foreach (var defender in _defenders)
                if (defender != null) Destroy(defender);
            _cops.Clear();
            _defenders.Clear();
        }
    }
}
