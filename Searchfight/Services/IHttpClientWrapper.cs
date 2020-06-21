using System.Threading.Tasks;

namespace Searchfight.Services
{
	public interface IHttpClientWrapper
	{
		Task<string> GetSearchResult(string uri, string searchTerms);
	}
}
