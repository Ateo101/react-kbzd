using System;
using System.Collections.Generic;
using CrimeEmpire.Bosses;
using CrimeEmpire.Core;
using CrimeEmpire.Economy;
using UnityEngine;

namespace CrimeEmpire.WorldMap
{
    /// <summary>
    /// Runs the city map offline against AI gangs: player marches, AI marches, district income.
    /// Views (district colors, march arrows) subscribe to the events and read <see cref="State"/>.
    /// </summary>
    public sealed class WorldMapController : MonoBehaviour
    {
        [SerializeField] string playerGangId = "player";
        [SerializeField] List<District> districts = new();
        [SerializeField] BossCatalog bossCatalog;

        [Header("Marches")]
        [SerializeField] double msPerDistanceUnit = 2000;
        [SerializeField] long marchCost = 100;
        [SerializeField] int basePlayerPower = 50;

        [Header("AI gangs")]
        [SerializeField] float aiDecisionInterval = 20f;
        [SerializeField] float aiPowerPerSecond = 0.5f;
        [SerializeField] float aiAttackMargin = 1.3f;

        [Header("Income")]
        [SerializeField] float incomeInterval = 60f;

        public event Action<BattleReport> BattleResolved;
        public event Action<March> MarchSent;

        public WorldMapState State { get; private set; }
        public string PlayerGangId => playerGangId;

        readonly Dictionary<string, float> _aiPower = new();
        readonly System.Random _rng = new();
        float _aiTimer;
        float _incomeTimer;

        static GameServices Services => GameServices.Instance;
        static long NowMs => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        void Awake()
        {
            foreach (var district in districts)
            {
                var savedOwner = Services.Progress.GetDistrictOwner(district.id);
                if (savedOwner != null) district.ownerId = savedOwner;
                if (!string.IsNullOrEmpty(district.ownerId) && district.ownerId != playerGangId)
                    _aiPower[district.ownerId] = 0f;
            }
            State = new WorldMapState(districts);
        }

        /// <summary>Squad strength: every recruited boss adds power, levels make them stronger.</summary>
        public int PlayerPower()
        {
            int power = basePlayerPower;
            var roster = Services.Bosses;
            foreach (var bossId in roster.RecruitedIds)
            {
                var boss = bossCatalog != null ? bossCatalog.Find(bossId) : null;
                if (boss != null) power += boss.Power(roster.GetLevel(bossId));
            }
            return power;
        }

        public bool CanPlayerAttack(string fromId, string targetId) =>
            State.CanAttack(playerGangId, fromId, targetId) && Services.Wallet.CanAfford(CurrencyType.CleanCash, marchCost);

        public bool SendPlayerMarch(string fromId, string targetId)
        {
            if (!CanPlayerAttack(fromId, targetId)) return false;

            var march = State.SendMarch(playerGangId, fromId, targetId, PlayerPower(), NowMs, msPerDistanceUnit);
            if (march == null) return false;

            Services.Wallet.TrySpend(CurrencyType.CleanCash, marchCost);
            MarchSent?.Invoke(march);
            return true;
        }

        void Update()
        {
            foreach (var report in State.Tick(NowMs, _rng))
            {
                if (report.AttackerWon) Services.Progress.SetDistrictOwner(report.DistrictId, report.AttackerId);
                BattleResolved?.Invoke(report);
            }

            TickAi();
            TickIncome();
        }

        void TickAi()
        {
            var gangs = new List<string>(_aiPower.Keys);
            foreach (var gang in gangs) _aiPower[gang] += aiPowerPerSecond * Time.deltaTime;

            _aiTimer += Time.deltaTime;
            if (_aiTimer < aiDecisionInterval) return;
            _aiTimer = 0f;

            foreach (var gang in gangs)
            {
                int power = Mathf.FloorToInt(_aiPower[gang]);
                if (!AIGangBrain.TryPickAttack(State, gang, power, aiAttackMargin, out var from, out var target)) continue;

                var march = State.SendMarch(gang, from, target, power, NowMs, msPerDistanceUnit);
                if (march == null) continue;
                _aiPower[gang] = 0f;
                MarchSent?.Invoke(march);
            }
        }

        void TickIncome()
        {
            _incomeTimer += Time.deltaTime;
            if (_incomeTimer < incomeInterval) return;
            _incomeTimer = 0f;

            long income = State.IncomePerHour(playerGangId) * (long)incomeInterval / 3600;
            Services.Wallet.Add(CurrencyType.CleanCash, income);
        }
    }
}
