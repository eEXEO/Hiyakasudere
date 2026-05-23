using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.Data.Post;
using Hiyakasudere.Data.Internal.Functionality;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hiyakasudere.Data.ExternalAPI.Safebooru
{
    public class SafebooruPostService : ISafebooruPostService
    {
        HttpClient client = HttpClientProvider.Client;
        IAppConfigService _appConfigService;
        XmlSerializer serializer;

        public SafebooruPostService(IAppConfigService appConfigService)
        {
            _appConfigService = appConfigService;
            serializer = new XmlSerializer(typeof(SafebooruPosts));
        }

        public string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags)
        {
            currentPage -= 1;
            string requestUri = $"https://safebooru.org/index.php?page=dapi&s=post&q=index&limit={postsPerPage}&pid={currentPage}&tags=";

            foreach (var tag in tags.Where(t => !string.IsNullOrEmpty(t)))
                requestUri += tag + " ";

            foreach (var tag in blackTags.Where(t => !string.IsNullOrEmpty(t)))
                requestUri += "-" + tag + "+";

            return requestUri;
        }

        public async Task<int> GetSafebooruPostCount(List<string> tags)
        {
            try
            {
                var req = "https://safebooru.org/index.php?page=dapi&s=post&q=index&limit=0&tags=";
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
            catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Safebooru Count] {e.Message}"); }

            return 0;
        }

        public async Task<IEnumerable<TagInternal>> GetTagsAutocompletion(string partialTag)
        {
            List<TagInternal> results = new();
            try
            {
                var req = $"https://safebooru.org/index.php?page=dapi&s=tag&q=index&limit=10&name_pattern=%25{partialTag}%25";
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
            catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Safebooru Tags] {e.Message}"); }

            return results;
        }

        public async Task<IEnumerable<SafebooruPost>> GetSafebooruData(string request)
        {
            try
            {
                var response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var members = (SafebooruPosts)serializer.Deserialize(await response.Content.ReadAsStreamAsync());
                    return members?.Posts?.AsEnumerable<SafebooruPost>();
                }
            }
            catch (TaskCanceledException) { }
            catch (HttpRequestException) { }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine($"[Safebooru] {e.Message}"); }

            return null;
        }
    }
}
