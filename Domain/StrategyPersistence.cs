using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace StaffRandomSelect.Domain
{
    internal static class StrategyPersistence
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static void Load(string path, ObservableCollection<RandomStrategyDefinition> target)
        {
            target.Clear();
            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path);
            var list = JsonSerializer.Deserialize<List<StrategyPersistenceDto>>(json, JsonOptions);
            if (list == null)
                return;

            foreach (var dto in list)
            {
                var def = new RandomStrategyDefinition
                {
                    Id = string.IsNullOrEmpty(dto.Id) ? Guid.NewGuid().ToString() : dto.Id,
                    Name = dto.Name ?? ""
                };
                if (dto.Rules != null)
                {
                    foreach (var r in dto.Rules)
                    {
                        var rule = FromDto(r);
                        if (rule != null)
                            def.Rules.Add(rule);
                    }
                }
                target.Add(def);
            }
        }

        public static void Save(string path, IEnumerable<RandomStrategyDefinition> strategies)
        {
            var list = strategies.Select(ToDto).ToList();
            var json = JsonSerializer.Serialize(list, JsonOptions);
            File.WriteAllText(path, json);
        }

        private static StrategyPersistenceDto ToDto(RandomStrategyDefinition d) =>
            new StrategyPersistenceDto
            {
                Id = d.Id,
                Name = d.Name,
                Rules = d.Rules.Select(r => new StrategyRuleDto
                {
                    Kind = r.Kind == StrategyRuleKind.Rarity ? "Rarity" : "Career",
                    Star = r.Star,
                    Career = r.Kind == StrategyRuleKind.Career ? r.Career.ToString() : null,
                    Count = r.Count
                }).ToList()
            };

        private static StrategyRule FromDto(StrategyRuleDto r)
        {
            if (r == null || r.Count <= 0)
                return null;

            if (string.Equals(r.Kind, "Rarity", StringComparison.OrdinalIgnoreCase))
            {
                if (r.Star < 1 || r.Star > 6)
                    return null;
                return new StrategyRule { Kind = StrategyRuleKind.Rarity, Star = r.Star, Count = r.Count };
            }

            if (string.Equals(r.Kind, "Career", StringComparison.OrdinalIgnoreCase))
            {
                if (!Enum.TryParse(r.Career, out Career career))
                    return null;
                return new StrategyRule { Kind = StrategyRuleKind.Career, Career = career, Count = r.Count };
            }

            return null;
        }
    }
}
