using NUnit.Framework;
using Searchfight.Services;
using System.Threading.Tasks;

namespace Tests
{
	public class HttpClientWrapperTests
	{
		private IHttpClientWrapper _httpClientWrapper;

		[SetUp]
		public void Setup()
		{
			_httpClientWrapper = new HttpClientWrapper();
		}

		[Test]
		public async Task GetSearchResult_SearchTermAndUriPassed_ContentIsReturned()
		{
			var result = await _httpClientWrapper.GetSearchResult("http://www.bing.com/search?q=", "test")
				.ConfigureAwait(false);

			Assert.IsNotEmpty(result);
		}

		[Test]
		public async Task GetSearchResult_SearchTermAndUriIsNotPassed_ContentIsReturned()
		{
			var result = await _httpClientWrapper.GetSearchResult("", "")
				.ConfigureAwait(false);

			Assert.IsEmpty(result);
		}
	}
}