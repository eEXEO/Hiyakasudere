using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.Functionality;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Hiyakasudere.Data.ExternalAPI.Yandere;

public class YanderePostService : IYanderePostService
{
    HttpClient client = HttpClientProvider.Client;
    IAppConfigService _appConfigService;

    public YanderePostService(IAppConfigService appConfigService)
    {
        _appConfigService = appConfigService;
    }

    public async Task<IEnumerable<YanderePost>> GetYandereData(string request)
    {
        try
        {
            var response = await client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<YanderePost>>(content);
            }
        }
        catch (TaskCanceledException) { } // Timeout - silent
        catch (HttpRequestException) { } // Network unreachable - silent
        catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Yandere] {e.Message}"); }

        return null;
    }

    public string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags)
    {
        string requestUri = $"https://yande.re/post.json?limit={postsPerPage}&page={currentPage}&tags=";

        foreach (var tag in tags.Where(t => !string.IsNullOrEmpty(t)))
            requestUri += tag + " ";

        foreach (var tag in blackTags.Where(t => !string.IsNullOrEmpty(t)))
            requestUri += "-" + tag + "+";

        return requestUri;
    }

    public async Task<IEnumerable<YandereTag>> GetTagsAutocompletion(string partialTag)
    {
        try
        {
            var response = await client.GetAsync($"https://yande.re/tag.json?limit=10&name={partialTag}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<YandereTag>>(content);
            }
        }
        catch (TaskCanceledException) { }
        catch (HttpRequestException) { }
        catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Yandere Tags] {e.Message}"); }

        return new List<YandereTag>();
    }

    public async Task<int> GetYanderePostCount(List<string> tags)
    {
        try
        {
            var req = "https://yande.re/post.xml?limit=1&tags=";
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
        catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Yandere Count] {e.Message}"); }

        return 0;
    }
}
