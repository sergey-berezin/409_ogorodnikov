using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibNamespace
{
    public class TournamentScheduler
    {
        public static Random rand = new Random();
        public static int R = 4;
        public static int N = 6;
        public static int K = 6;
        public static int PopulationSize = 7;
        public static int Generations = 7;
        public static double MutationRate = 0.1;
        public class Schedule
        {
            public int [,] Matrix = new int[R, N];
            public int Fitness { get; set; }

            public int UniqueOpponentsCount(int player)
            {
                HashSet<int> opponents = new HashSet<int>();
                for (int i = 0; i < R; ++i)
                {
                    int opponent = FindOpponentInRound(i, player);
                    if (opponent != -1)
                    {
                        opponents.Add(opponent);
                    }
                }    
                return opponents.Count;
            }
            public int UniqueLocationsCount(int player)
            {
                HashSet<int> locations = new HashSet<int>();
                for (int r = 0; r < R; ++r) 
                {
                    locations.Add(Matrix[r, player]);
                }
                return locations.Count;
            }
            private int FindOpponentInRound(int round, int player)
            {
                for (int n = 0; n < N; n++)
                {
                    if ((n != player) && (Matrix[round, n] == Matrix[round, player])) 
                        return n;
                }
                return -1;
            }
            public void CalculateFitness()
            {
                int minLocations = K;
                int minOpponents = N;
                for (int player = 0; player < N; player++)
                {
                    minLocations = Math.Min(minLocations, UniqueLocationsCount(player));
                    minOpponents = Math.Min(minOpponents, UniqueOpponentsCount(player));
                }
                Fitness = minOpponents * minOpponents + minLocations * minLocations;
            }
        }
        public static Schedule CreateRandomSchedule()
        {
            Schedule schedule= new Schedule();
            for (int r = 0; r < R; ++r)
            {
                int [] freeLocations = Enumerable.Range(1, K).ToArray();
                int [] unscheduled = Enumerable.Range(0, N).ToArray();
                while (unscheduled.Length > 1)
                {
                    int randomLocation = rand.Next(freeLocations.Length);
                    int randomPlayer = rand.Next(unscheduled.Length);
                    schedule.Matrix[r, unscheduled[randomPlayer]] = freeLocations[randomLocation];
                    unscheduled = unscheduled.Where((val, idx) => idx != randomPlayer).ToArray();
                    randomPlayer = rand.Next(unscheduled.Length);
                    schedule.Matrix[r, unscheduled[randomPlayer]] = freeLocations[randomLocation];
                    unscheduled = unscheduled.Where((val, idx) => idx != randomPlayer ).ToArray();
                    freeLocations = freeLocations.Where((val, idx) => idx != randomLocation).ToArray();
                }
                if (unscheduled.Length == 1)
                {
                    int randomLocation1 = rand.Next(freeLocations.Length);
                    int randomPlayer1 = unscheduled[0];
                    schedule.Matrix[r, randomPlayer1] = freeLocations[randomLocation1];
                }
            }
            schedule.CalculateFitness();
            return schedule;
        }
        static Schedule SelectParent(List<Schedule> population)
        {
            int tournamentSize = 5;
            List<Schedule> tournament = new List<Schedule>();
            for (int i = 0; i < tournamentSize; ++i)
            {
                tournament.Add(population[rand.Next(population.Count)]);
            }
            return tournament.OrderByDescending(s => s.Fitness).First();
        }
        static Schedule Crossover(Schedule schedule1, Schedule schedule2)
        {
            Schedule newSchedule = new Schedule();
            int crossPoint = rand.Next(1, R);
            for (int i = 0; i < R; ++i)
            {
                if (i < crossPoint)
                {
                    for (int n = 0; n < N; ++n)
                    {
                        newSchedule.Matrix[i, n] = schedule1.Matrix[i, n];
                    }
                }
                else
                {
                    for (int n = 0; n < N; ++n)
                    {
                        newSchedule.Matrix[i, n] = schedule2.Matrix[i, n];
                    }
                }
            }
            newSchedule.CalculateFitness();
            return newSchedule;
        }
        static void Mutate(Schedule schedule0)
        {
            if (rand.NextDouble() < MutationRate)
            {
                int n1 = rand.Next(1, N);
                int n2 = rand.Next(1, N);
                int r = rand.Next(1, R);
                int tmp = schedule0.Matrix[r, n1];
                schedule0.Matrix[r, n1] = schedule0.Matrix[r, n2];
                schedule0.Matrix[r, n2] = tmp;
                schedule0.CalculateFitness();
            }
        }
        public static List<Schedule> NextGeneration(List<Schedule> population)
        {
            List<Schedule> newPopulation = new List<Schedule>();
            for (int i = 0; i < TournamentScheduler.PopulationSize; ++i)
            {
                Schedule schedule1 = SelectParent(population);
                Schedule schedule2 = SelectParent(population);
                Schedule newSchedule = Crossover(schedule1, schedule2);
                Mutate(newSchedule); 
                Mutate(schedule1);
                Mutate(schedule2);
                newPopulation.Add(newSchedule);
                newPopulation.Add(schedule1);
                newPopulation.Add(schedule2);    
            }
            newPopulation = newPopulation.OrderByDescending(s => s.Fitness).Take(PopulationSize).ToList();
            return newPopulation;
        }
    }
}