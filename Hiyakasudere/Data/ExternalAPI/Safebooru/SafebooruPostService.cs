using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.Data.Post;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hiyakasudere.Data.ExternalAPI.Safebooru
{
    public class SafebooruPostService : ISafebooruPostService
    {
        HttpClient client;
        XDocument xDocument;
        IAppConfigService _appConfigService;
        XmlSerializer serializer;

        public SafebooruPostService(IAppConfigService appConfigService)
        {
            client = new HttpClient();
            xDocument = new XDocument();
            _appConfigService = appConfigService;
            serializer = new XmlSerializer(typeof(SafebooruPosts));
        }

        public string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags)
        {
            currentPage -= 1;
            string requestUri = "https://safebooru.org/index.php?page=dapi&s=post&q=index";
            requestUri += $"&limit={postsPerPage}";
            requestUri += $"&pid={currentPage}";
            requestUri += "&tags=";

            if (tags.Any())
            {
                foreach (var tag in tags)
                {
                    requestUri += tag.ToString() + " ";
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

        public async Task<int> GetSafebooruPostCount(List<string> tags)
        {
            int safebooruPostCount = 0;

            var req = "https://safebooru.org/index.php?page=dapi&s=post&q=index&limit=0&tags=";

            if (tags.Any())
            {
                foreach (var tag in tags)
                {
                    req += tag.ToString() + " ";
                }
            }

            try
            {
                var response = await client.GetAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    xDocument = XDocument.Parse(content);
                    safebooruPostCount = int.Parse(xDocument.Root.Attribute("count").Value);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }

            return safebooruPostCount;
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
                        var count = long.TryParse(tag.Attribute("count")?.Value, out var c) ? c : 0;
                        var type = long.TryParse(tag.Attribute("type")?.Value, out var t) ? t : 0;
                        var id = long.TryParse(tag.Attribute("id")?.Value, out var i) ? i : 0;

                        results.Add(new TagInternal(id, name, count, type, false));
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return results;
        }

        public async Task<IEnumerable<SafebooruPost>> GetSafebooruData(string request)
        {
            IEnumerable<SafebooruPost> safebooruPost = null;
            var members = (SafebooruPosts)null;

            try
            {
                var response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Request: " + request);

                    members = (SafebooruPosts)serializer.Deserialize(await response.Content.ReadAsStreamAsync());

                    safebooruPost = members.Posts.AsEnumerable<SafebooruPost>();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }

            return safebooruPost;
        }
    }
}
