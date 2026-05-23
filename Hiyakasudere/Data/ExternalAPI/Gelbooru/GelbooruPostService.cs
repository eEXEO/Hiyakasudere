using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.Data.Post;
using Hiyakasudere.Data.Internal.Functionality;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hiyakasudere.Data.ExternalAPI.Gelbooru
{
    public class GelbooruPostService : IGelbooruPostService
    {
        HttpClient client = HttpClientProvider.Client;
        IAppConfigService _appConfigService;
        XmlSerializer serializer;

        public GelbooruPostService(IAppConfigService appConfigService)
        {
            _appConfigService = appConfigService;
            serializer = new XmlSerializer(typeof(GelbooruPosts));
        }

        private string AppendAuth(string url)
        {
            if (!string.IsNullOrEmpty(_appConfigService.GelbooruApiKey) && !string.IsNullOrEmpty(_appConfigService.GelbooruUserId))
                url += $"&api_key={_appConfigService.GelbooruApiKey}&user_id={_appConfigService.GelbooruUserId}";
            return url;
        }

        public string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags)
        {
            currentPage -= 1;
            var url = AppendAuth($"https://gelbooru.com/index.php?page=dapi&s=post&q=index&limit={postsPerPage}&pid={currentPage}");
            url += "&tags=";

            foreach (var tag in tags.Where(t => !string.IsNullOrEmpty(t)))
                url += tag + " ";
            foreach (var tag in blackTags.Where(t => !string.IsNullOrEmpty(t)))
                url += "-" + tag + "+";

            return url;
        }

        public async Task<int> GetGelbooruPostCount(List<string> tags)
        {
            try
            {
                var req = AppendAuth("https://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=0") + "&tags=";
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
            catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Gelbooru Count] {e.Message}"); }

            return 0;
        }

        public async Task<IEnumerable<TagInternal>> GetTagsAutocompletion(string partialTag)
        {
            List<TagInternal> results = new();
            try
            {
                var req = AppendAuth($"https://gelbooru.com/index.php?page=dapi&s=tag&q=index&limit=10&name_pattern=%25{partialTag}%25");
                var response = await client.GetAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var xDoc = XDocument.Parse(content);
                    foreach (var tag in xDoc.Descendants("tag"))
                    {
                        var name = tag.Attribute("name")?.Value ?? "";
                        long.TryParse(tag.Attribute("count")?.Value, out var c);
                        long.TryParse(tag.Attribute("type")?.Value, out var t);
                        long.TryParse(tag.Attribute("id")?.Value, out var i);
                        results.Add(new TagInternal(i, name, c, t, false));
                    }
                }
            }
            catch (TaskCanceledException) { }
            catch (HttpRequestException) { }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Gelbooru Tags] {e.Message}"); }

            return results;
        }

        public async Task<IEnumerable<GelbooruPost>> GetGelbooruData(string request)
        {
            try
            {
                var response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var members = (GelbooruPosts)serializer.Deserialize(await response.Content.ReadAsStreamAsync());
                    return members?.Posts?.AsEnumerable<GelbooruPost>();
                }
            }
            catch (TaskCanceledException) { }
            catch (HttpRequestException) { }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Gelbooru] {e.Message}"); }

            return null;
        }
    }
}
