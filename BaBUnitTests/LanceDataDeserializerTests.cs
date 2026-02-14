using LanceSystem.Deserialization;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class LanceDataDeserializerTests
    {
        [TestMethod]
        public void Normalize_AllZeroExceptLast_BecomesOneForLast()
        {
            var t0 = new TroopData(LanceTroopCategory.Infantry, 0.0, string.Empty);
            var t1 = new TroopData(LanceTroopCategory.Ranged, 0.0, string.Empty);
            var t2 = new TroopData(LanceTroopCategory.Cavalry, 0.0, string.Empty);
            var t3 = new TroopData(LanceTroopCategory.HorseArcher, 1000.0, string.Empty);

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods([t0, t1, t2, t3]);

            Assert.AreEqual(0.0, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[2].Likelihood, 1e-9);
            Assert.AreEqual(1.0, res[3].Likelihood, 1e-9);
        }

        [TestMethod]
        public void Normalize_NegativesTreatedAsZero_BecomesOneForLast()
        {
            var t0 = new TroopData(LanceTroopCategory.Infantry, -1.0, string.Empty);
            var t1 = new TroopData(LanceTroopCategory.Ranged, -5.0, string.Empty);
            var t2 = new TroopData(LanceTroopCategory.Cavalry, 0.0, string.Empty);
            var t3 = new TroopData(LanceTroopCategory.HorseArcher, 1.0, string.Empty);

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods([t0, t1, t2, t3]);

            Assert.AreEqual(0.0, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.0, res[2].Likelihood, 1e-9);
            Assert.AreEqual(1.0, res[3].Likelihood, 1e-9);
        }

        [TestMethod]
        public void Normalize_PositiveValues_ProportionalDistribution()
        {
            var t0 = new TroopData(LanceTroopCategory.Infantry, 5.0, string.Empty);
            var t1 = new TroopData(LanceTroopCategory.Ranged, 10.0, string.Empty);
            var t2 = new TroopData(LanceTroopCategory.Cavalry, 5.0, string.Empty);
            var t3 = new TroopData(LanceTroopCategory.HorseArcher, 30.0, string.Empty);
            var res = LanceDataDeserializer.NormalizeTroopLikelihoods([t0, t1, t2, t3]);

            Assert.AreEqual(0.1, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.2, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.1, res[2].Likelihood, 1e-9);
            Assert.AreEqual(0.6, res[3].Likelihood, 1e-9);
        }

        [TestMethod]
        public void Normalize_ZeroSum_WithValidIds_DistributesEquallyAmongValid()
        {
            var t0 = new TroopData(LanceTroopCategory.Infantry, 0.0, string.Empty);
            var t1 = new TroopData(LanceTroopCategory.Ranged, 0.0, string.Empty);
            var t2 = new TroopData(LanceTroopCategory.Cavalry, 0.0, string.Empty);
            var t3 = new TroopData(LanceTroopCategory.HorseArcher, 0.0, string.Empty);

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods([t0, t1, t2, t3]);

            Assert.AreEqual(0.25, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.25, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.25, res[2].Likelihood, 1e-9);
            Assert.AreEqual(0.25, res[3].Likelihood, 1e-9);
        }

        [TestMethod]
        public void Normalize_PositiveSum_WithNegativeNumbers()
        {
            var t0 = new TroopData(LanceTroopCategory.Infantry, 5.0, string.Empty);
            var t1 = new TroopData(LanceTroopCategory.Ranged, -1.0, string.Empty);
            var t2 = new TroopData(LanceTroopCategory.Cavalry, -1.0, string.Empty);
            var t3 = new TroopData(LanceTroopCategory.HorseArcher, -1.0, string.Empty);

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods([t0, t1, t2, t3]);

            Assert.AreEqual(1, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0, res[2].Likelihood, 1e-9);
            Assert.AreEqual(0, res[3].Likelihood, 1e-9);
        }

        [TestMethod]
        public void Normalize_PositiveSum_WithNegativesAndZeros()
        {
            var t0 = new TroopData(LanceTroopCategory.Infantry, -1.0, string.Empty);
            var t1 = new TroopData(LanceTroopCategory.Ranged, -2.0, string.Empty);
            var t2 = new TroopData(LanceTroopCategory.Cavalry, 0.0, string.Empty);
            var t3 = new TroopData(LanceTroopCategory.HorseArcher, 0.0, string.Empty);

            var res = LanceDataDeserializer.NormalizeTroopLikelihoods([t0, t1, t2, t3]);
            Assert.AreEqual(0.25, res[0].Likelihood, 1e-9);
            Assert.AreEqual(0.25, res[1].Likelihood, 1e-9);
            Assert.AreEqual(0.25, res[2].Likelihood, 1e-9);
            Assert.AreEqual(0.25, res[3].Likelihood, 1e-9);
        }
    }
}
