using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmLib
{
    public class AlgorithmState
    {
        public int N, K, R, _populationSize, _survivingPopulation, _epoch;
        public double _mutationChance;
        public Solution[] _population;

        public AlgorithmState(int n, int k, int r,
        int populationSize, int survivingPopulation, int _epoch, double MutationChance, Solution[] ?population = null)
        {
            N = n; K = k; R = r;
            _populationSize = populationSize; _survivingPopulation = survivingPopulation; this._epoch = _epoch;
            _mutationChance = MutationChance;
            _population = new Solution[populationSize];
            if(population != null)
            {
                for (int i = 0; i < populationSize; i++) _population[i] = (Solution)population[i].Clone();
            }
            else
            {
                for (int i = 0; i < populationSize; i++)
                {
                    _population[i] = new Solution(N, K, R);
                    _population[i].Init();
                }
            }
        }

        public Solution Best()
        {
            return _population.Max();
        }

        public string GetString()
        {
            string str = $"Algorithm:  N: {N}, K: {K}, R: {R};   Round{_epoch}\n";
            for (int i = 0; i < _populationSize; ++i)
            {
                str += $"({_population[i].MetricOpponents} {_population[i].MetricLocations}),  ";
            }
            return str;
        }
    }

    public class GeneticAlgorithm
    {
        public readonly int N, K, R; //1 <= R(rounds) < N(participants) <= K(locations)

        int _populationSize = 420, _survivingPopulation, _tournamentGroupSize = 2, _epoch = 0;
        double _mutationChance;
        Solution[] _population;

        public GeneticAlgorithm(int n, int k, int r,
            int populationSize, double survivingPopulationPart = 0.5, double mutationChance = 0.05)
        {
            N = n;
            K = k;
            R = r;
            _populationSize = populationSize;
            _mutationChance = mutationChance;


            _population = new Solution[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                _population[i] = new Solution(N, K, R);
                _population[i].Init();
            }
        }

        public GeneticAlgorithm(AlgorithmState algorithmState)
        {
            N = algorithmState.N;
            K = algorithmState.K;
            R = algorithmState.R;
            _epoch = algorithmState._epoch;
            _populationSize = algorithmState._populationSize;
            _mutationChance = algorithmState._mutationChance;


            _population = new Solution[_populationSize];
            for (int i = 0; i < _populationSize; i++)
            {
                _population[i] = algorithmState._population[i];
            }
        }

        public AlgorithmState IterateSolutions()
        {
            if (_epoch == 0)
            {
                ++_epoch;
                return new AlgorithmState(N, K, R, _populationSize, _survivingPopulation, _epoch, _mutationChance, _population);
            }
            Random RNG = new();
            int i;

            Solution[] newPopulation = new Solution[_populationSize], populationCopy = new Solution[_populationSize];
            for (i = 0; i < _populationSize; i++)
                populationCopy[i] = _population[i];
            //crossover
            RNG.Shuffle<Solution>(populationCopy);
            Parallel.For(0, Math.Min(_populationSize - _survivingPopulation, _populationSize / 2), i =>
            {
                newPopulation[_survivingPopulation + i] = Solution.Crossover(populationCopy[i * 2], populationCopy[i * 2 + 1], _mutationChance);
            });
            if (_survivingPopulation < _populationSize / 2)
            {
                RNG.Shuffle<Solution>(populationCopy);
                Parallel.For(0, _populationSize / 2 - _survivingPopulation,  i =>
                {
                    newPopulation[_survivingPopulation + i + _populationSize / 2] = Solution.Crossover(populationCopy[i * 2], populationCopy[i * 2 + 1], _mutationChance);
                });
            }

            //(un)natural selection
            RNG.Shuffle<Solution>(populationCopy);
            if (_survivingPopulation >= _populationSize / 2)
            {
                var populationCopy2 = new Solution[_populationSize];
                Parallel.For(0, _populationSize / 2, i =>
                {
                    if (populationCopy[i * 2].CompareTo(populationCopy[i * 2 + 1]) < 0)
                    {
                        newPopulation[i] = populationCopy[i * 2 + 1];
                        populationCopy2[i] = populationCopy[i * 2];
                    }
                    else
                    {
                        newPopulation[i] = populationCopy[i * 2];
                        populationCopy2[i] = populationCopy[i * 2 + 1];
                    }
                });
                Parallel.For(0, _survivingPopulation - _populationSize / 2, i =>
                {
                    if (populationCopy2[i * 2].CompareTo(populationCopy2[i * 2 + 1]) < 0)
                        newPopulation[i + _populationSize / 2] = populationCopy2[i * 2 + 1];
                    else
                        newPopulation[i + _populationSize / 2] = populationCopy2[i * 2];
                });
            }
            else
            {
                Parallel.For(0, _survivingPopulation, i =>
                {
                    if (populationCopy[i * 2].CompareTo(populationCopy[i * 2 + 1]) < 0)
                        newPopulation[i] = populationCopy[i * 2 + 1];
                    else
                        newPopulation[i] = populationCopy[i * 2];
                });
            }

            _population = newPopulation;
            var state = new AlgorithmState(N, K, R, _populationSize, _survivingPopulation, _epoch, _mutationChance, newPopulation);
            ++_epoch;
            return state;
        }
    }
}
