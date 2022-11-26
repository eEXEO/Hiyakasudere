using Hiyakasudere.Data.Internal.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;

namespace Hiyakasudere.Data.ExternalAPI.Konachan;

public class KonachanPostService : IKonachanPostService
{
    HttpClient client;
    XDocument xDocument;
    IAppConfigService _appConfigService;

    public KonachanPostService(IAppConfigService appConfigService)
    {
        client = new HttpClient();
        xDocument = new XDocument();
        _appConfigService = appConfigService;
    }

    public async Task<IEnumerable<KonachanPost>> GetKonachanData(string request)
    {
        Debug.WriteLine(request);
        IEnumerable<KonachanPost> konachanPosts = null;        

        try
        {
            var response = await client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                konachanPosts = JsonConvert.DeserializeObject<IEnumerable<KonachanPost>>(content);
            }

        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            throw;
        }

        return konachanPosts;
    }
    public string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags)
    {
        string requestUri = "https://konachan.com/post.json";
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

    public async Task<IEnumerable<KonachanTag>> GetTagsAutocompletion(string partialTag)
    {
        IEnumerable<KonachanTag> results = null;

        try
        {
            var req = "https://konachan.com/tag.json?limit=10&name=" + partialTag;

            var response = await client.GetAsync(req);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                results = JsonConvert.DeserializeObject<IEnumerable<KonachanTag>>(content);
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            throw;
        }

        return results;
    }

        public async Task<int> GetKonachanPostCount(List<string> tags)
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
