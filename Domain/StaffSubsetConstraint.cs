using System.Collections.Generic;

namespace StaffRandomSelect.Domain
{
    /// <summary>「限制特定干员人数」合并后用于组队的约束。</summary>
    public sealed class StaffSubsetConstraint
    {
        public HashSet<string> Names { get; set; }
        public bool IsExact { get; set; }
        /// <summary>固定值模式下为必须出现的人数；范围模式下为下限。</summary>
        public int ExactOrLo { get; set; }
        /// <summary>仅范围模式使用。</summary>
        public int Hi { get; set; }
    }
}
