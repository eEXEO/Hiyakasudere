using Hiyakasudere.Data.ExternalAPI.Konachan;
using Hiyakasudere.Data.ExternalAPI.Safebooru;
using Hiyakasudere.Data.Internal.Config;
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
            requestUri += $"&limit={postsPerPage}";
            requestUri += $"&pid={currentPage}";
            requestUri += "&tags=rating:s+";

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

            var req = "https://api.rule34.xxx/index.php?page=dapi&s=post&q=index&limit=0&tags=rating:s+";

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
