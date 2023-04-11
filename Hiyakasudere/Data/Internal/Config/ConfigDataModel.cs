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
        }
        public ConfigDataModel(int selectedSource, int postsPerPage, bool nSFWEnabled, List<TagInternal> blackListedTags)
        {
            SelectedSource = selectedSource;
            PostsPerPage = postsPerPage;
            NSFWEnabled = nSFWEnabled;
            BlackListedTags = blackListedTags;
        }

        [Required]
        [Range(1, 2,
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

    }
}
