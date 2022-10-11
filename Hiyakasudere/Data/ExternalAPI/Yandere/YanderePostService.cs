using Hiyakasudere.Data.Internal.Config;
using Newtonsoft.Json;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

namespace Hiyakasudere.Data.ExternalAPI.Yandere;

public class YanderePostService
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
                System.Diagnostics.Debug.WriteLine("Incoming Size: " + yanderePosts.Count());
            }

        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            throw;
        }

        return yanderePosts;
    }
    public string GenerateRequestURL(int postsPerPage, int currentPage)
    {
        string requestUri = "https://yande.re/post.json";
        requestUri += $"?limit={postsPerPage}";
        requestUri += $"&page={currentPage}";
        requestUri += "&tags=";
        System.Diagnostics.Debug.WriteLine(_appConfigService.isNSFW);

        return requestUri;
    }

    public async Task<int> GetYanderePostCount()
    {
        int yanderePostCount = 0;

        try
        {
            var response = await client.GetAsync("https://yande.re/post.xml?limit=1");
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
