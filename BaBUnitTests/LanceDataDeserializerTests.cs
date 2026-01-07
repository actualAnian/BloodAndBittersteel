using System;
using BloodAndBittersteel.Features.LanceSystem.Deserialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class LanceDataDeserializerTests
    {
        [TestMethod]
        public void Normalize_AllZeroExceptLast_BecomesOneForLast()
        {
            var t0 = new TroopData(0.0, string.Empty);
            var t1 = new TroopData(0.0, string.Empty);
            var t2 = new TroopData(0.0, string.Empty);
            var t3 = new TroopData(1000.0, string.Empty);

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods(new[] { t0, t1, t2, t3 });

            Assert.AreEqual(0.0, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[2].Likelihood, 1e-9);
            Assert.AreEqual(1.0, res[3].Likelihood, 1e-9);
        }

        [TestMethod]
        public void Normalize_NegativesTreatedAsZero_BecomesOneForLast()
        {
            var t0 = new TroopData(-1.0, string.Empty);
            var t1 = new TroopData(-5.0, string.Empty);
            var t2 = new TroopData(0.0, string.Empty);
            var t3 = new TroopData(1.0, string.Empty);

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods(new[] { t0, t1, t2, t3 });

            Assert.AreEqual(0.0, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[2].Likelihood, 1e-9);
            Assert.AreEqual(1.0, res[3].Likelihood, 1e-9);
        }

        [TestMethod]
        public void Normalize_PositiveValues_ProportionalDistribution()
        {
            var t0 = new TroopData(5.0, "m");
            var t1 = new TroopData(10.0, "r");
            var t2 = new TroopData(5.0, "c");
            var t3 = new TroopData(30.0, "h");

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods(new[] { t0, t1, t2, t3 });

            Assert.AreEqual(0.1, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.2, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.1, res[2].Likelihood, 1e-9);
            Assert.AreEqual(0.6, res[3].Likelihood, 1e-9);
        }

        [TestMethod]
        public void Normalize_ZeroSum_WithValidIds_DistributesEquallyAmongValid()
        {
            var t0 = new TroopData(0.0, "a");
            var t1 = new TroopData(0.0, string.Empty);
            var t2 = new TroopData(0.0, "b");
            var t3 = new TroopData(0.0, string.Empty);

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods(new[] { t0, t1, t2, t3 });

            Assert.AreEqual(0.5, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.5, res[2].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[3].Likelihood, 1e-9);
        }

        [TestMethod]
        public void Normalize_AllZero_NoValidIds_ReturnsAllZeros()
        {
            var t0 = new TroopData(0.0, string.Empty);
            var t1 = new TroopData(0.0, string.Empty);
            var t2 = new TroopData(0.0, string.Empty);
            var t3 = new TroopData(0.0, string.Empty);

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods(new[] { t0, t1, t2, t3 });

            Assert.AreEqual(0.0, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[2].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[3].Likelihood, 1e-9);
        }

        [TestMethod]
        public void Normalize_NegativesAndZeros_WithValidIds_DistributesEqually()
        {
            var t0 = new TroopData(-1.0, "a");
            var t1 = new TroopData(-2.0, "b");
            var t2 = new TroopData(0.0, string.Empty);
            var t3 = new TroopData(0.0, string.Empty);

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods(new[] { t0, t1, t2, t3 });

            Assert.AreEqual(0.5, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.5, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[2].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[3].Likelihood, 1e-9);
        }
    }
}
