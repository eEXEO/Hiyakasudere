using System.ComponentModel.DataAnnotations;

namespace Hiyakasudere.Data.Internal.Database
{
    public class SearchHistoryEntry
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Comma-separated tags that were searched</summary>
        public string Tags { get; set; } = "";

        /// <summary>Source used at time of search (1-5)</summary>
        public int SourceId { get; set; }

        /// <summary>When this search was performed</summary>
        public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
    }
}
