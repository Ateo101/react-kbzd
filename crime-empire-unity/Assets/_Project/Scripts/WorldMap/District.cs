using System;
using System.Collections.Generic;

namespace CrimeEmpire.WorldMap
{
    [Serializable]
    public sealed class District
    {
        public string id;
        public string displayName;
        public string ownerId;
        public int garrison = 100;
        public long incomePerHour = 1000;
        public float x;
        public float y;
        public List<string> neighbors = new();
    }

    public sealed class March
    {
        public string AttackerId { get; }
        public string FromId { get; }
        public string TargetId { get; }
        public int Power { get; }
        public long DepartUnixMs { get; }
        public long ArriveUnixMs { get; }

        public March(string attackerId, string fromId, string targetId, int power, long departUnixMs, long arriveUnixMs)
        {
            AttackerId = attackerId;
            FromId = fromId;
            TargetId = targetId;
            Power = power;
            DepartUnixMs = departUnixMs;
            ArriveUnixMs = arriveUnixMs;
        }

        public float Progress01(long nowUnixMs) =>
            ArriveUnixMs <= DepartUnixMs ? 1f : Math.Clamp((float)(nowUnixMs - DepartUnixMs) / (ArriveUnixMs - DepartUnixMs), 0f, 1f);
    }

    public readonly struct BattleReport
    {
        public readonly string DistrictId;
        public readonly string AttackerId;
        public readonly string DefenderId;
        public readonly int AttackPower;
        public readonly int DefensePower;
        public readonly bool AttackerWon;
        public readonly int Survivors;

        public BattleReport(string districtId, string attackerId, string defenderId, int attackPower, int defensePower, bool attackerWon, int survivors)
        {
            DistrictId = districtId;
            AttackerId = attackerId;
            DefenderId = defenderId;
            AttackPower = attackPower;
            DefensePower = defensePower;
            AttackerWon = attackerWon;
            Survivors = survivors;
        }
    }
}
