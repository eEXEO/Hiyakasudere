using Hiyakasudere.Data.Internal.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;

namespace Hiyakasudere.Data.ExternalAPI.Yandere;

public class YanderePostService : IYanderePostService
{
    HttpClient client;
    XDocument xDocument;
    IAppConfigService _appConfigService;

    public YanderePostService(IAppConfigService appConfigService)
    {
        client = new HttpClient();
        xDocument = new XDocument();
        _appConfigService = appConfigService;
    }

    public async Task<IEnumerable<YanderePost>> GetYandereData(string request)
    {
        IEnumerable<YanderePost> yanderePosts = null;

        try
        {
            var response = await client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                yanderePosts = JsonConvert.DeserializeObject<IEnumerable<YanderePost>>(content);
            }

        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            throw;
        }

        return yanderePosts;
    }
    public string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags)
    {
        string requestUri = "https://yande.re/post.json";
        requestUri += $"?limit={postsPerPage}";
        requestUri += $"&page={currentPage}";
        requestUri += "&tags=rating:s+";

        if(tags.Any())
        {
            foreach(var tag in tags)
            {
                requestUri += tag.ToString()+" ";
            }
        }

        if (blackTags.Any())
        {
            foreach (var tag in blackTags)
            {
                requestUri += "-" + tag.ToString() + "+";
            }
        }

        return requestUri;
    }

    public async Task<IEnumerable<YandereTag>> GetTagsAutocompletion(string partialTag)
    {
        IEnumerable<YandereTag> results = new List<YandereTag>();

        try
        {
            var req = "https://yande.re/tag.json?limit=10&name=" + partialTag;

            try
            {
                var response = await client.GetAsync(req);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    results = JsonConvert.DeserializeObject<IEnumerable<YandereTag>>(content);
                }
            } catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            throw;
        }

        return results;
    }

        public async Task<int> GetYanderePostCount(List<string> tags)
    {
        int yanderePostCount = 0;

        try
        {
            var req = "https://yande.re/post.xml?limit=1&tags=rating:s+";

            if (tags.Any())
            {
                foreach (var tag in tags)
                {
                    req += tag.ToString() + " ";
                }
            }

            var response = await client.GetAsync(req);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                xDocument = XDocument.Parse(content);
                yanderePostCount = int.Parse(xDocument.Root.Attribute("count").Value);
            }

        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            throw;
        }

        return yanderePostCount;
    }

}
