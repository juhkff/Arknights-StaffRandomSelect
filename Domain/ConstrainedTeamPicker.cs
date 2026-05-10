using System;
using System.Collections.Generic;
using System.Linq;

namespace StaffRandomSelect.Domain
{
    /// <summary>
    /// 在满足「指定稀有度人数」「指定职业人数」精确计数的前提下，从候选池无放回随机组队。
    /// </summary>
    public static class ConstrainedTeamPicker
    {
        private const int MaxAttempts = 8000;

        public static bool TryPick(
            IReadOnlyList<Staff> pool,
            int k,
            Dictionary<int, int> rarityReq,
            Dictionary<Career, int> careerReq,
            Random random,
            out List<Staff> team)
        {
            team = null;
            if (pool == null || pool.Count == 0 || k <= 0 || k > pool.Count)
                return false;

            if (rarityReq == null || rarityReq.Count == 0)
                rarityReq = new Dictionary<int, int>();
            if (careerReq == null || careerReq.Count == 0)
                careerReq = new Dictionary<Career, int>();

            foreach (var kv in rarityReq)
            {
                if (kv.Value < 0 || kv.Key < 1 || kv.Key > 6)
                    return false;
            }

            foreach (var kv in careerReq)
            {
                if (kv.Value < 0)
                    return false;
            }

            if (rarityReq.Values.Sum() > k || careerReq.Values.Sum() > k)
                return false;

            if (!PoolHasCapacity(pool, rarityReq, careerReq))
                return false;

            var starCap = new int[7];
            for (int s = 1; s <= 6; s++)
            {
                if (rarityReq.TryGetValue(s, out int need))
                    starCap[s] = need;
                else
                    starCap[s] = k;
            }

            var order = Enumerable.Range(0, pool.Count).ToList();
            var picked = new List<Staff>(k);
            var used = new bool[pool.Count];
            var starCnt = new int[7];
            var careerCnt = new Dictionary<Career, int>();
            foreach (Career c in Enum.GetValues(typeof(Career)))
                careerCnt[c] = 0;

            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                Shuffle(order, random);
                picked.Clear();
                Array.Clear(starCnt, 0, starCnt.Length);
                foreach (Career c in Enum.GetValues(typeof(Career)))
                    careerCnt[c] = 0;
                Array.Fill(used, false);

                if (Dfs(pool, order, k, rarityReq, careerReq, starCap, picked, used, starCnt, careerCnt))
                {
                    team = picked.ToList();
                    return true;
                }
            }

            return false;
        }

        private static bool PoolHasCapacity(
            IReadOnlyList<Staff> pool,
            Dictionary<int, int> rarityReq,
            Dictionary<Career, int> careerReq)
        {
            foreach (var kv in rarityReq)
            {
                int have = pool.Count(s => s.Star == kv.Key);
                if (have < kv.Value)
                    return false;
            }

            foreach (var kv in careerReq)
            {
                int have = pool.Count(s => s.Career == kv.Key);
                if (have < kv.Value)
                    return false;
            }

            return true;
        }

        private static bool Dfs(
            IReadOnlyList<Staff> pool,
            List<int> order,
            int k,
            Dictionary<int, int> rarityReq,
            Dictionary<Career, int> careerReq,
            int[] starCap,
            List<Staff> picked,
            bool[] used,
            int[] starCnt,
            Dictionary<Career, int> careerCnt)
        {
            if (picked.Count == k)
                return FinalOk(starCnt, careerCnt, k, rarityReq, careerReq);

            int remaining = k - picked.Count;
            if (!PartialOk(starCnt, careerCnt, remaining, rarityReq, careerReq))
                return false;

            foreach (int idx in order)
            {
                if (used[idx])
                    continue;

                var s = pool[idx];
                int ns = starCnt[s.Star] + 1;
                if (ns > starCap[s.Star])
                    continue;

                int nc = careerCnt[s.Career] + 1;
                if (careerReq.TryGetValue(s.Career, out int cneed) && nc > cneed)
                    continue;

                used[idx] = true;
                picked.Add(s);
                starCnt[s.Star]++;
                careerCnt[s.Career]++;

                if (Dfs(pool, order, k, rarityReq, careerReq, starCap, picked, used, starCnt, careerCnt))
                    return true;

                careerCnt[s.Career]--;
                starCnt[s.Star]--;
                picked.RemoveAt(picked.Count - 1);
                used[idx] = false;
            }

            return false;
        }

        private static bool PartialOk(
            int[] starCnt,
            Dictionary<Career, int> careerCnt,
            int remaining,
            Dictionary<int, int> rarityReq,
            Dictionary<Career, int> careerReq)
        {
            foreach (var kv in rarityReq)
            {
                int have = starCnt[kv.Key];
                if (have > kv.Value)
                    return false;
                if (kv.Value - have > remaining)
                    return false;
            }

            foreach (var kv in careerReq)
            {
                int have = careerCnt[kv.Key];
                if (have > kv.Value)
                    return false;
                if (kv.Value - have > remaining)
                    return false;
            }

            return true;
        }

        private static bool FinalOk(
            int[] starCnt,
            Dictionary<Career, int> careerCnt,
            int k,
            Dictionary<int, int> rarityReq,
            Dictionary<Career, int> careerReq)
        {
            int sum = 0;
            for (int i = 1; i <= 6; i++)
                sum += starCnt[i];
            if (sum != k)
                return false;

            foreach (var kv in rarityReq)
            {
                if (starCnt[kv.Key] != kv.Value)
                    return false;
            }

            foreach (var kv in careerReq)
            {
                if (careerCnt[kv.Key] != kv.Value)
                    return false;
            }

            return true;
        }

        private static void Shuffle(List<int> order, Random rng)
        {
            for (int i = order.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (order[i], order[j]) = (order[j], order[i]);
            }
        }

        public static void MergeRules(
            RandomStrategyDefinition def,
            out Dictionary<int, int> rarityReq,
            out Dictionary<Career, int> careerReq)
        {
            rarityReq = new Dictionary<int, int>();
            careerReq = new Dictionary<Career, int>();

            if (def?.Rules == null)
                return;

            foreach (var r in def.Rules)
            {
                if (r.Count <= 0)
                    continue;

                if (r.Kind == StrategyRuleKind.Rarity && r.Star >= 1 && r.Star <= 6)
                {
                    if (!rarityReq.ContainsKey(r.Star))
                        rarityReq[r.Star] = 0;
                    rarityReq[r.Star] += r.Count;
                }
                else if (r.Kind == StrategyRuleKind.Career)
                {
                    if (!careerReq.ContainsKey(r.Career))
                        careerReq[r.Career] = 0;
                    careerReq[r.Career] += r.Count;
                }
            }
        }
    }
}
