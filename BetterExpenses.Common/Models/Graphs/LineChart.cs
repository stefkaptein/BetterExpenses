namespace BetterExpenses.Common.Models.Graphs;

public class LineChart
{
    public List<string> Labels { get; set; }
    public Dictionary<string, List<double>> DataDictionary { get; set; }
}