using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LAB_V2.Worker
{
    public class Scraper
    {
        public static string BranchScraper(string link)
        {
            HttpClient client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true, PreAuthenticate = true });
            string response = client.GetStringAsync(@$"{link}").GetAwaiter().GetResult();
            Regex regex = new Regex(@"Merge pull request (?:[0-9]*?) from (?<branch>.*?) into", RegexOptions.Compiled);
            Match match = regex.Match(response);
            string branchName = match.Groups["branch"].Value;

            return branchName;
        }

        public static string RepositoryScraper(string url)
        {
            HttpClient client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true, PreAuthenticate = true });
            string response = client.GetStringAsync(@$"{url}").GetAwaiter().GetResult();

            Regex regex = new Regex(@"(?:[0-9]*?),""GitRepositoryName"":""(?<repository>.*?)""", RegexOptions.Compiled);
            Match match = regex.Match(response);
            string repository = match.Groups["repository"].Value;

            return repository;
        }
    }
}
