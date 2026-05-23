using System.ComponentModel.DataAnnotations;
using Hiyakasudere.Data.Internal.Data.Post;

namespace Hiyakasudere.Data.Internal.Config
{
    public class ConfigDataModel
    {
        public ConfigDataModel()
        {
            SelectedSource = 1;
            PostsPerPage = 15;
            NSFWEnabled = false;
            BlackListedTags = new();
            GelbooruApiKey = "";
            GelbooruUserId = "";
            Rule34ApiKey = "";
            Rule34UserId = "";
        }
        public ConfigDataModel(int selectedSource, int postsPerPage, bool nSFWEnabled, List<TagInternal> blackListedTags)
        {
            SelectedSource = selectedSource;
            PostsPerPage = postsPerPage;
            NSFWEnabled = nSFWEnabled;
            BlackListedTags = blackListedTags;
            GelbooruApiKey = "";
            GelbooruUserId = "";
            Rule34ApiKey = "";
            Rule34UserId = "";
        }

        [Required]
        [Range(1, 5,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int SelectedSource { get; set; }

        [Range(6, 48,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required]
        public int PostsPerPage { get; set; }

        [Required]
        public bool NSFWEnabled { get; set; }

        [Required]
        public List<TagInternal> BlackListedTags { get; set; }

        /// <summary>Gelbooru API key (get from gelbooru.com account settings)</summary>
        public string GelbooruApiKey { get; set; }

        /// <summary>Gelbooru User ID (get from gelbooru.com account settings)</summary>
        public string GelbooruUserId { get; set; }

        /// <summary>Rule34 API key (get from rule34.xxx account)</summary>
        public string Rule34ApiKey { get; set; }

        /// <summary>Rule34 User ID (get from rule34.xxx account)</summary>
        public string Rule34UserId { get; set; }
    }
}
