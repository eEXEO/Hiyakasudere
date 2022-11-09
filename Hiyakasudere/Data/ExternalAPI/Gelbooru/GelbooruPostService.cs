using Hiyakasudere.Data.ExternalAPI.Gelbooru;
using Hiyakasudere.Data.Internal.Config;
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
            int safebooruPostCount = 0;

            var req = "https://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=0&tags=";

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
