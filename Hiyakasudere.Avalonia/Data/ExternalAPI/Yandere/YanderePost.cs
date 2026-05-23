using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hiyakasudere.Data.ExternalAPI.Yandere
{
    public partial class YanderePost
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public long UpdatedAt { get; set; }

        [JsonProperty("creator_id")]
        public long? CreatorId { get; set; }

        [JsonProperty("approver_id")]
        public object ApproverId { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("change")]
        public long Change { get; set; }

        [JsonProperty("source")]
        public Uri Source { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        [JsonProperty("file_size")]
        public long FileSize { get; set; }

        [JsonProperty("file_ext")]
        public string FileExt { get; set; }

        [JsonProperty("file_url")]
        public Uri FileUrl { get; set; }

        [JsonProperty("is_shown_in_index")]
        public bool IsShownInIndex { get; set; }

        [JsonProperty("preview_url")]
        public Uri PreviewUrl { get; set; }

        [JsonProperty("preview_width")]
        public long PreviewWidth { get; set; }

        [JsonProperty("preview_height")]
        public long PreviewHeight { get; set; }

        [JsonProperty("actual_preview_width")]
        public long ActualPreviewWidth { get; set; }

        [JsonProperty("actual_preview_height")]
        public long ActualPreviewHeight { get; set; }

        [JsonProperty("sample_url")]
        public Uri SampleUrl { get; set; }

        [JsonProperty("sample_width")]
        public long SampleWidth { get; set; }

        [JsonProperty("sample_height")]
        public long SampleHeight { get; set; }

        [JsonProperty("sample_file_size")]
        public long SampleFileSize { get; set; }

        [JsonProperty("jpeg_url")]
        public Uri JpegUrl { get; set; }

        [JsonProperty("jpeg_width")]
        public long JpegWidth { get; set; }

        [JsonProperty("jpeg_height")]
        public long JpegHeight { get; set; }

        [JsonProperty("jpeg_file_size")]
        public long JpegFileSize { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("is_rating_locked")]
        public bool IsRatingLocked { get; set; }

        [JsonProperty("has_children")]
        public bool HasChildren { get; set; }

        [JsonProperty("parent_id")]
        public object ParentId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("is_pending")]
        public bool IsPending { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("is_held")]
        public bool IsHeld { get; set; }

        [JsonProperty("frames_pending_string")]
        public string FramesPendingString { get; set; }

        [JsonProperty("frames_pending")]
        public List<object> FramesPending { get; set; }

        [JsonProperty("frames_string")]
        public string FramesString { get; set; }

        [JsonProperty("frames")]
        public List<object> Frames { get; set; }

        [JsonProperty("is_note_locked")]
        public bool IsNoteLocked { get; set; }

        [JsonProperty("last_noted_at")]
        public long LastNotedAt { get; set; }

        [JsonProperty("last_commented_at")]
        public long LastCommentedAt { get; set; }
    }
}