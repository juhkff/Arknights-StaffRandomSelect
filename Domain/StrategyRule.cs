using System.Collections.Generic;
using System.Linq;

namespace StaffRandomSelect.Domain
{
    public class StrategyRule
    {
        public StrategyRuleKind Kind { get; set; }

        /// <summary>1–6，仅当 <see cref="Kind"/> 为 <see cref="StrategyRuleKind.Rarity"/> 时有效。</summary>
        public int Star { get; set; }

        public Career Career { get; set; }

        public int Count { get; set; }

        /// <summary>区间上界，仅 <see cref="StrategyRuleKind.CareerRange"/> 与 <see cref="StrategyRuleKind.StaffSubsetRange"/> 使用。</summary>
        public int CountMax { get; set; }

        /// <summary>干员名列表，仅 <see cref="StrategyRuleKind.StaffSubsetExact"/> / <see cref="StrategyRuleKind.StaffSubsetRange"/> 使用。</summary>
        public List<string> StaffNames { get; set; } = new List<string>();

        public string SummaryLine =>
            Kind switch
            {
                StrategyRuleKind.Rarity => $"固定特定稀有度总量：{Star} 星 × {Count}",
                StrategyRuleKind.CareerRange => $"固定特定职业数量范围：{Career} {Count}–{CountMax}",
                StrategyRuleKind.Career => $"固定特定职业数量：{Career} × {Count}",
                StrategyRuleKind.StaffSubsetExact => $"限制特定干员人数：从 {FormatStaffNames()} 中固定 {Count} 个",
                StrategyRuleKind.StaffSubsetRange => $"限制特定干员人数：从 {FormatStaffNames()} 中范围 {Count}–{CountMax} 个",
                _ => ""
            };

        private string FormatStaffNames()
        {
            if (StaffNames == null || StaffNames.Count == 0)
                return "（未指定）";
            var head = string.Join("、", StaffNames.Take(4));
            return StaffNames.Count > 4 ? head + "…" : head;
        }
    }
}
