using System.Text.Json;
using System.IO;
using TournamentSchedule;
using Microsoft.Win32;
using System.Windows;

public class Experiment
{
    public string Name { get; set; }
    public string FileName { get; set; }
}
public class PopulationData
{
    public int[][][] PopulationMatrix { get; set; }
    public int[] FitnessValues { get; set; }
    public int GenerationValues { get; set; } //номер поколения у всей популяции один
}
public static class ExperimentManager
{
    private const string RunsFile = "runs.json";
    public static List<Experiment> LoadExperiments()
    {
        if (!File.Exists(RunsFile))
            return new List<Experiment>();
        var json = File.ReadAllText(RunsFile);
        return JsonSerializer.Deserialize<List<Experiment>>(json) ?? new List<Experiment>();
    }
    public static void SaveExperiments(List<Experiment> experiments)
    {
        var tempFile = RunsFile + ".tmp";
        var json = JsonSerializer.Serialize(experiments, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(tempFile, json);
        if (File.Exists(RunsFile))
        {
            File.Replace(tempFile, RunsFile, null);
        }
        else
        {
            File.Move(tempFile, RunsFile);
        }
    }
    public static void SavePopulation(string fileName, List<TournamentScheduler.Schedule> population)
    {
        var data = new PopulationData
        {
            PopulationMatrix = population.Select(p => p.Matrix).ToArray(),
            FitnessValues = population.Select(p => p.Fitness).ToArray(),
            GenerationValues = population.First().CurrentGeneration
        };
        var tempFile = fileName + ".tmp";
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(tempFile, json);
        if (File.Exists(fileName))
        {
            File.Replace(tempFile, fileName, null);
        }
        else
        {
            File.Move(tempFile, fileName);
        }
    }
    public static List<TournamentScheduler.Schedule> LoadPopulation(string fileName)
    {
        var json = File.ReadAllText(fileName);
        var data = JsonSerializer.Deserialize<PopulationData>(json);
        var population = data.PopulationMatrix
                .Zip(data.FitnessValues, (matrix, fitness) =>
                    new TournamentScheduler.Schedule
                    {
                        Matrix = matrix,
                        Fitness = fitness
                    }).ToList();
        foreach (var schedule in population)
        {
            schedule.CurrentGeneration = data.GenerationValues;
        }
        return population;
    }

}


