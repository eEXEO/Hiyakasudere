using Hiyakasudere.Data.ExternalAPI.Konachan;
using Hiyakasudere.Data.ExternalAPI.Safebooru;
using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.Data.Post;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hiyakasudere.Data.ExternalAPI.Rule34
{
    public class Rule34PostService : IRule34PostService
    {
        HttpClient client;
        XDocument xDocument;
        IAppConfigService _appConfigService;
        XmlSerializer serializer;

        public Rule34PostService(IAppConfigService appConfigService)
        {
            client = new HttpClient();
            xDocument = new XDocument();
            _appConfigService = appConfigService;
            serializer = new XmlSerializer(typeof(Rule34Posts));
        }

        public string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags)
        {
            currentPage -= 1;
            string requestUri = "https://api.rule34.xxx/index.php?page=dapi&s=post&q=index";

            // Add API credentials (required since 2025)
            if (!string.IsNullOrEmpty(_appConfigService.Rule34ApiKey) && !string.IsNullOrEmpty(_appConfigService.Rule34UserId))
            {
                requestUri += $"&api_key={_appConfigService.Rule34ApiKey}";
                requestUri += $"&user_id={_appConfigService.Rule34UserId}";
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

        public async Task<int> GetRule34PostCount(List<string> tags)
        {
            int rule34PostCount = 0;

            var req = "https://api.rule34.xxx/index.php?page=dapi&s=post&q=index&limit=0";

            // Add API credentials (required since 2025)
            if (!string.IsNullOrEmpty(_appConfigService.Rule34ApiKey) && !string.IsNullOrEmpty(_appConfigService.Rule34UserId))
            {
                req += $"&api_key={_appConfigService.Rule34ApiKey}";
                req += $"&user_id={_appConfigService.Rule34UserId}";
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
                    rule34PostCount = int.Parse(xDocument.Root.Attribute("count").Value);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }

            return rule34PostCount;
        }

        public async Task<IEnumerable<TagInternal>> GetTagsAutocompletion(string partialTag)
        {
            List<TagInternal> results = new();

            try
            {
                var req = $"https://api.rule34.xxx/index.php?page=dapi&s=tag&q=index&limit=10&name_pattern=%25{partialTag}%25";

                // Add API credentials (required since 2025)
                if (!string.IsNullOrEmpty(_appConfigService.Rule34ApiKey) && !string.IsNullOrEmpty(_appConfigService.Rule34UserId))
                {
                    req += $"&api_key={_appConfigService.Rule34ApiKey}";
                    req += $"&user_id={_appConfigService.Rule34UserId}";
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

        public async Task<IEnumerable<Rule34Post>> GetRule34Data(string request)
        {
            IEnumerable<Rule34Post> rulePost = null;
            var members = (Rule34Posts)null;

            try
            {
                var response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Request: " + request);

                    members = (Rule34Posts)serializer.Deserialize(await response.Content.ReadAsStreamAsync());

                    rulePost = members.Posts.AsEnumerable<Rule34Post>();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }

            return rulePost;
        }
    }
}
