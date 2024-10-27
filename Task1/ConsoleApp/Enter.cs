using ClassLibNamespace;

namespace ConsoleApp
{
    internal class Enter
    {
        public static void EnterParameters()
        {
            Console.WriteLine("Enter number of rounds(R), participants(P) and locations(K)");
            Console.Write("Rounds(R) = ");
            TournamentScheduler.R = Convert.ToInt32(Console.ReadLine());
            Console.Write("Participants(P) = ");
            TournamentScheduler.N = Convert.ToInt32(Console.ReadLine());
            Console.Write("Locations(L) = ");
            TournamentScheduler.K = Convert.ToInt32(Console.ReadLine());
        }
    }
}
