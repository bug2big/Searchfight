using Searchfight.Models;
using Searchfight.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Searchfight
{
	public class App
	{
		private readonly IHttpClientWrapper _clientWrapper;
		private readonly SearchConfigurationModel _searchConfiguration;

		public App(IHttpClientWrapper clientWrapper)
		{
			_clientWrapper = clientWrapper;
			_searchConfiguration = CreateConfiguration();
		}

		public async Task Run()
		{
			var results = await RetrieveChallengeResult()
				.ConfigureAwait(false);

			FormatResult(results);

			Console.ReadKey();
		}

		private async Task<List<ResultModel>> RetrieveChallengeResult()
		{
			var googleConfiguration = _searchConfiguration.SearchEngineConfig[SearchEngineType.Google];
			var msnConfiguration = _searchConfiguration.SearchEngineConfig[SearchEngineType.Msn];

			var searchTerms = _searchConfiguration.SearchTerms;

			var results = new List<ResultModel>();

			var googleResults = new ResultModel
			{
				Type = SearchEngineType.Google
			};

			foreach (var term in searchTerms)
			{
				var content = await _clientWrapper.GetSearchResult(googleConfiguration[0], term)
					.ConfigureAwait(false);
				var result = TrimUnnecessaryContent(content, googleConfiguration[1]);
				int amount;

				if (term == ".net")
				{
					int.TryParse(result, out amount);
					googleResults.AmountForNet = amount;
				}
				else
				{
					int.TryParse(result, out amount);
					googleResults.AmountForJava = amount;
				}
			}

			results.Add(googleResults);

			var msnResults = new ResultModel
			{
				Type = SearchEngineType.Msn
			};

			foreach (var term in searchTerms)
			{
				var content = await _clientWrapper.GetSearchResult(msnConfiguration[0], term)
					.ConfigureAwait(false);
				var result = TrimUnnecessaryContent(content, msnConfiguration[1]);
				int amount;

				if (term == ".net")
				{
					int.TryParse(result, out amount);
					msnResults.AmountForNet = amount;
				}
				else
				{
					int.TryParse(result, out amount);
					msnResults.AmountForJava = amount;
				}
			}

			results.Add(msnResults);

			return results;
		}

		private static string TrimUnnecessaryContent(string content, string tag)
		{
			if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(tag))
				return string.Empty;

			var index = content.IndexOf(tag) + tag.Length;
			content = content.Substring(index, 30);
			content = Regex.Replace(content, @"[^\d]", "");

			return content;
		}

		private void FormatResult(List<ResultModel> resultModels)
		{
			var googleWinner = resultModels.Where(i => i.Type == SearchEngineType.Google)
				.Select(i => new
				{
					WinnerName = i.AmountForNet > i.AmountForJava ? ".net" : "java",
					i.Type
				}).FirstOrDefault();

			var msnWinner = resultModels.Where(i => i.Type == SearchEngineType.Msn)
				.Select(i => new
				{
					WinnerName = i.AmountForNet > i.AmountForJava ? ".net" : "java",
					i.Type
				}).FirstOrDefault();

			var totalWinner = resultModels.Max(i => i.AmountForJava) > resultModels.Max(i => i.AmountForJava) ? ".net" : "java";

			Console.WriteLine($".net: Google: { resultModels.FirstOrDefault(i => i.Type == SearchEngineType.Google)?.AmountForNet } " +
							  $"MSN Search: {resultModels.FirstOrDefault(i => i.Type == SearchEngineType.Msn)?.AmountForNet}");
			Console.WriteLine($"java: Google: {resultModels.FirstOrDefault(i => i.Type == SearchEngineType.Google)?.AmountForJava} " +
							  $"MSN Search: {resultModels.FirstOrDefault(i => i.Type == SearchEngineType.Msn)?.AmountForJava}");
			Console.WriteLine($"Google winner: {googleWinner?.WinnerName}");
			Console.WriteLine($"MSN Search winner: {msnWinner?.WinnerName}");
			Console.WriteLine($"Total winner: {totalWinner}");
		}

		private SearchConfigurationModel CreateConfiguration()
		{
			return new SearchConfigurationModel
			{
				SearchEngineConfig = new Dictionary<SearchEngineType, string[]>
				{
					{SearchEngineType.Google, new [] { "https://www.google.com/search?q=.net", "<div id=\"result-stats\">" } },
					{SearchEngineType.Msn, new [] {"https://www.bing.com/search?q=", "<span class=\"sb_count\"" } }
				},
				SearchTerms = new[]
				{
					".net",
					"java"
				}
			};
		}
	}
}
