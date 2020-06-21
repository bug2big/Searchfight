using System.Collections.Generic;

namespace Searchfight.Models
{
	public class SearchConfigurationModel
	{
		public Dictionary<SearchEngineType, string[]> SearchEngineConfig { get; set; }
		public string[] SearchTerms { get; set; }
	}
}
