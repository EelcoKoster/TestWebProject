using System.Text;

namespace SonarCloudConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string componentName = "vitas-development-webproject";

            var measurements = SonarCloud.GetMeasurements(componentName);
            var projectMeasurements = new ProjectMeasurements(measurements);
            Console.WriteLine($"Project summary for {componentName}:");
            Console.WriteLine($"The project consists of {projectMeasurements.LinesOfCode} lines of code.");
            Console.WriteLine($"The unittest of this project covers {projectMeasurements.Coverage}% of the code.");
            Console.WriteLine($"The project has {projectMeasurements.DuplicatedLinesDensity} duplicated lines of density.");
            Console.WriteLine($"SonarCloud found {projectMeasurements.SecurityHotspots} security hotspots, {projectMeasurements.Bugs} bugs and {projectMeasurements.CodeSmells} code smells which need to be addressed.");

            Console.WriteLine("---------------");

            var issues = SonarCloud.GetIssues(componentName);
            foreach (var issue in issues)
            {
                Console.WriteLine($"{issue.severity}: {issue.message}");
                string file = issue.component.Substring(issue.component.LastIndexOf(':') + 1);
                Console.WriteLine($" File: {file} in line {issue.line}");
            }
        }
    }

    public class ProjectMeasurements
    {
        List<Measure> _measturements;
        public ProjectMeasurements(List<Measure> measturements)
        {
            _measturements = measturements;
        }

        public int? Bugs { get { return Convert.ToInt32(_measturements.FirstOrDefault(m => m.metric == "bugs")?.value); } }
        public string Coverage { get { return _measturements.FirstOrDefault(m => m.metric == "coverage")?.value; } }
        public int? CodeSmells { get { return Convert.ToInt32(_measturements.FirstOrDefault(m => m.metric == "code_smells")?.value); } }
        public string DuplicatedLinesDensity { get { return _measturements.FirstOrDefault(m => m.metric == "duplicated_lines_density")?.value; } }
        public int? LinesOfCode { get { return Convert.ToInt32(_measturements.FirstOrDefault(m => m.metric == "ncloc")?.value); } }
        public int? SecurityHotspots { get { return Convert.ToInt32(_measturements.FirstOrDefault(m => m.metric == "security_hotspots")?.value); } }
    }
}