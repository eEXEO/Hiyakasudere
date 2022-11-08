using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.ExternalAPI;
using Hiyakasudere.Data.ExternalAPI.Yandere;
using Hiyakasudere.Data.ExternalAPI.Safebooru;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using System.Globalization;
using Hiyakasudere.Data.ExternalAPI.Konachan;

namespace Hiyakasudere.Data.Internal.Data.Post
{
    public class PostTranslationService : IPostTranslationService
    {
        readonly private IAppConfigService _appConfigService;
        readonly private IYanderePostService _yanderePostService;
        readonly private ISafebooruPostService _safebooruPostService;
        readonly private IKonachanPostService _konachanPostService;

        private List<TagInternal> tags = new();


        public PostTranslationService(IAppConfigService appConfigService, IYanderePostService yanderePostService, 
            ISafebooruPostService safebooruPostService, IKonachanPostService konachanPostService)
        {
            _appConfigService = appConfigService;
            _yanderePostService = yanderePostService;
            _safebooruPostService = safebooruPostService;
            _konachanPostService = konachanPostService;
        }

        protected List<string> GetSimplefiedTags()
        {
            List<string> temp = new();

            temp.Add("");

            try
            {
                foreach (TagInternal tag in tags)
                {
                    temp.Add(tag.Name);
                }
            }
            catch (Exception)
            {

            }

            return temp;
        }

        public async Task<int> GetPageCount()
        {
            var count = 1;

            
            switch (_appConfigService.SelectedSource)
            {
                case 1:
                    count = await _yanderePostService.GetYanderePostCount(GetSimplefiedTags());
                    break;
                case 2:
                    count = await _safebooruPostService.GetSafebooruPostCount(GetSimplefiedTags());
                    break;
                case 3:
                    count = await _konachanPostService.GetKonachanPostCount(GetSimplefiedTags());
                    break;
            }

            return count / _appConfigService.PostsPerPage;
        }

        public void UpdateTags(List<TagInternal> tags)
        {
            this.tags = tags;

            foreach (TagInternal tag in tags)
            {
                Debug.WriteLine(tag.Name);
            }
        }

        public List<TagInternal> GetTags()
        {
            return tags;
        }

        public async Task<IEnumerable<PostInternal>> GetPostData(int currentPage)
        {
            IEnumerable<PostInternal> posts = null;
            IEnumerable<YanderePost> yanderePosts = null;
            IEnumerable<SafebooruPost> safebooruPosts = null;
            IEnumerable<KonachanPost> konachanPosts = null;

            //temp vars
            DateTime loDate = new DateTime();

            List<PostInternal> temp = new();

            if(_appConfigService.IsLoaded is false)
            {
                await Task.Run(() =>
                {
                    while (_appConfigService.IsLoaded is false)
                    {
                        Task.Delay(50);
                    }

                    return;
                });
            }

            switch (_appConfigService.SelectedSource)
            {
                case 1:
                    yanderePosts = await _yanderePostService.GetYandereData(_yanderePostService.GenerateRequestURL(_appConfigService.PostsPerPage, currentPage, GetSimplefiedTags(), _appConfigService.GetSimplifiedBlackTags()));

                    loDate = new DateTime();

                    foreach (YanderePost element in yanderePosts)
                    {
                        //(first letter from - safe, questionable, explict)
                        if (_appConfigService.IsNSFW is false && element.Rating.Equals("e") || _appConfigService.IsNSFW is false && element.Rating.Equals("q")) continue;

                        //Translate rating value
                        if (element.Rating.Equals("e"))
                        {
                            element.Rating = "Explict";
                        }
                        else if (element.Rating.Equals("q"))
                        {
                            element.Rating = "Questionable";
                        }
                        else if (element.Rating.Equals("s"))
                        {
                            element.Rating = "Safe";
                        }

                        loDate = DateTimeOffset.FromUnixTimeSeconds(element.CreatedAt).UtcDateTime;

                        temp.Add(new PostInternal(element.Id, element.Tags, loDate, element.Author, element.Source, 
                            element.Score, element.PreviewUrl, element.PreviewWidth, element.PreviewHeight, element.SampleUrl,
                            element.SampleWidth, element.SampleHeight, element.FileUrl, element.Width, element.Height,
                            element.FileSize / 1048576.0f, element.Rating, element.HasChildren));
                    }

                    posts = temp.AsEnumerable<PostInternal>();
                    break;

                case 2:
                    //This is visible pain
                    safebooruPosts = await _safebooruPostService.GetSafebooruData(_safebooruPostService.GenerateRequestURL(_appConfigService.PostsPerPage, currentPage, GetSimplefiedTags(), _appConfigService.GetSimplifiedBlackTags()));
                   
                    Uri source = null;
                    loDate = new DateTime();
                    long loId = 0;
                    long loScore = 0;
                    long loPrWidth = 0;
                    long loPrHeight = 0;
                    long loSaWidth = 0;
                    long loSaHeight = 0;
                    long loWidth = 0;
                    long loHeight = 0;
                    bool loHasChildren = false;

                    foreach (SafebooruPost element in safebooruPosts)
                    {
                        //This source is safe for viewer, so no precautions taken
                        //API is inconsistent FFS
                        source = null;
                        try
                        {
                            Uri.TryCreate(element.Source, UriKind.RelativeOrAbsolute, out source);
                            loId = long.Parse(element.Id);
                            loScore = long.Parse(element.Score);
                            loSaWidth = long.Parse(element.Sample_width);
                            loSaHeight = long.Parse(element.Sample_height);
                            loPrWidth = long.Parse(element.Preview_width);
                            loPrHeight = long.Parse(element.Preview_height);
                            loWidth = long.Parse(element.Width);
                            loHeight = long.Parse(element.Height);
                            loHasChildren = bool.Parse(element.Has_children);

                            //Translate rating value
                            if (element.Rating.Equals("q"))
                            {
                                element.Rating = "General";
                            }
                            else if (element.Rating.Equals("s"))
                            {
                                element.Rating = "Safe";
                            }

                            //Parse date
                            element.Created_at = element.Created_at.Insert(element.Created_at.IndexOf("+") + 3, ":");
                            loDate = DateTime.ParseExact(element.Created_at, "ddd MMM dd H:mm:ss zzz yyyy", CultureInfo.GetCultureInfo("en-US"));

                        }
                        catch(Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e);
                            System.Diagnostics.Debug.WriteLine("Everything is fine!");
                        }

                        temp.Add(new PostInternal(loId, element.Tags, loDate, element.Creator_id, source,
                            loScore, new Uri(element.Preview_url), loPrWidth, loPrHeight, new Uri(element.Sample_url),
                            loSaWidth, loSaHeight, new Uri(element.File_url), loWidth, loHeight, 0f,
                            element.Rating, loHasChildren));
                    }

                    posts = temp.AsEnumerable<PostInternal>();
                    break;

                case 3:
                    konachanPosts = await _konachanPostService.GetKonachanData(_konachanPostService.GenerateRequestURL(_appConfigService.PostsPerPage, currentPage, GetSimplefiedTags(), _appConfigService.GetSimplifiedBlackTags()));

                    loDate = new DateTime();

                    foreach (KonachanPost element in konachanPosts)
                    {
                        //(first letter from - safe, questionable, explict)
                        if (_appConfigService.IsNSFW is false && element.Rating.Equals("e") || _appConfigService.IsNSFW is false && element.Rating.Equals("q")) continue;

                        //Translate rating value
                        if (element.Rating.Equals("e"))
                        {
                            element.Rating = "Explict";
                        }
                        else if (element.Rating.Equals("q"))
                        {
                            element.Rating = "Questionable";
                        }
                        else if (element.Rating.Equals("s"))
                        {
                            element.Rating = "Safe";
                        }

                        loDate = DateTimeOffset.FromUnixTimeSeconds(element.CreatedAt).UtcDateTime;

                        temp.Add(new PostInternal(element.Id, element.Tags, loDate, element.Author, element.Source,
                            element.Score, element.PreviewUrl, element.PreviewWidth, element.PreviewHeight, element.SampleUrl,
                            element.SampleWidth, element.SampleHeight, element.FileUrl, element.Width, element.Height,
                            element.FileSize / 1048576.0f, element.Rating, element.HasChildren));
                    }

                    posts = temp.AsEnumerable<PostInternal>();
                    break;
                default:
                    break;
            }

            return posts;
        }

        public async Task<IEnumerable<TagInternal>> TryAutocompleteTag(string partialTag)
        {
            List<TagInternal> tagInternal = new();

            var yandereTags = await _yanderePostService.GetTagsAutocompletion(partialTag);

            tagInternal.Add(new TagInternal(0, partialTag, 1, 1, false));
            foreach (YandereTag tag in yandereTags)
            {
                tagInternal.Add(new TagInternal(tag.Id, tag.Name, tag.Count, tag.Type, tag.Ambiguous));
            }
            return tagInternal.AsEnumerable(); 
        }
    }
}
