using System;

namespace BloodAndBittersteel.Library.RuleEngine
{
    public class Rule<TContext, TResult>
    {
        public Rule(Func<TContext, bool> condition, Func<TContext, TResult> valueFactory)
        {
            Matches = condition;
            Resolve = valueFactory;
        }

        public Func<TContext, bool> Matches { get; init; }
        public Func<TContext, TResult> Resolve { get; init; }
    }
}
