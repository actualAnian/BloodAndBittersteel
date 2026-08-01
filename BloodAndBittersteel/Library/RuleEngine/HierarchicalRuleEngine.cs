using System;
using System.Collections.Generic;
using System.Linq;

namespace BloodAndBittersteel.Library.RuleEngine
{
    public class HierarchicalRuleEngine<TContext, TResult>
    {
        private readonly List<Rule<TContext, TResult>> _rules;
        public HierarchicalRuleEngine(IEnumerable<Rule<TContext, TResult>> rules)
        {
            _rules = rules.ToList();
        }
        public TResult Get(TContext context)
        {
            foreach (var rule in _rules)
            {
                if (rule.Matches(context))
                    return rule.Resolve(context);
            }
            throw new InvalidOperationException("No matching rule.");
        }
    }
}
