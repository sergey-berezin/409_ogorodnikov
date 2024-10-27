using System.Windows;
using ClassLibNamespace;
namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private CancellationTokenSource _cancellationTokenSource;
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int rounds = int.Parse(RoundsInput.Text);
            int participants = int.Parse(ParticipantsInput.Text);
            int locations = int.Parse(LocationsInput.Text);
            
            TournamentScheduler.R = rounds;
            TournamentScheduler.N = participants;
            TournamentScheduler.K = locations;
            
            _cancellationTokenSource = new CancellationTokenSource();

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            try
            {
                await RunAlgorithmAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Algorithm was cancelled.");
            }
            finally
            {
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
            }
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }
        private async Task RunAlgorithmAsync(CancellationToken token)
        {
            List<TournamentScheduler.Schedule> population = new List<TournamentScheduler.Schedule>();
            for (int i = 0; i < TournamentScheduler.PopulationSize; ++i)
            {
                population.Add(TournamentScheduler.CreateRandomSchedule());
            }

            for (int g = 0; g < TournamentScheduler.Generations; ++g)
            {
                token.ThrowIfCancellationRequested();
                population = TournamentScheduler.NextGeneration(population);
                var bestSchedule = population.OrderByDescending(s => s.Fitness).First();
                await Dispatcher.InvokeAsync(() =>
                {
                    CurrentFitness.Text = $"Generation {g + 1}: Best Fitness = {bestSchedule.Fitness}";
                    BestSolution.Text = FormatSchedule(bestSchedule);
                });

                await Task.Delay(100);
            }
        }
        private string FormatSchedule(TournamentScheduler.Schedule schedule)
        {
            string result = "";
            for (int r = 0; r < TournamentScheduler.R; ++r)
            {
                for (int n = 0; n < TournamentScheduler.N; ++n)
                {
                    result += $"{schedule.Matrix[r, n], 3} ";
                }
                result += Environment.NewLine;
            }
            return result;
        }
    }
}