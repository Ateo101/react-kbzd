using System;
using System.Collections.Generic;

namespace CrimeEmpire.Bosses
{
    /// <summary>Which bosses the player owns, their levels and which business each one runs.</summary>
    public sealed class BossRoster
    {
        readonly Dictionary<string, int> _levels = new();
        readonly Dictionary<string, string> _assignments = new(); // businessId -> bossId

        public event Action<string> Recruited;
        public event Action<string, int> LevelChanged;
        public event Action<string, string> AssignmentChanged; // businessId, bossId (null when cleared)

        public IEnumerable<KeyValuePair<string, int>> Levels => _levels;
        public IEnumerable<KeyValuePair<string, string>> Assignments => _assignments;
        public IEnumerable<string> RecruitedIds => _levels.Keys;

        public bool IsRecruited(string bossId) => bossId != null && _levels.ContainsKey(bossId);

        public int GetLevel(string bossId) => _levels.TryGetValue(bossId, out var level) ? level : 0;

        public void Recruit(string bossId)
        {
            if (string.IsNullOrEmpty(bossId) || IsRecruited(bossId)) return;
            _levels[bossId] = 1;
            Recruited?.Invoke(bossId);
        }

        public void LevelUp(string bossId)
        {
            if (!IsRecruited(bossId)) return;
            _levels[bossId]++;
            LevelChanged?.Invoke(bossId, _levels[bossId]);
        }

        public void Restore(string bossId, int level)
        {
            if (string.IsNullOrEmpty(bossId)) return;
            _levels[bossId] = Math.Max(1, level);
        }

        public string GetAssignedBoss(string businessId) =>
            _assignments.TryGetValue(businessId, out var bossId) ? bossId : null;

        /// <summary>A boss runs one business at a time; assigning moves them.</summary>
        public void Assign(string businessId, string bossId)
        {
            if (string.IsNullOrEmpty(businessId)) return;

            if (string.IsNullOrEmpty(bossId))
            {
                if (_assignments.Remove(businessId)) AssignmentChanged?.Invoke(businessId, null);
                return;
            }

            if (!IsRecruited(bossId) || GetAssignedBoss(businessId) == bossId) return;

            string previousBusiness = null;
            foreach (var pair in _assignments)
                if (pair.Value == bossId) previousBusiness = pair.Key;
            if (previousBusiness != null)
            {
                _assignments.Remove(previousBusiness);
                AssignmentChanged?.Invoke(previousBusiness, null);
            }

            _assignments[businessId] = bossId;
            AssignmentChanged?.Invoke(businessId, bossId);
        }
    }
}
