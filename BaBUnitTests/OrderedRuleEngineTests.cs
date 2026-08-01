using BloodAndBittersteel.Library.RuleEngine;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class OrderedRuleEngineTests
    {
        [TestMethod]
        public void Get_SingleMatchingRule_ReturnsResolvedValue()
        {
            var rule = new Rule<int, string>(matches: c => c > 0, resolve: c => "matched");
            var engine = new OrderedRuleEngine<int, string>([rule]);

            var result = engine.Get(5);

            Assert.AreEqual("matched", result);
        }

        [TestMethod]
        public void Get_SingleNonMatchingRule_Throws()
        {
            var rule = new Rule<int, string>(matches: c => false, resolve: c => "unused");
            var engine = new OrderedRuleEngine<int, string>([rule]);

            Assert.ThrowsExactly<InvalidOperationException>(() => engine.Get(5));
        }

        [TestMethod]
        public void Get_MultipleRules_FirstMatchingWins()
        {
            var first = new Rule<int, string>(matches: c => true, resolve: c => "first");
            var second = new Rule<int, string>(matches: c => true, resolve: c => "second");
            var third = new Rule<int, string>(matches: c => true, resolve: c => "third");
            var engine = new OrderedRuleEngine<int, string>([first, second, third]);

            var result = engine.Get(1);

            Assert.AreEqual("first", result);
        }

        [TestMethod]
        public void Get_MultipleRules_SkipsNonMatchingUntilMatch()
        {
            var first = new Rule<int, string>(matches: c => false, resolve: c => "first");
            var second = new Rule<int, string>(matches: c => true, resolve: c => "second");
            var engine = new OrderedRuleEngine<int, string>([first, second]);

            var result = engine.Get(1);

            Assert.AreEqual("second", result);
        }

        [TestMethod]
        public void Get_MultipleRules_DifferentContexts_SelectDifferentRules()
        {
            var forOne = new Rule<int, string>(matches: c => c == 1, resolve: c => "one");
            var forTwo = new Rule<int, string>(matches: c => c == 2, resolve: c => "two");
            var engine = new OrderedRuleEngine<int, string>([forOne, forTwo]);

            Assert.AreEqual("one", engine.Get(1));
            Assert.AreEqual("two", engine.Get(2));
        }

        [TestMethod]
        public void Get_MultipleRules_AllNonMatching_Throws()
        {
            var first = new Rule<int, string>(matches: c => false, resolve: c => "first");
            var second = new Rule<int, string>(matches: c => false, resolve: c => "second");
            var engine = new OrderedRuleEngine<int, string>([first, second]);

            Assert.ThrowsExactly<InvalidOperationException>(() => engine.Get(1));
        }

        [TestMethod]
        public void Get_MultipleRules_StopsAfterFirstMatch()
        {
            int matchesEvaluated = 0;
            var first = new Rule<int, string>(matches: c => true, resolve: c => "first");
            var second = new Rule<int, string>(matches: c => { matchesEvaluated++; return true; }, resolve: c => "second");
            var engine = new OrderedRuleEngine<int, string>([first, second]);

            engine.Get(1);

            Assert.AreEqual(0, matchesEvaluated);
        }

        [TestMethod]
        public void Get_EmptyRules_Throws()
        {
            var engine = new OrderedRuleEngine<int, string>([]);

            Assert.ThrowsExactly<InvalidOperationException>(() => engine.Get(1));
        }

        [TestMethod]
        public void Constructor_NullRules_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new OrderedRuleEngine<int, string>(null!));
        }

        [TestMethod]
        public void Get_PassesContextToResolve()
        {
            var rule = new Rule<int, string>(matches: c => true, resolve: c => c.ToString());
            var engine = new OrderedRuleEngine<int, string>([rule]);

            var result = engine.Get(42);

            Assert.AreEqual("42", result);
        }
    }
}
