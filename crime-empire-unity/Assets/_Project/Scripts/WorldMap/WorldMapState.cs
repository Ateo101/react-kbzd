using System;
using System.Collections.Generic;

namespace CrimeEmpire.WorldMap
{
    /// <summary>
    /// District ownership, marches and battles. No UnityEngine here on purpose:
    /// the same rules can run on a server once PvP goes online.
    /// </summary>
    public sealed class WorldMapState
    {
        readonly Dictionary<string, District> _districts = new();
        readonly List<March> _marches = new();

        public WorldMapState(IEnumerable<District> districts)
        {
            foreach (var district in districts)
                if (district != null && !string.IsNullOrEmpty(district.id)) _districts[district.id] = district;
        }

        public IEnumerable<District> Districts => _districts.Values;
        public IReadOnlyList<March> Marches => _marches;

        public District Get(string id) => id != null && _districts.TryGetValue(id, out var district) ? district : null;

        public bool CanAttack(string gangId, string fromId, string targetId)
        {
            var from = Get(fromId);
            var target = Get(targetId);
            return from != null && target != null
                && from.ownerId == gangId
                && target.ownerId != gangId
                && from.neighbors.Contains(targetId)
                && !_marches.Exists(m => m.AttackerId == gangId && m.TargetId == targetId);
        }

        public March SendMarch(string gangId, string fromId, string targetId, int power, long nowUnixMs, double msPerDistanceUnit)
        {
            if (power <= 0 || !CanAttack(gangId, fromId, targetId)) return null;

            var from = Get(fromId);
            var target = Get(targetId);
            double distance = Math.Sqrt(Math.Pow(target.x - from.x, 2) + Math.Pow(target.y - from.y, 2));
            var march = new March(gangId, fromId, targetId, power, nowUnixMs, nowUnixMs + (long)(distance * msPerDistanceUnit));
            _marches.Add(march);
            return march;
        }

        /// <summary>Resolves every march that has arrived by <paramref name="nowUnixMs"/>, in arrival order.</summary>
        public List<BattleReport> Tick(long nowUnixMs, Random rng)
        {
            var reports = new List<BattleReport>();
            var arrived = _marches.FindAll(m => m.ArriveUnixMs <= nowUnixMs);
            if (arrived.Count == 0) return reports;

            arrived.Sort((a, b) => a.ArriveUnixMs.CompareTo(b.ArriveUnixMs));
            foreach (var march in arrived)
            {
                _marches.Remove(march);
                var target = Get(march.TargetId);
                if (target == null) continue;

                // Someone else took it first and it is ours now: the march just reinforces.
                if (target.ownerId == march.AttackerId)
                {
                    target.garrison += march.Power;
                    continue;
                }

                var (attackerWon, survivors) = Resolve(march.Power, target.garrison, rng);
                reports.Add(new BattleReport(target.id, march.AttackerId, target.ownerId, march.Power, target.garrison, attackerWon, survivors));

                if (attackerWon) target.ownerId = march.AttackerId;
                target.garrison = survivors;
            }
            return reports;
        }

        /// <summary>
        /// Lanchester's square law with ±15% luck: the stronger side wins and keeps sqrt(a² - b²).
        /// Big armies win cheaply, even fights are bloody.
        /// </summary>
        public static (bool attackerWon, int survivors) Resolve(int attack, int defense, Random rng)
        {
            double effective = attack * (0.85 + rng.NextDouble() * 0.3);
            if (effective > defense)
            {
                int left = (int)Math.Round(Math.Sqrt(effective * effective - (double)defense * defense));
                return (true, Math.Clamp(left, 1, Math.Max(1, attack)));
            }

            int held = (int)Math.Round(Math.Sqrt((double)defense * defense - effective * effective));
            return (false, Math.Max(1, held));
        }

        public long IncomePerHour(string gangId)
        {
            long total = 0;
            foreach (var district in _districts.Values)
                if (district.ownerId == gangId) total += district.incomePerHour;
            return total;
        }
    }
}
