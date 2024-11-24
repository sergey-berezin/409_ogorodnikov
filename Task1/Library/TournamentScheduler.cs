using System.Collections.Concurrent;

namespace TournamentSchedule
{
    public class TournamentScheduler
    {
        public static Random rand = new();
        public static int R;
        public static int N;
        public static int K;
        public static int PopulationSize = 20;
        public static int Generations = 500;
        public static double MutationRate = 0.1;
        public class Schedule
        {
            public int[][] Matrix { get; set; }
            public int Fitness { get; set; }
            public int CurrentGeneration { get; set;}
            public Schedule()
            {
                Matrix = new int[R][];
                for (int r = 0; r < R; ++r)
                {
                    Matrix[r] = new int[N];
                }
                Fitness = 0;
                CurrentGeneration = 0;
            }
            public int UniqueOpponentsCount(int player)
            {
                var opponents = new HashSet<int>();
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
                var locations = new HashSet<int>();
                for (int r = 0; r < R; ++r) 
                {
                    locations.Add(Matrix[r][player]);
                }
                return locations.Count;
            }
            private int FindOpponentInRound(int round, int player)
            {
                for (int n = 0; n < N; n++)
                {
                    if ((n != player) && (Matrix[round][n] == Matrix[round][player])) 
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
        public static bool CheckParameters(int R, int N, int K)
        {
            return 1 <= R && R < N && N <= K;
        }
        public static Schedule CreateRandomSchedule()
        {
            var schedule = new Schedule();
            Parallel.For(0, R, r => 
            {
                int [] freeLocations = Enumerable.Range(1, K).ToArray();
                int [] unscheduled = Enumerable.Range(0, N).ToArray();
                while (unscheduled.Length > 1)
                {
                    int randomLocation = rand.Next(freeLocations.Length);
                    int randomPlayer = rand.Next(unscheduled.Length);
                    schedule.Matrix[r][unscheduled[randomPlayer]] = freeLocations[randomLocation];
                    unscheduled = unscheduled.Where((val, idx) => idx != randomPlayer).ToArray();
                    randomPlayer = rand.Next(unscheduled.Length);
                    schedule.Matrix[r][unscheduled[randomPlayer]] = freeLocations[randomLocation];
                    unscheduled = unscheduled.Where((val, idx) => idx != randomPlayer ).ToArray();
                    freeLocations = freeLocations.Where((val, idx) => idx != randomLocation).ToArray();
                } 
                if (unscheduled.Length == 1)
                {
                    int randomLocation1 = rand.Next(freeLocations.Length);
                    int randomPlayer1 = unscheduled[0];
                    schedule.Matrix[r][randomPlayer1] = freeLocations[randomLocation1];
                }
            });
            schedule.CalculateFitness();
            return schedule;
        }
        static Schedule SelectParent(List<Schedule> population)
        {
            int tournamentSize = 5;
            var tournament = new List<Schedule>();
            for (int i = 0; i < tournamentSize; ++i)
            {
                tournament.Add(population[rand.Next(population.Count)]);
            }
            return tournament.OrderByDescending(s => s.Fitness).First();
        }
        static Schedule Crossover(Schedule schedule1, Schedule schedule2)
        {
            var newSchedule = new Schedule();
            int crossPoint = rand.Next(1, R);
            for (int i = 0; i < R; ++i)
            {
                if (i < crossPoint)
                {
                    for (int n = 0; n < N; ++n)
                    {
                        newSchedule.Matrix[i][n] = schedule1.Matrix[i][n];
                    }
                }
                else
                {
                    for (int n = 0; n < N; ++n)
                    {
                        newSchedule.Matrix[i][n] = schedule2.Matrix[i][n];
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
                var tuple = new Tuple<int, int>(schedule0.Matrix[r][n1], schedule0.Matrix[r][n2]);
                schedule0.Matrix[r][n1] = tuple.Item2;
                schedule0.Matrix[r][n2] = tuple.Item1;
                schedule0.CalculateFitness();
            }
        }
        public static void SetGeneration(List<Schedule> population, int g)
        {
            Parallel.For(0, population.Count, i =>
                population[i].CurrentGeneration = g
            );
        }
        public static List<Schedule> NextGeneration(List<Schedule> population)
        {
            var tasks = new List<Task>();
            var newPopulation = new ConcurrentBag<Schedule>();
            for (int i = 0; i < PopulationSize; ++i)
            {
                tasks.Add(Task.Run(() =>
                {
                    Schedule schedule1 = SelectParent(population);
                    Schedule schedule2 = SelectParent(population);
                    Schedule newSchedule = Crossover(schedule1, schedule2);
                    newPopulation.Add(newSchedule);
                    newPopulation.Add(schedule1);
                    newPopulation.Add(schedule2);
                    Mutate(newSchedule); 
                    Mutate(schedule1);
                    Mutate(schedule2);
                    newPopulation.Add(newSchedule);
                    newPopulation.Add(schedule1);
                    newPopulation.Add(schedule2);    
                }));
            }
            Task.WaitAll([..tasks]);
            var newPopulationList = new List<Schedule>(newPopulation);
            newPopulationList = newPopulationList.OrderByDescending(s => s.Fitness).Take(PopulationSize / 2).ToList();            
            newPopulationList = newPopulationList.OrderByDescending(s => s.Fitness).TakeLast(PopulationSize / 2).ToList();
            return newPopulationList;
        }
    }
}