using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmLib
{
    public class Solution : IComparable, ICloneable
    {
        public int N, K, R; //1 <= R(rounds) < N(participants) <= K(locations)
        public int MetricOpponents, MetricLocations;
        public int[,] SolutionMatrix;
        public Solution(int n, int k, int r)
        {
            N = n;
            K = k;
            R = r;
            SolutionMatrix = new int[R, N];
        }

        public void Init()
        {
            // N/2 - используемые площадки
            Random RNG = new Random();
            int[] randLocations = new int[K];
            int[] selectedLocations = new int[N];
            for (int i = 0; i < K; ++i) randLocations[i] = i;

            for (int i = 0; i < R; ++i)
            {
                RandomExtensions.Shuffle(RNG, randLocations);

                for (int j = 0; j < N - 1; j += 2)
                {
                    selectedLocations[j] = selectedLocations[j + 1] = randLocations[j];
                }
                if (N % 2 == 1) selectedLocations[N - 1] = -1;

                RandomExtensions.Shuffle(RNG, selectedLocations);

                for (int j = 0; j < N; ++j) SolutionMatrix[i, j] = selectedLocations[j];
            }
            CalculateMetrics();
        }

        private void CalculateMetrics()
        {
            CalculateMetricLocations();
            CalculateMetricOpponents();
        }

        private void CalculateMetricOpponents()
        {
            bool[,] OpponentsTracker = new bool[N, N];
            int[,] LocationTracker = new int[K, R];
            int metric = N;

            for (int i = 0; i < K; ++i)
                for (int j = 0; j < R; ++j)
                    LocationTracker[i, j] = -1;

            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < R; ++j)
                {
                    if (LocationTracker[SolutionMatrix[j, i], j] != -1)
                    {
                        OpponentsTracker[LocationTracker[SolutionMatrix[j, i], j], i] = true;
                        OpponentsTracker[i, LocationTracker[SolutionMatrix[j, i], j]] = true;
                    }
                    else
                    {
                        LocationTracker[SolutionMatrix[j, i], j] = i;
                    }
                }
            }
            for (int i = 0; i < N; ++i)
            {
                int counter = 0;
                for (int j = 0; j < N; ++j)
                {
                    if (OpponentsTracker[i, j]) ++counter;
                }
                metric = Math.Min(metric, counter);
            }
            MetricOpponents = metric;
        }

        private void CalculateMetricLocations()
        {
            int metric = N;
            HashSet<int> locations = new();
            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < R; ++j)
                {
                    locations.Add(SolutionMatrix[j, i]);
                }
                metric = Math.Min(metric, locations.Count);
                locations.Clear();
            }
            MetricLocations = metric;
        }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            Solution otherSolution = obj as Solution;
            if (otherSolution != null)
                return (this.MetricOpponents != otherSolution.MetricOpponents) ?
                    this.MetricOpponents.CompareTo(otherSolution.MetricOpponents) :
                    this.MetricLocations.CompareTo(otherSolution.MetricLocations);
            else
                throw new ArgumentException("Object is not a Solution");
        }

        public object Clone()
        {
            Solution clone = new(N, K, R);
            for (int i = 0; i < R; ++i)
                for (int j = 0; j < N; ++j)
                    clone.SolutionMatrix[i, j] = SolutionMatrix[i, j];
            clone.MetricLocations = MetricLocations;
            clone.MetricOpponents = MetricOpponents;
            return clone;
        }

        private void Mutate(double geneMutationChance)
        {
            Random RNG = new();
            for (int i = 0; i < R; ++i)
            {
                if (!RandomExtensions.RandomChance(RNG, geneMutationChance)) continue;

                int newLocation = RNG.Next(K), oldLocation = SolutionMatrix[i, RNG.Next(N)];
                if (oldLocation == -1) continue;

                for (int j = 0; j < N; ++j)
                {
                    if (SolutionMatrix[i, j] == oldLocation) SolutionMatrix[i, j] = newLocation;
                    else if (SolutionMatrix[i, j] == newLocation) SolutionMatrix[i, j] = oldLocation;
                }
            }
        }

        public static Solution Crossover(Solution solution1, Solution solution2, double geneMutationChance = 0)
        {
            Random RNG = new();
            int n = solution1.N, k = solution1.K, r = solution1.R;
            Solution res = new Solution(n, k, r);
            for (int i = 0; i < r; ++i)
            {
                if (RandomExtensions.RandomChance(RNG, 0.5))
                {
                    for (int j = 0; j < n; ++j)
                        res.SolutionMatrix[i, j] = solution1.SolutionMatrix[i, j];
                }
                else
                {
                    for (int j = 0; j < n; ++j)
                        res.SolutionMatrix[i, j] = solution2.SolutionMatrix[i, j];
                }
            }
            res.Mutate(geneMutationChance);
            res.CalculateMetrics();
            return res;
        }

        public string GetInfoStr()
        {
            string info = "    ";
            for (int j = 0; j < N; ++j) info += $"{j,-4}";
            for (int i = 0; i < R; ++i)
            {
                info += $"\nR{i,-3}";
                for (int j = 0; j < N; ++j)
                {
                    info += $"{SolutionMatrix[i, j],-4}";
                }
            }
            info += $"\nMetrics: Locations {MetricLocations}, Opponents {MetricOpponents}";
            return info;
        }
    }
}
