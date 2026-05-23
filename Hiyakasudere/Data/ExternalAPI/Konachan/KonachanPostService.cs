using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.Functionality;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Hiyakasudere.Data.ExternalAPI.Konachan;

public class KonachanPostService : IKonachanPostService
{
    HttpClient client = HttpClientProvider.Client;
    IAppConfigService _appConfigService;

    public KonachanPostService(IAppConfigService appConfigService)
    {
        _appConfigService = appConfigService;
    }

    public async Task<IEnumerable<KonachanPost>> GetKonachanData(string request)
    {
        try
        {
            var response = await client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<KonachanPost>>(content);
            }
        }
        catch (TaskCanceledException) { }
        catch (HttpRequestException) { }
        catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Konachan] {e.Message}"); }

        return null;
    }

    public string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags)
    {
        string requestUri = $"https://konachan.com/post.json?limit={postsPerPage}&page={currentPage}&tags=";

        foreach (var tag in tags.Where(t => !string.IsNullOrEmpty(t)))
            requestUri += tag + " ";

        foreach (var tag in blackTags.Where(t => !string.IsNullOrEmpty(t)))
            requestUri += "-" + tag + "+";

        return requestUri;
    }

    public async Task<IEnumerable<KonachanTag>> GetTagsAutocompletion(string partialTag)
    {
        try
        {
            var response = await client.GetAsync($"https://konachan.com/tag.json?limit=10&name={partialTag}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<KonachanTag>>(content);
            }
        }
        catch (TaskCanceledException) { }
        catch (HttpRequestException) { }
        catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Konachan Tags] {e.Message}"); }

        return new List<KonachanTag>();
    }

    public async Task<int> GetKonachanPostCount(List<string> tags)
    {
        try
        {
            var req = "https://konachan.com/post.xml?limit=1&tags=";
            foreach (var tag in tags.Where(t => !string.IsNullOrEmpty(t)))
                req += tag + " ";

            var response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var xDoc = XDocument.Parse(content);
                return int.Parse(xDoc.Root.Attribute("count").Value);
            }
        }
        catch (TaskCanceledException) { }
        catch (HttpRequestException) { }
        catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Konachan Count] {e.Message}"); }

        return 0;
    }
}
