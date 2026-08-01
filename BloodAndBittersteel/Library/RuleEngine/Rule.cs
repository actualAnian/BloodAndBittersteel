using System;

namespace BloodAndBittersteel.Library.RuleEngine
{
    public class Rule<TContext, TResult>
    {
        public Rule(Func<TContext, bool> matches, Func<TContext, TResult> resolve)
        {
            Matches = matches;
            Resolve = resolve;
        }

        public Func<TContext, bool> Matches { get; init; }
        public Func<TContext, TResult> Resolve { get; init; }
    }
}
