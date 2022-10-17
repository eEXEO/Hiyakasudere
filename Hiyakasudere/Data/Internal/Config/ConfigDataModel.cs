using System.ComponentModel.DataAnnotations;

namespace Hiyakasudere.Data.Internal.Config
{
    public class ConfigDataModel
    {
        public ConfigDataModel(int selectedSource, int postsPerPage, bool nSFWEnabled)
        {
            SelectedSource = selectedSource;
            PostsPerPage = postsPerPage;
            NSFWEnabled = nSFWEnabled;
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

    }
}
