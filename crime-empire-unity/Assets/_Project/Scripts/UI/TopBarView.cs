using System;
using CrimeEmpire.Core;
using CrimeEmpire.Economy;
using TMPro;
using UnityEngine;

namespace CrimeEmpire.UI
{
    /// <summary>Resource bar at the top: 15.5k | 1070 | 199/500 | 123.5k.</summary>
    public sealed class TopBarView : MonoBehaviour
    {
        [Serializable]
        sealed class Entry
        {
            public CurrencyType currency;
            public TMP_Text label;
            [Tooltip("Shows amount/cap, e.g. 199/500")] public bool showCap;
        }

        [SerializeField] Entry[] entries;

        Wallet Wallet => GameServices.Instance.Wallet;

        void OnEnable()
        {
            Wallet.Changed += OnChanged;
            foreach (var entry in entries) Refresh(entry);
        }

        void OnDisable()
        {
            if (GameServices.HasInstance) Wallet.Changed -= OnChanged;
        }

        void OnChanged(CurrencyType type, long _)
        {
            foreach (var entry in entries)
                if (entry.currency == type) Refresh(entry);
        }

        void Refresh(Entry entry)
        {
            if (entry.label == null) return;
            long amount = Wallet.Get(entry.currency);
            entry.label.text = entry.showCap && Wallet.HasCap(entry.currency)
                ? $"{NumberFormat.Short(amount)}/{NumberFormat.Short(Wallet.GetCap(entry.currency))}"
                : NumberFormat.Short(amount);
        }
    }
}
