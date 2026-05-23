using Hiyakasudere.Data.ExternalAPI.Gelbooru;
using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.Data.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hiyakasudere.Data.ExternalAPI.Gelbooru
{
    public class GelbooruPostService : IGelbooruPostService
    {
        HttpClient client;
        XDocument xDocument;
        IAppConfigService _appConfigService;
        XmlSerializer serializer;

        public GelbooruPostService(IAppConfigService appConfigService)
        {
            client = new HttpClient();
            xDocument = new XDocument();
            _appConfigService = appConfigService;
            serializer = new XmlSerializer(typeof(GelbooruPosts));
        }

        public string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags)
        {
            currentPage -= 1;
            string requestUri = "https://gelbooru.com/index.php?page=dapi&s=post&q=index";

            // Add API credentials (required since 2025)
            if (!string.IsNullOrEmpty(_appConfigService.GelbooruApiKey) && !string.IsNullOrEmpty(_appConfigService.GelbooruUserId))
            {
                requestUri += $"&api_key={_appConfigService.GelbooruApiKey}";
                requestUri += $"&user_id={_appConfigService.GelbooruUserId}";
            }

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

        public async Task<int> GetGelbooruPostCount(List<string> tags)
        {
            int gelbooruPostCount = 0;

            var req = "https://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=0";

            // Add API credentials (required since 2025)
            if (!string.IsNullOrEmpty(_appConfigService.GelbooruApiKey) && !string.IsNullOrEmpty(_appConfigService.GelbooruUserId))
            {
                req += $"&api_key={_appConfigService.GelbooruApiKey}";
                req += $"&user_id={_appConfigService.GelbooruUserId}";
            }

            req += "&tags=";

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
                    gelbooruPostCount = int.Parse(xDocument.Root.Attribute("count").Value);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }

            return gelbooruPostCount;
        }

        public async Task<IEnumerable<TagInternal>> GetTagsAutocompletion(string partialTag)
        {
            List<TagInternal> results = new();

            try
            {
                var req = $"https://gelbooru.com/index.php?page=dapi&s=tag&q=index&limit=10&name_pattern=%25{partialTag}%25";

                // Add API credentials (required since 2025)
                if (!string.IsNullOrEmpty(_appConfigService.GelbooruApiKey) && !string.IsNullOrEmpty(_appConfigService.GelbooruUserId))
                {
                    req += $"&api_key={_appConfigService.GelbooruApiKey}";
                    req += $"&user_id={_appConfigService.GelbooruUserId}";
                }

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

        public async Task<IEnumerable<GelbooruPost>> GetGelbooruData(string request)
        {
            IEnumerable<GelbooruPost> gelboruPost = null;
            var members = (GelbooruPosts)null;

            try
            {
                var response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Request: " + request);

                    members = (GelbooruPosts)serializer.Deserialize(await response.Content.ReadAsStreamAsync());

                    gelboruPost = members.Posts.AsEnumerable<GelbooruPost>();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }

            return gelboruPost;
        }
    }
}
