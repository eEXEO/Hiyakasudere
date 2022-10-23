using System.ComponentModel.DataAnnotations;

namespace Hiyakasudere.Data.Internal.Config
{
    public class ConfigDataModel
    {
        public ConfigDataModel()
        {
            SelectedSource = 1;
            PostsPerPage = 12;
            NSFWEnabled = false;
            ImageSavePath = "";
        }
        public ConfigDataModel(int selectedSource, int postsPerPage, bool nSFWEnabled, string imageSavePath)
        {
            SelectedSource = selectedSource;
            PostsPerPage = postsPerPage;
            NSFWEnabled = nSFWEnabled;
            ImageSavePath = imageSavePath;
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
        public string ImageSavePath { get; set; }

    }
}
