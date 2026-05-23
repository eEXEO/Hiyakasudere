using System.ComponentModel.DataAnnotations;

namespace Hiyakasudere.Data.Internal.Database
{
    public class FavoritePost
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Post ID from the booru source</summary>
        public long SourcePostId { get; set; }

        /// <summary>Source identifier (1=Yandere, 2=Safebooru, 3=Konachan, 4=Gelbooru, 5=Rule34)</summary>
        public int SourceId { get; set; }

        /// <summary>Preview/thumbnail URL for display in favorites grid</summary>
        public string PreviewUrl { get; set; } = "";

        /// <summary>Sample (medium) image URL</summary>
        public string SampleUrl { get; set; } = "";

        /// <summary>Original full-resolution URL</summary>
        public string OriginalUrl { get; set; } = "";

        /// <summary>Space-separated tags string</summary>
        public string Tags { get; set; } = "";

        /// <summary>Content rating (Safe, Questionable, Explicit)</summary>
        public string Rating { get; set; } = "";

        /// <summary>Score/votes from the booru</summary>
        public long Score { get; set; }

        /// <summary>Image width</summary>
        public long Width { get; set; }

        /// <summary>Image height</summary>
        public long Height { get; set; }

        /// <summary>When this was added to favorites</summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
