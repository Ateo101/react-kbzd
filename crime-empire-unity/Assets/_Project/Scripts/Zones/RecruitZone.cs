using CrimeEmpire.Bosses;
using CrimeEmpire.Core;
using UnityEngine;

namespace CrimeEmpire.Zones
{
    /// <summary>Jail cell with a boss inside: stand on the pad, pay, and the boss joins the gang.</summary>
    public sealed class RecruitZone : PayZone
    {
        [SerializeField] BossDefinition boss;
        [SerializeField, Tooltip("The boss model sitting in the cell; hidden once recruited")]
        GameObject cellOccupant;

        protected override string ProgressKey => "recruit:" + boss.Id;
        protected override bool CanPay => !Services.Bosses.IsRecruited(boss.Id);

        protected override void Start()
        {
            base.Start();
            Services.Bosses.Recruited += OnRecruited;
            Refresh();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (GameServices.HasInstance) Services.Bosses.Recruited -= OnRecruited;
        }

        protected override void OnPaidInFull()
        {
            Services.Progress.SetPaid(ProgressKey, 0);
            Services.Bosses.Recruit(boss.Id);
        }

        void OnRecruited(string bossId)
        {
            if (bossId == boss.Id) Refresh();
        }

        void Refresh()
        {
            bool recruited = Services.Bosses.IsRecruited(boss.Id);
            SetZoneVisible(!recruited);
            if (cellOccupant != null) cellOccupant.SetActive(!recruited);
        }
    }
}
