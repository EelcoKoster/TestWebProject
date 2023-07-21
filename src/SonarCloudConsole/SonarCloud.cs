using Newtonsoft.Json;

namespace SonarCloudConsole
{
    public class SonarCloud
    {
        public static List<Issue> GetIssues(string component)
        {
            var result = new List<Issue>();
            var issues = GetIssues(component, 1);
            result.AddRange(issues.issues);
            var total = issues.ps * issues.p;
            while (issues != null && issues.paging.total > total)
            {
                issues = GetIssues(component, issues.paging.pageIndex + 1);
                result.AddRange(issues.issues);
                total = issues.ps * issues.p;
            }
            return result;
        }

        public static List<Measure> GetMeasurements(string component)
        {
            var client = new HttpClient();
            var response = client.GetAsync($"https://sonarcloud.io/api/measures/component?component={component}&metricKeys=bugs,code_smells,coverage,duplicated_lines_density,ncloc,security_hotspots,sqale_index,vulnerabilities").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var measurements = JsonConvert.DeserializeObject<Measurements>(content);
            return measurements?.component.measures;
        }

        private static Issues GetIssues(string component, int page)
        {
            var client = new HttpClient();
            var response = client.GetAsync($"https://sonarcloud.io/api/issues/search?componentKeys={component}&ps=100&p={page}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var issues = JsonConvert.DeserializeObject<Issues>(content);
            return issues;
        }
    }


    public class Issue
    {
        public string key { get; set; }
        public string rule { get; set; }
        public string severity { get; set; }
        public string component { get; set; }
        public string project { get; set; }
        public int line { get; set; }
        public string hash { get; set; }
        public TextRange textRange { get; set; }
        public List<object> flows { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string effort { get; set; }
        public string debt { get; set; }
        public string assignee { get; set; }
        public List<string> tags { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime updateDate { get; set; }
        public string type { get; set; }
        public string organization { get; set; }
        public string externalRuleEngine { get; set; }
    }

    public class Organization
    {
        public string key { get; set; }
        public string name { get; set; }
    }

    public class Paging
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
    }

    public class Issues
    {
        public int total { get; set; }
        public int p { get; set; }
        public int ps { get; set; }
        public Paging paging { get; set; }
        public int effortTotal { get; set; }
        public int debtTotal { get; set; }
        public List<Issue> issues { get; set; }
    }

    public class TextRange
    {
        public int startLine { get; set; }
        public int endLine { get; set; }
        public int startOffset { get; set; }
        public int endOffset { get; set; }
    }

    public class Component
    {
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string qualifier { get; set; }
        public List<Measure> measures { get; set; }
    }

    public class Measure
    {
        public string metric { get; set; }
        public string value { get; set; }
        public bool bestValue { get; set; }
    }

    public class Measurements
    {
        public Component component { get; set; }
    }
}
