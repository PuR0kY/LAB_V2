using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;

namespace LAB_V2.Worker.Other
{
    public class Scraper
    {
        public async Task<(string branch, string repository)> ScrapePRLink(string? link)
        {
            #region strings
            HttpClient client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true, PreAuthenticate = true });
            string response = await client.GetStringAsync(@$"{link}");
            string idk = "\"CodeReviewStatus\":{\"$type\":\"System.String\",\"$value\":\"Completed\"}";
            string messageBoxText = "Selected Pull Request has status Completed. Do you want to Checkout master branch?";
            string windowName = "Completed branch warning";
            string branchName = "";
            #endregion

            if (response.Contains(idk))
            {
                if (MessageBox.Show(messageBoxText,
                    windowName,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                    branchName = "master";
            }

            if (!response.Contains(idk))
            {
                //Branch
                Regex regex = new Regex(@"Merge pull request (?:[0-9]*?) from (?<branch>.*?) into", RegexOptions.Compiled);
                Match match = regex.Match(response);
                branchName = match.Groups["branch"].Value;
            }

            //Repository
            Regex regex2 = new Regex(@"(?:[0-9]*?),""GitRepositoryName"":""(?<repository>.*?)""", RegexOptions.Compiled);
            Match match2 = regex2.Match(response);
            string repository = match2.Groups["repository"].Value;

            return (branchName, repository);
        }
    }
}
