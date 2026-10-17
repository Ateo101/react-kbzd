using System;
using System.Collections.Generic;
using CrimeEmpire.Bosses;
using CrimeEmpire.Core;
using CrimeEmpire.Economy;
using CrimeEmpire.Zones;
using UnityEngine;
using UnityEngine.Pool;

namespace CrimeEmpire.Businesses
{
    /// <summary>
    /// Bar, casino, pawn shop... Customers line up, someone at the counter serves them,
    /// money lands on the <see cref="CashPile"/>. The player serves by standing at the counter,
    /// an assigned boss serves on their own (and keeps earning while the game is closed).
    /// </summary>
    public sealed class Business : MonoBehaviour
    {
        static readonly List<Business> ActiveList = new();

        [SerializeField] string id;
        [SerializeField] BossCatalog bossCatalog;

        [Header("Customers")]
        [SerializeField] Customer customerPrefab;
        [SerializeField] Transform spawnPoint;
        [SerializeField] Transform exitPoint;
        [SerializeField] CustomerQueue queue;
        [SerializeField] float spawnInterval = 3f;

        [Header("Service")]
        [SerializeField] PlayerTrigger counter;
        [SerializeField, Tooltip("Where the manager boss stands")] Transform managerSpot;
        [SerializeField] float baseServiceTime = 1.2f;
        [SerializeField] long basePrice = 10;
        [SerializeField, Tooltip("0 = sells without stock")] long contrabandPerSale;
        [SerializeField] CashPile cashPile;

        [Header("Upgrades")]
        [SerializeField] int maxLevel = 30;
        [SerializeField] float priceGrowthPerLevel = 0.15f;
        [SerializeField] float speedGrowthPerLevel = 0.05f;
        [SerializeField] long baseUpgradeCost = 200;
        [SerializeField] float upgradeCostGrowth = 1.35f;

        public static IReadOnlyList<Business> Active => ActiveList;

        /// <summary>True when a customer waits but there is no stock. Show an "out of stock" icon.</summary>
        public event Action<bool> StockStateChanged;
        public event Action<long> Sold;

        ObjectPool<Customer> _pool;
        GameObject _managerView;
        float _spawnTimer;
        float _serviceTimer;
        bool _outOfStock;

        static GameServices Services => GameServices.Instance;
        string LevelKey => "biz:" + id;

        public string Id => id;
        public int Level => Services.Progress.GetLevel(LevelKey);
        public bool IsMaxLevel => Level >= maxLevel;
        public long PricePerSale => (long)Math.Round(basePrice * (1 + priceGrowthPerLevel * (Level - 1)));
        public long UpgradeCost => (long)Math.Round(baseUpgradeCost * Math.Pow(upgradeCostGrowth, Level - 1));
        public BossDefinition Manager => bossCatalog != null ? bossCatalog.Find(Services.Bosses.GetAssignedBoss(id)) : null;

        public float ServiceTime
        {
            get
            {
                float time = baseServiceTime / (1f + speedGrowthPerLevel * (Level - 1));
                var manager = Manager;
                return manager != null ? time * (1f - manager.ServiceSpeedBonus) : time;
            }
        }

        /// <summary>Coins per second a manager makes alone. Used for offline income.</summary>
        public double IdleIncomePerSecond =>
            Manager != null ? PricePerSale / Math.Max(ServiceTime, spawnInterval) : 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => ActiveList.Clear();

        void Awake()
        {
            _pool = new ObjectPool<Customer>(
                () => Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation, transform),
                customer => customer.gameObject.SetActive(true),
                customer => customer.gameObject.SetActive(false),
                customer => Destroy(customer.gameObject),
                maxSize: 32);
        }

        void OnEnable() => ActiveList.Add(this);
        void OnDisable() => ActiveList.Remove(this);

        void Start()
        {
            Services.Bosses.AssignmentChanged += OnAssignmentChanged;
            RefreshManagerView();
        }

        void OnDestroy()
        {
            if (GameServices.HasInstance) Services.Bosses.AssignmentChanged -= OnAssignmentChanged;
        }

        public bool TryUpgrade()
        {
            if (IsMaxLevel || !Services.Wallet.TrySpend(CurrencyType.Coins, UpgradeCost)) return false;
            Services.Progress.SetLevel(LevelKey, Level + 1);
            return true;
        }

        void Update()
        {
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= spawnInterval && queue.HasFreeSlot)
            {
                _spawnTimer = 0f;
                var customer = _pool.Get();
                customer.Spawn(spawnPoint.position, _pool.Release);
                queue.Enqueue(customer);
            }

            var head = queue.Head;
            bool staffed = counter.IsInside || Manager != null;
            if (head == null || !head.HasArrived || !staffed)
            {
                _serviceTimer = 0f;
                return;
            }

            bool hasStock = Services.Wallet.CanAfford(CurrencyType.Contraband, contrabandPerSale);
            SetOutOfStock(!hasStock);
            if (!hasStock) return;

            _serviceTimer += Time.deltaTime;
            if (_serviceTimer < ServiceTime) return;
            _serviceTimer = 0f;

            Services.Wallet.TrySpend(CurrencyType.Contraband, contrabandPerSale);
            queue.Dequeue(head);
            head.Leave(exitPoint.position);
            cashPile.Add(PricePerSale);
            Sold?.Invoke(PricePerSale);
        }

        void SetOutOfStock(bool value)
        {
            if (_outOfStock == value) return;
            _outOfStock = value;
            StockStateChanged?.Invoke(value);
        }

        void OnAssignmentChanged(string businessId, string _)
        {
            if (businessId == id) RefreshManagerView();
        }

        void RefreshManagerView()
        {
            if (_managerView != null) Destroy(_managerView);
            var manager = Manager;
            if (manager == null || manager.Prefab == null || managerSpot == null) return;

            _managerView = Instantiate(manager.Prefab, managerSpot.position, managerSpot.rotation, managerSpot);
            // Behind the counter the boss is decoration: no shooting at customers.
            foreach (var behaviour in _managerView.GetComponentsInChildren<MonoBehaviour>())
                behaviour.enabled = false;
        }

        void OnValidate()
        {
            if (string.IsNullOrEmpty(id)) id = name;
        }
    }
}
