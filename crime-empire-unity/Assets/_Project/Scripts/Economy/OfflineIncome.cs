using System;
using CrimeEmpire.Businesses;
using CrimeEmpire.Core;
using UnityEngine;

namespace CrimeEmpire.Economy
{
    /// <summary>
    /// Pays what managed businesses earned while the game was closed, up to a cap.
    /// The UI shows the popup and can offer x2 for a rewarded ad.
    /// </summary>
    public sealed class OfflineIncome : MonoBehaviour
    {
        [SerializeField] float maxOfflineHours = 2f;
        [SerializeField] float minAwayMinutes = 1f;

        public event Action<long, TimeSpan> Granted;

        void Start()
        {
            // Start runs after every Business.OnEnable, so all earners are registered by now.
            GameServices.Instance.ReturnedAfterAway += OnReturned;
            Grant();
        }

        void OnDestroy()
        {
            if (GameServices.HasInstance) GameServices.Instance.ReturnedAfterAway -= OnReturned;
        }

        void OnReturned(TimeSpan _) => Grant();

        void Grant()
        {
            var away = GameServices.Instance.ConsumeTimeAway();
            if (away.TotalMinutes < minAwayMinutes) return;

            double seconds = Math.Min(away.TotalSeconds, maxOfflineHours * 3600.0);
            double perSecond = 0;
            foreach (var business in Business.Active) perSecond += business.IdleIncomePerSecond;

            long amount = (long)(perSecond * seconds);
            if (amount <= 0) return;

            long added = GameServices.Instance.Wallet.Add(CurrencyType.Coins, amount);
            Granted?.Invoke(added, away);
        }
    }
}
