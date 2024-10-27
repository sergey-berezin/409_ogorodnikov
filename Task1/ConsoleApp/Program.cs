using ClassLibNamespace;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Enter.EnterParameters();
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
                List<TournamentScheduler.Schedule> population = new List<TournamentScheduler.Schedule>();
                for (int i = 0; i < TournamentScheduler.PopulationSize; ++i)
                {
                    population.Add(TournamentScheduler.CreateRandomSchedule());
                }
                for (int g = 0; g < TournamentScheduler.Generations; ++g)
                {
                    population = TournamentScheduler.NextGeneration(population);
                    TournamentScheduler.Schedule bestSchedule = population.OrderByDescending(s => s.Fitness).First();
                    Console.WriteLine($"Generation {g + 1}: Best Fitness = {bestSchedule.Fitness}");
                    if (g == TournamentScheduler.Generations - 1)
                    {
                        Console.WriteLine("Best schedule is found:");
                        for (int r = 0; r < TournamentScheduler.R; ++r)
                        {
                            for (int n = 0; n < TournamentScheduler.N; ++n)
                            {
                                Console.Write($"{bestSchedule.Matrix[r, n], 2} ");
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}