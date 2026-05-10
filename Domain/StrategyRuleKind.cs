namespace StaffRandomSelect.Domain
{
    public enum StrategyRuleKind
    {
        Rarity,
        Career,
        /// <summary>指定某职业人数落在 [<see cref="StrategyRule.Count"/>, <see cref="StrategyRule.CountMax"/>] 闭区间内。</summary>
        CareerRange,

        /// <summary>指定干员集合内恰有 <see cref="StrategyRule.Count"/> 人（0 表示集合内无人）。</summary>
        StaffSubsetExact,

        /// <summary>指定干员集合内人数落在 [<see cref="StrategyRule.Count"/>, <see cref="StrategyRule.CountMax"/>]。</summary>
        StaffSubsetRange
    }
}

