using System;
using System.Collections.Generic;

namespace CrimeEmpire.Economy
{
    /// <summary>All player currencies. Plain C# so it can be unit tested and later mirrored on a server.</summary>
    public sealed class Wallet
    {
        readonly Dictionary<CurrencyType, long> _amounts = new();
        readonly Dictionary<CurrencyType, long> _caps = new();

        /// <summary>Fires with the new amount whenever an amount or a cap changes.</summary>
        public event Action<CurrencyType, long> Changed;

        public IEnumerable<KeyValuePair<CurrencyType, long>> Amounts => _amounts;
        public IEnumerable<KeyValuePair<CurrencyType, long>> Caps => _caps;

        public long Get(CurrencyType type) => _amounts.TryGetValue(type, out var value) ? value : 0;

        public long GetCap(CurrencyType type) => _caps.TryGetValue(type, out var cap) ? cap : long.MaxValue;

        public bool HasCap(CurrencyType type) => _caps.ContainsKey(type);

        public void SetCap(CurrencyType type, long cap)
        {
            _caps[type] = Math.Max(0, cap);
            if (_amounts.TryGetValue(type, out var current) && current > _caps[type]) _amounts[type] = _caps[type];
            // Always notify: the UI shows the cap ("199/500"), so it changes even when the amount does not.
            Changed?.Invoke(type, Get(type));
        }

        /// <summary>Adds up to the cap. Returns how much was actually added.</summary>
        public long Add(CurrencyType type, long amount)
        {
            if (amount <= 0) return 0;
            long current = Get(type);
            long room = Math.Max(0, GetCap(type) - current);
            long added = Math.Min(amount, room);
            if (added == 0) return 0;
            Set(type, current + added);
            return added;
        }

        public bool CanAfford(CurrencyType type, long amount) => amount <= 0 || Get(type) >= amount;

        public bool TrySpend(CurrencyType type, long amount)
        {
            if (amount < 0) return false;
            if (!CanAfford(type, amount)) return false;
            if (amount > 0) Set(type, Get(type) - amount);
            return true;
        }

        /// <summary>Spends as much as possible up to <paramref name="max"/>. Used by zones that are paid gradually.</summary>
        public long SpendUpTo(CurrencyType type, long max)
        {
            long spent = Math.Min(Math.Max(0, max), Get(type));
            if (spent > 0) Set(type, Get(type) - spent);
            return spent;
        }

        /// <summary>Sets a value from a save file. Respects the cap.</summary>
        public void Restore(CurrencyType type, long amount) =>
            Set(type, Math.Clamp(amount, 0, GetCap(type)));

        void Set(CurrencyType type, long value)
        {
            if (_amounts.TryGetValue(type, out var old) && old == value) return;
            _amounts[type] = value;
            Changed?.Invoke(type, value);
        }
    }
}
