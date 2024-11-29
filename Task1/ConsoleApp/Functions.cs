using TournamentSchedule;

namespace ConsoleApp
{
    internal class Function
    {
        public static void EnterParameters()
        {
            Console.WriteLine("Enter number of rounds(R), participants(N) and locations(K)");
            Console.Write("Rounds(R) = ");
            TournamentScheduler.R = Convert.ToInt32(Console.ReadLine());
            Console.Write("Participants(N) = ");
            TournamentScheduler.N = Convert.ToInt32(Console.ReadLine());
            Console.Write("Locations(K) = ");
            TournamentScheduler.K = Convert.ToInt32(Console.ReadLine());
        }
        public static void PrintScheduleMatrix(TournamentScheduler.Schedule schedule)
        {   
            Console.WriteLine("Best schedule is found:");
            for (int r = 0; r < TournamentScheduler.R; ++r)
            {
                for (int n = 0; n < TournamentScheduler.N; ++n)
                {
                    Console.Write($"{schedule.Matrix[r][n], 2} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
