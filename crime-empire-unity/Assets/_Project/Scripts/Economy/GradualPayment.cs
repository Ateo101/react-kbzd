using System;

namespace CrimeEmpire.Economy
{
    /// <summary>
    /// Drains money into a price over time while the player stands in a zone ("4600/5000").
    /// The full price always takes about the same time, so big and small zones feel the same.
    /// </summary>
    public sealed class GradualPayment
    {
        readonly Wallet _wallet;
        readonly CurrencyType _currency;
        readonly double _ratePerSecond;
        double _pending;

        public long Price { get; }
        public long Paid { get; private set; }
        public bool IsComplete => Paid >= Price;
        public float Progress01 => Price <= 0 ? 1f : (float)((double)Paid / Price);

        public GradualPayment(Wallet wallet, CurrencyType currency, long price, long alreadyPaid, float fullDurationSeconds)
        {
            _wallet = wallet;
            _currency = currency;
            Price = Math.Max(0, price);
            Paid = Math.Clamp(alreadyPaid, 0, Price);
            _ratePerSecond = Price / Math.Max(0.01, fullDurationSeconds);
        }

        /// <summary>Returns the amount moved from the wallet this tick.</summary>
        public long Tick(float deltaTime)
        {
            if (IsComplete || deltaTime <= 0) return 0;

            _pending += _ratePerSecond * deltaTime;
            long want = Math.Min((long)_pending, Price - Paid);
            if (want <= 0) return 0;

            long spent = _wallet.SpendUpTo(_currency, want);
            // Out of money: drop the remainder instead of building up a burst for later.
            _pending = spent < want ? 0 : _pending - spent;
            Paid += spent;
            return spent;
        }

        public void ResetPending() => _pending = 0;
    }
}
