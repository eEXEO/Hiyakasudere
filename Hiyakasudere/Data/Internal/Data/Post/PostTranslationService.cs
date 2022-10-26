using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.ExternalAPI;
using Hiyakasudere.Data.ExternalAPI.Yandere;
using Hiyakasudere.Data.ExternalAPI.Safebooru;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.Data.Post
{


    public class PostTranslationService : IPostTranslationService
    {
        readonly private IAppConfigService _appConfigService;
        readonly private IYanderePostService _yanderePostService;
        readonly private ISafebooruPostService _safebooruPostService;


        public PostTranslationService(IAppConfigService appConfigService, IYanderePostService yanderePostService, ISafebooruPostService safebooruPostService)
        {
            _appConfigService = appConfigService;
            _yanderePostService = yanderePostService;
            _safebooruPostService = safebooruPostService;
        }

        public async Task<IEnumerable<PostInternal>> GetPostData(int currentPage)
        {
            IEnumerable<PostInternal> posts = null;
            IEnumerable<YanderePost> yanderePosts = null;
            IEnumerable<SafebooruPost> safebooruPosts = null;

            List<PostInternal> temp = new();

            if(_appConfigService.IsLoaded is false)
            {
                await Task.Run(() =>
                {
                    while (_appConfigService.IsLoaded is false)
                    {
                        Task.Delay(25);
                    }

                    return;
                });
            }

            System.Diagnostics.Debug.WriteLine("PostTranslationService: " + _appConfigService.SelectedSource + ", " + _appConfigService.PostsPerPage + ", ", _appConfigService.IsNSFW);

            switch (_appConfigService.SelectedSource)
            {
                case 1:
                    yanderePosts = await _yanderePostService.GetYandereData(_yanderePostService.GenerateRequestURL(_appConfigService.PostsPerPage, currentPage));

                    foreach (YanderePost element in yanderePosts)
                    {
                        //temp.Add(new PostInternal(element.Id, element.Tags, element.CreatedAt.ToString(), element.Author, element.Source, element.Score, element.PreviewUrl, element.PreviewWidth,
                        //element.PreviewHeight, element.JpegUrl, element.Width, element.Height, element.FileSize, element.Rating, element.HasChildren, element.SampleUrl));
                        temp.Add(new PostInternal(element.Id, element.Tags, "", element.Author, element.Source, 
                            element.Score, element.PreviewUrl, element.PreviewWidth, element.PreviewHeight, element.SampleUrl,
                            element.SampleWidth, element.SampleHeight, element.FileUrl, element.Width, element.Height,
                            element.FileSize, element.Rating, element.HasChildren));
                    }

                    posts = temp.AsEnumerable<PostInternal>();
                    break;

                case 2://This is visible pain
                    safebooruPosts = await _safebooruPostService.GetSafebooruData(_safebooruPostService.GenerateRequestURL(_appConfigService.PostsPerPage, currentPage));
                   
                    Uri source = null;
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
                        }catch(Exception)
                        {
                            System.Diagnostics.Debug.WriteLine("Everything is fine!");
                        }

                        temp.Add(new PostInternal(loId, element.Tags, element.Created_at, element.Creator_id, source,
                            loScore, new Uri(element.Preview_url), loPrWidth, loPrHeight, new Uri(element.Sample_url),
                            loSaWidth, loSaHeight, new Uri(element.File_url), loWidth, loHeight, null,
                            element.Rating, loHasChildren));

                        //temp.Add(new PostInternal(loId, element.Tags, element.Created_at, element.Creator_id, source, loScore, 
                        //    new System.Uri(element.Preview_url), loPrWidth, loPrHeight, new Uri(element.File_url),
                        //    loWidth, loHeight, null, element.Rating, loHasChildren));
                    }

                    posts = temp.AsEnumerable<PostInternal>();
                    break;
                default:
                    break;
            }

            return posts;
        }

    }
}
