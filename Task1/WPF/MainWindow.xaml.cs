using System.Windows;
using System.Windows.Threading;
using TournamentSchedule;
namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private DispatcherTimer _timer;
        private TournamentScheduler.Schedule? _bestSchedule;
        private List<TournamentScheduler.Schedule>? _population;
        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;

            var experiments = ExperimentManager.LoadExperiments();
            ExperimentList.ItemsSource = null;
            ExperimentList.ItemsSource = experiments;
            //ExperimentList.ItemsSource = experiments;
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_bestSchedule != null)
            {
                CurrentFitness.Text = $"Generation {_bestSchedule.CurrentGeneration}; Best Fitness = {_bestSchedule.Fitness}";
                BestSolution.Text = FormatSchedule(_bestSchedule);
            }
        }
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

            _population = new List<TournamentScheduler.Schedule>();
            for (int i = 0; i < TournamentScheduler.PopulationSize; ++i)
            {
                _population.Add(TournamentScheduler.CreateRandomSchedule());
            }

            _timer.Start();

            try
            {
                await Task.Run(() => RunAlgorithm(_cancellationTokenSource.Token));
            }
            catch (OperationCanceledException) {
                MessageBox.Show("Algorithm was cancelled.");
            }   
            finally {
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
                _timer.Stop();
                if (_bestSchedule != null)
                {
                    CurrentFitness.Text = $"Generation {_bestSchedule.CurrentGeneration}; Best Fitness = {_bestSchedule.Fitness}";
                    BestSolution.Text = FormatSchedule(_bestSchedule);
                }
            }
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }
        private void RunAlgorithm(CancellationToken token)
        { 
            for (int g = _population.First().CurrentGeneration; g < TournamentScheduler.Generations; ++g)
            {
                token.ThrowIfCancellationRequested();
                _population = TournamentScheduler.NextGeneration(_population);
                TournamentScheduler.SetGeneration(_population, g + 1);
                _bestSchedule = _population.OrderByDescending(s => s.Fitness).First();
            }
        }
        private static string FormatSchedule(TournamentScheduler.Schedule schedule)
        {
            string result = "";
            for (int r = 0; r < TournamentScheduler.R; ++r)
            {
                for (int n = 0; n < TournamentScheduler.N; ++n)
                {
                    result += $"{schedule.Matrix[r][n], 3} ";
                }
                result += Environment.NewLine;
            }
            return result;
        }
        private void SaveExperiment_Click(object sender, RoutedEventArgs e)
        {
            if (_population == null || !_population.Any())
            {
                MessageBox.Show("No population to save");
            }
            var experimentName = ExperimentNameInput.Text;
            if (string.IsNullOrWhiteSpace(experimentName))
            {
                MessageBox.Show("Enter a valid name");
            }
            var experiments = ExperimentManager.LoadExperiments();
            var filename = $"experiment_{experiments.Count + 1}.json";

            ExperimentManager.SavePopulation(filename, _population);
            experiments.Add(new Experiment { Name = experimentName, FileName = filename });
            ExperimentManager.SaveExperiments(experiments);

            //ExperimentList.ItemsSource = experiments;
            ExperimentList.ItemsSource = null;
            ExperimentList.ItemsSource = experiments;
            MessageBox.Show("Experiment was saved successfuly.");
        }
        private void LoadExperiment_Click(object sender, RoutedEventArgs e)
        {
            var selectedExperiment = ExperimentList.SelectedItem as Experiment;
            if (selectedExperiment != null)
            {
                MessageBox.Show("Select an experiment to load");
                return;
            }
            _population = ExperimentManager.LoadPopulation(selectedExperiment.FileName);
            _bestSchedule = _population.OrderByDescending(s => s.Fitness).First();
            int currentGeneration = _population.First().CurrentGeneration;
            TournamentScheduler.SetGeneration(_population, currentGeneration);
            CurrentFitness.Text = $"Generation {_bestSchedule.CurrentGeneration}; Best Fitness = {_bestSchedule.Fitness}";
            BestSolution.Text = FormatSchedule(_bestSchedule);
            MessageBox.Show($"Experiment \"{selectedExperiment.Name}\" loaded successfully.");
        }
        private async void ContinueOptimization_Click(object sender, RoutedEventArgs e)
        {
            if (_population == null || !_population.Any())
            {
                MessageBox.Show("No population loaded. Load an experiment first.");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            _timer.Start();

            try
            {
                await Task.Run(() => RunAlgorithm(_cancellationTokenSource.Token));
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Optimization was cancelled.");
            }
            finally
            {
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
                _timer.Stop();
                if (_bestSchedule != null)
                {
                    CurrentFitness.Text = $"Generation {_bestSchedule.CurrentGeneration}; Best Fitness = {_bestSchedule.Fitness}";
                    BestSolution.Text = FormatSchedule(_bestSchedule);
                }
            }
        }
    }
}