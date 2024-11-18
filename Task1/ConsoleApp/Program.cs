using TournamentSchedule;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Function.EnterParameters();
            if (TournamentScheduler.CheckParameters())
            {
                Console.WriteLine("Check the correctness of parameters. They must satisfy the inequasion: 1 ≤ R < N ≤ K");
                return;
            }
            var stopEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                stopEvent.Set();
            };
            while (!stopEvent.WaitOne(0))
            {
                var population = new List<TournamentScheduler.Schedule>();
                for (int i = 0; i < TournamentScheduler.PopulationSize; ++i)
                {
                    population.Add(TournamentScheduler.CreateRandomSchedule());
                }
                for (int g = 0; g < TournamentScheduler.Generations; ++g)
                {
                    population = TournamentScheduler.NextGeneration(population);
                    var bestSchedule = population.OrderByDescending(s => s.Fitness).First();
                    Console.WriteLine($"Generation {g + 1}: Best Fitness = {bestSchedule.Fitness}");
                    if (g == TournamentScheduler.Generations - 1)
                    {
                        Function.PrintScheduleMatrix(bestSchedule);
                    }
                }
            }
        }
    }
}