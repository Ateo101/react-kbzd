using CrimeEmpire.Economy;
using NUnit.Framework;

namespace CrimeEmpire.Tests
{
    public sealed class EconomyTests
    {
        [Test]
        public void Add_RespectsCap()
        {
            var wallet = new Wallet();
            wallet.SetCap(CurrencyType.Contraband, 500);

            Assert.AreEqual(450, wallet.Add(CurrencyType.Contraband, 450));
            Assert.AreEqual(50, wallet.Add(CurrencyType.Contraband, 100));
            Assert.AreEqual(500, wallet.Get(CurrencyType.Contraband));
        }

        [Test]
        public void LoweringCap_ClampsAmount()
        {
            var wallet = new Wallet();
            wallet.Add(CurrencyType.Contraband, 300);
            wallet.SetCap(CurrencyType.Contraband, 200);

            Assert.AreEqual(200, wallet.Get(CurrencyType.Contraband));
        }

        [Test]
        public void TrySpend_FailsWithoutFunds_AndKeepsBalance()
        {
            var wallet = new Wallet();
            wallet.Add(CurrencyType.Coins, 100);

            Assert.IsFalse(wallet.TrySpend(CurrencyType.Coins, 101));
            Assert.AreEqual(100, wallet.Get(CurrencyType.Coins));
            Assert.IsTrue(wallet.TrySpend(CurrencyType.Coins, 100));
            Assert.AreEqual(0, wallet.Get(CurrencyType.Coins));
        }

        [Test]
        public void GradualPayment_FillsInConfiguredTime()
        {
            var wallet = new Wallet();
            wallet.Add(CurrencyType.Coins, 10_000);
            var payment = new GradualPayment(wallet, CurrencyType.Coins, 5000, 0, 2f);

            for (int i = 0; i < 110; i++) payment.Tick(0.02f); // a bit over 2 seconds; float steps round down

            Assert.IsTrue(payment.IsComplete);
            Assert.AreEqual(5000, payment.Paid);
            Assert.AreEqual(5000, wallet.Get(CurrencyType.Coins));
        }

        [Test]
        public void GradualPayment_StopsWhenBroke_AndResumesFromSavedProgress()
        {
            var wallet = new Wallet();
            wallet.Add(CurrencyType.Coins, 600);
            var payment = new GradualPayment(wallet, CurrencyType.Coins, 5000, 4000, 2f);

            for (int i = 0; i < 100; i++) payment.Tick(0.02f);

            Assert.AreEqual(4600, payment.Paid);
            Assert.IsFalse(payment.IsComplete);
            Assert.AreEqual(0, wallet.Get(CurrencyType.Coins));

            // Money comes back: no burst from the time spent broke.
            wallet.Add(CurrencyType.Coins, 1000);
            Assert.LessOrEqual(payment.Tick(0.02f), 50);
        }

        [TestCase(199, "199")]
        [TestCase(1070, "1070")]
        [TestCase(15_500, "15.5k")]
        [TestCase(15_300, "15.3k")]
        [TestCase(15_599, "15.5k")]
        [TestCase(123_500, "123.5k")]
        [TestCase(2_000_000, "2M")]
        public void ShortFormat_MatchesHud(long value, string expected)
        {
            Assert.AreEqual(expected, NumberFormat.Short(value));
        }
    }
}
