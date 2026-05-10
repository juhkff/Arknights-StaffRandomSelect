namespace StaffRandomSelect.Domain
{
    public class StrategyRule
    {
        public StrategyRuleKind Kind { get; set; }

        /// <summary>1–6，仅当 <see cref="Kind"/> 为 <see cref="StrategyRuleKind.Rarity"/> 时有效。</summary>
        public int Star { get; set; }

        public Career Career { get; set; }

        public int Count { get; set; }

        public string SummaryLine =>
            Kind == StrategyRuleKind.Rarity
                ? $"固定总体稀有度数量：{Star} 星 × {Count}"
                : $"固定特定职业数量：{Career} × {Count}";
    }
}
