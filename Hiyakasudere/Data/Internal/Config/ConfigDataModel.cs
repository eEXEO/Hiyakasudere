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
        public int SelectedSource { get; set; }

        [Required]
        public int PostsPerPage { get; set; }

        [Required]
        public bool NSFWEnabled { get; set; }

    }
}
