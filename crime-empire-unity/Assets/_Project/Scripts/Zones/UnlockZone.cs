using CrimeEmpire.Core;
using UnityEngine;

namespace CrimeEmpire.Zones
{
    /// <summary>
    /// Builds a room, business or upgrade when paid. Zones chain through <see cref="requires"/>,
    /// which is how the base grows step by step (bar -> office -> gym -> shooting range ...).
    /// </summary>
    public sealed class UnlockZone : PayZone
    {
        [SerializeField] string id;
        [SerializeField] UnlockZone[] requires;
        [SerializeField] GameObject[] enableOnUnlock;
        [SerializeField] GameObject[] disableOnUnlock;

        public string Id => id;
        public bool IsUnlocked => Services.Progress.IsUnlocked(ProgressKey);

        protected override string ProgressKey => "zone:" + id;
        protected override bool CanPay => !IsUnlocked && RequirementsMet();

        protected override void Start()
        {
            base.Start();
            Services.Progress.ZoneUnlocked += OnAnyZoneUnlocked;
            ApplyState();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (GameServices.HasInstance) Services.Progress.ZoneUnlocked -= OnAnyZoneUnlocked;
        }

        protected override void OnPaidInFull() => Services.Progress.MarkUnlocked(ProgressKey);

        void OnAnyZoneUnlocked(string _) => ApplyState();

        bool RequirementsMet()
        {
            if (requires == null) return true;
            foreach (var zone in requires)
                if (zone != null && !zone.IsUnlocked) return false;
            return true;
        }

        void ApplyState()
        {
            bool unlocked = IsUnlocked;
            SetZoneVisible(!unlocked && RequirementsMet());
            SetAllActive(enableOnUnlock, unlocked);
            SetAllActive(disableOnUnlock, !unlocked);
        }

        static void SetAllActive(GameObject[] objects, bool active)
        {
            if (objects == null) return;
            foreach (var go in objects)
                if (go != null) go.SetActive(active);
        }

        void OnValidate()
        {
            if (string.IsNullOrEmpty(id)) id = name;
        }
    }
}
