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
                    Kind = r.Kind switch
                    {
                        StrategyRuleKind.Rarity => "Rarity",
                        StrategyRuleKind.CareerRange => "CareerRange",
                        StrategyRuleKind.Career => "Career",
                        StrategyRuleKind.StaffSubsetExact => "StaffSubsetExact",
                        StrategyRuleKind.StaffSubsetRange => "StaffSubsetRange",
                        _ => "Career"
                    },
                    Star = r.Star,
                    Career = (r.Kind == StrategyRuleKind.Career || r.Kind == StrategyRuleKind.CareerRange)
                        ? r.Career.ToString()
                        : null,
                    Count = r.Count,
                    CountMax = (r.Kind == StrategyRuleKind.CareerRange || r.Kind == StrategyRuleKind.StaffSubsetRange)
                        ? r.CountMax
                        : 0,
                    StaffNames = (r.Kind == StrategyRuleKind.StaffSubsetExact || r.Kind == StrategyRuleKind.StaffSubsetRange)
                        ? (r.StaffNames != null ? new List<string>(r.StaffNames) : null)
                        : null
                }).ToList()
            };

        private static StrategyRule FromDto(StrategyRuleDto r)
        {
            if (r == null)
                return null;

            if (string.Equals(r.Kind, "CareerRange", StringComparison.OrdinalIgnoreCase))
            {
                if (!Enum.TryParse(r.Career, out Career career))
                    return null;
                int lo = r.Count;
                int hi = r.CountMax;
                if (lo > hi || lo < 0)
                    return null;
                return new StrategyRule
                {
                    Kind = StrategyRuleKind.CareerRange,
                    Career = career,
                    Count = lo,
                    CountMax = hi
                };
            }

            if (string.Equals(r.Kind, "StaffSubsetExact", StringComparison.OrdinalIgnoreCase))
            {
                var names = NormalizeStaffNamesDto(r.StaffNames);
                if (names.Count == 0 || r.Count < 0)
                    return null;
                return new StrategyRule
                {
                    Kind = StrategyRuleKind.StaffSubsetExact,
                    StaffNames = names,
                    Count = r.Count
                };
            }

            if (string.Equals(r.Kind, "StaffSubsetRange", StringComparison.OrdinalIgnoreCase))
            {
                var names = NormalizeStaffNamesDto(r.StaffNames);
                int lo = r.Count;
                int hi = r.CountMax;
                if (names.Count == 0 || lo > hi || lo < 0)
                    return null;
                return new StrategyRule
                {
                    Kind = StrategyRuleKind.StaffSubsetRange,
                    StaffNames = names,
                    Count = lo,
                    CountMax = hi
                };
            }

            if (r.Count <= 0)
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

        private static List<string> NormalizeStaffNamesDto(List<string> raw)
        {
            var list = new List<string>();
            if (raw == null)
                return list;
            foreach (var n in raw)
            {
                if (string.IsNullOrWhiteSpace(n))
                    continue;
                var t = n.Trim();
                if (!list.Contains(t))
                    list.Add(t);
            }

            return list;
        }
    }
}
