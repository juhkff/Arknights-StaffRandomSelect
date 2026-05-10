using System;
using System.Collections.ObjectModel;

namespace StaffRandomSelect.Domain
{
    public class RandomStrategyDefinition
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = "";

        public ObservableCollection<StrategyRule> Rules { get; } = new ObservableCollection<StrategyRule>();
    }
}
