using System.Collections.Generic;

namespace StaffRandomSelect.Domain
{
    internal class StrategyPersistenceDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<StrategyRuleDto> Rules { get; set; }
    }

    internal class StrategyRuleDto
    {
        public string Kind { get; set; }
        public int Star { get; set; }
        public string Career { get; set; }
        public int Count { get; set; }
        public int CountMax { get; set; }
        public List<string> StaffNames { get; set; }
    }
}
