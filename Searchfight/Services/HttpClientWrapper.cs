using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Searchfight.Services
{
	public class HttpClientWrapper : IHttpClientWrapper
	{
		public async Task<string> GetSearchResult(string uri, string searchTerms)
		{
			if (string.IsNullOrWhiteSpace(uri) || string.IsNullOrWhiteSpace(searchTerms))
				return string.Empty;

			using (var httpClient = new HttpClient())
			{
				var url = new Uri(uri + searchTerms);
				using (var request = new HttpRequestMessage(HttpMethod.Get, url))
				{
					request.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
					request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
					request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
					request.Headers.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");

					using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
					{
						try
						{
							response.EnsureSuccessStatusCode();
							using (var responseStream =
								await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
							using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
							using (var streamReader = new StreamReader(decompressedStream))
							{
								return await streamReader.ReadToEndAsync().ConfigureAwait(false);
							}
						}
						catch (Exception)
						{
							return string.Empty;
						}
					}
				}
			}
		}
	}
}
