using System;
using System.Collections.Generic;

namespace CrimeEmpire.Save
{
    /// <summary>Everything the player has built: unlocked zones, partial payments, levels, map ownership.</summary>
    public sealed class ProgressState
    {
        readonly HashSet<string> _unlocked = new();
        readonly Dictionary<string, long> _paid = new();
        readonly Dictionary<string, int> _levels = new();
        readonly Dictionary<string, string> _districtOwners = new();

        public event Action<string> ZoneUnlocked;
        public event Action<string, int> LevelChanged;

        public IEnumerable<string> Unlocked => _unlocked;
        public IEnumerable<KeyValuePair<string, long>> Payments => _paid;
        public IEnumerable<KeyValuePair<string, int>> Levels => _levels;
        public IEnumerable<KeyValuePair<string, string>> DistrictOwners => _districtOwners;

        public bool IsUnlocked(string key) => _unlocked.Contains(key);

        public void MarkUnlocked(string key)
        {
            if (!_unlocked.Add(key)) return;
            _paid.Remove(key);
            ZoneUnlocked?.Invoke(key);
        }

        public long GetPaid(string key) => _paid.TryGetValue(key, out var paid) ? paid : 0;

        public void SetPaid(string key, long amount)
        {
            if (amount <= 0) _paid.Remove(key);
            else _paid[key] = amount;
        }

        public int GetLevel(string key) => _levels.TryGetValue(key, out var level) ? level : 1;

        public void SetLevel(string key, int level)
        {
            level = Math.Max(1, level);
            if (GetLevel(key) == level) return;
            _levels[key] = level;
            LevelChanged?.Invoke(key, level);
        }

        public string GetDistrictOwner(string districtId) =>
            _districtOwners.TryGetValue(districtId, out var owner) ? owner : null;

        public void SetDistrictOwner(string districtId, string ownerId) => _districtOwners[districtId] = ownerId;
    }
}
