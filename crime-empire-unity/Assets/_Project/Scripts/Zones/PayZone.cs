using System;
using CrimeEmpire.Core;
using CrimeEmpire.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrimeEmpire.Zones
{
    /// <summary>
    /// A pad the player stands on to pay for something bit by bit ("4600/5000").
    /// Partial payments are saved, so the player can leave and come back.
    /// </summary>
    public abstract class PayZone : MonoBehaviour
    {
        [SerializeField] CurrencyType currency = CurrencyType.Coins;
        [SerializeField] long price = 1000;
        [SerializeField, Tooltip("Seconds to pay the full price")] float fillDuration = 2f;
        [SerializeField, Tooltip("Grace time so walking across a pad does not spend money")] float startDelay = 0.35f;
        [SerializeField] protected PlayerTrigger trigger;
        [SerializeField, Tooltip("Pad, label and trigger. Hidden when the zone is not available")]
        protected GameObject zoneVisual;
        [SerializeField] TMP_Text progressLabel;
        [SerializeField] Image progressFill;

        /// <summary>Amount paid this tick. Hook coin-fly VFX and sounds here.</summary>
        public event Action<long> Paid;
        public event Action Completed;

        GradualPayment _payment;
        float _standingTime;

        protected static GameServices Services => GameServices.Instance;
        protected abstract string ProgressKey { get; }
        protected virtual bool CanPay => true;

        protected abstract void OnPaidInFull();

        protected virtual void Start()
        {
            _payment = new GradualPayment(Services.Wallet, currency, price, Services.Progress.GetPaid(ProgressKey), fillDuration);
            if (trigger != null) trigger.Entered += OnEntered;
            RefreshView();
        }

        protected virtual void OnDestroy()
        {
            if (trigger != null) trigger.Entered -= OnEntered;
        }

        protected virtual void Update()
        {
            if (_payment == null || _payment.IsComplete || trigger == null || !trigger.IsInside || !CanPay) return;

            _standingTime += Time.deltaTime;
            if (_standingTime < startDelay) return;

            long spent = _payment.Tick(Time.deltaTime);
            if (spent <= 0) return;

            Services.Progress.SetPaid(ProgressKey, _payment.Paid);
            Paid?.Invoke(spent);
            RefreshView();

            if (!_payment.IsComplete) return;
            OnPaidInFull();
            Completed?.Invoke();
        }

        protected void SetZoneVisible(bool visible)
        {
            if (zoneVisual != null) zoneVisual.SetActive(visible);
        }

        void OnEntered()
        {
            _standingTime = 0f;
            _payment?.ResetPending();
        }

        void RefreshView()
        {
            if (progressLabel != null)
                progressLabel.text = $"{NumberFormat.Short(_payment.Paid)}/{NumberFormat.Short(_payment.Price)}";
            if (progressFill != null)
                progressFill.fillAmount = _payment.Progress01;
        }
    }
}
