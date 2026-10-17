using System;
using System.Collections.Generic;
using CrimeEmpire.Core;
using CrimeEmpire.Economy;
using CrimeEmpire.Zones;
using UnityEngine;

namespace CrimeEmpire.Businesses
{
    /// <summary>Money stacked on the table. Walk over it to pick it up.</summary>
    public sealed class CashPile : MonoBehaviour
    {
        [SerializeField] PlayerTrigger pickup;
        [SerializeField] CurrencyType currency = CurrencyType.Coins;

        [Header("Visual stack")]
        [SerializeField] Transform stackRoot;
        [SerializeField] GameObject billPrefab;
        [SerializeField] long valuePerBill = 10;
        [SerializeField] int columns = 3;
        [SerializeField] int rows = 2;
        [SerializeField] int maxVisible = 36;
        [SerializeField] Vector3 cellSize = new(0.45f, 0.1f, 0.25f);

        readonly List<GameObject> _bills = new();
        long _stored;

        /// <summary>Amount added to the wallet. Hook coin-fly VFX here.</summary>
        public event Action<long> Collected;

        public long Stored => _stored;

        public void Add(long amount)
        {
            if (amount <= 0) return;
            _stored += amount;
            RefreshVisual();
        }

        void Update()
        {
            if (_stored <= 0 || pickup == null || !pickup.IsInside) return;

            long added = GameServices.Instance.Wallet.Add(currency, _stored);
            if (added <= 0) return; // wallet is full (capped currency): money stays on the table

            _stored -= added;
            RefreshVisual();
            Collected?.Invoke(added);
        }

        void RefreshVisual()
        {
            if (billPrefab == null || stackRoot == null) return;

            long needed = (_stored + valuePerBill - 1) / Math.Max(1, valuePerBill);
            int visible = (int)Math.Min(maxVisible, needed);
            int perLayer = Mathf.Max(1, columns * rows);

            while (_bills.Count < visible)
            {
                int i = _bills.Count;
                int layer = i / perLayer;
                int column = i % perLayer % columns;
                int row = i % perLayer / columns;
                var bill = Instantiate(billPrefab, stackRoot);
                bill.transform.localPosition = new Vector3(column * cellSize.x, layer * cellSize.y, row * cellSize.z);
                _bills.Add(bill);
            }

            for (int i = 0; i < _bills.Count; i++)
                _bills[i].SetActive(i < visible);
        }
    }
}
