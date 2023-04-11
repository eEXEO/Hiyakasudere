using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.Data.Post
{
    public class PostInternal
    {
        public PostInternal(long id, string tags, DateTime createdAt, string author, Uri? source, long score, Uri previewUrl, long previewWidth, long previewHeight, Uri sampleUrl, long sampleWidth, long sampleHeight, Uri originalUrl, long originalWidth, long originalHeight, float originalFileSize, string rating, bool hasChildren)
        {
            Id = id;
            Tags = tags;
            CreatedAt = createdAt;
            Author = author;
            Source = source;
            Score = score;
            SampleUrl = sampleUrl;
            PreviewUrl = previewUrl;
            PreviewWidth = previewWidth;
            PreviewHeight = previewHeight;
            OriginalUrl = originalUrl;
            OriginalWidth = originalWidth;
            OriginalHeight = originalHeight;
            OriginalFileSize = originalFileSize;
            SampleHeight = sampleHeight;
            SampleWidth = sampleHeight;
            Rating = rating;
            HasChildren = hasChildren;
        }

        public long Id { get; set; }

        public int tempId { get; set; }

        public string Tags { get; set; }

        public DateTime CreatedAt { get; set; }

        public long ?UpdatedAt { get; set; }

        public long ?CreatorId { get; set; }

        public object ?ApproverId { get; set; }

        public string Author { get; set; }

        public long ?Change { get; set; }

        public Uri? Source { get; set; }

        public long Score { get; set; }

        public string ?Md5 { get; set; }

        public long FileSize { get; set; }

        public string FileExt { get; set; }

        public Uri FileUrl { get; set; }

        public bool IsShownInIndex { get; set; }

        public Uri PreviewUrl { get; set; }

        public long PreviewWidth { get; set; }

        public long PreviewHeight { get; set; }

        public long ActualPreviewWidth { get; set; }

        public long ActualPreviewHeight { get; set; }

        public Uri SampleUrl { get; set; }

        public long SampleWidth { get; set; }

        public long SampleHeight { get; set; }

        public long SampleFileSize { get; set; }

        public Uri OriginalUrl { get; set; }

        public long OriginalWidth { get; set; }

        public long OriginalHeight { get; set; }

        public float OriginalFileSize { get; set; }

        public string? Rating { get; set; }

        public bool? IsRatingLocked { get; set; }

        public bool? HasChildren { get; set; }

        public object ParentId { get; set; }

        public string Status { get; set; }

        public bool ?IsPending { get; set; }

        public long Width { get; set; }

        public long Height { get; set; }

        public string ?FramesPendingString { get; set; }

        public string ?FramesString { get; set; }
    }
}
