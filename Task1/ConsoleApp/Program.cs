using System.Diagnostics;
using ClassLibNamespace;
class Program
{
    static void Main(string[] args)
    {
        
        Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
        TournamentScheduler.Schedule bestSchedule = TournamentScheduler.GeneticAlgorithm();
        Console.WriteLine($"{TournamentScheduler.R} rounds\n{TournamentScheduler.N} participants\n{TournamentScheduler.K} locations");
        Console.WriteLine("Best schedule is found:");
        for (int r = 0; r < TournamentScheduler.R; ++r)
        {
            for (int n = 0; n < TournamentScheduler.N; ++n)
            {
                Console.Write($"{bestSchedule.Matrix[r, n]} ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    protected static void myHandler(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Console.WriteLine("Ctrl+C was pressed");
            
        }
}