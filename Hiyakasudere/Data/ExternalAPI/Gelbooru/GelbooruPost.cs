using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Hiyakasudere.Data.ExternalAPI.Gelbooru
{
    [XmlRoot(ElementName = "posts")]
    public class GelbooruPosts
    {

        public GelbooruPosts()
        {
            Posts = new List<GelbooruPost>();
        }

        [XmlElement(ElementName = "post")]
        public List<GelbooruPost> Posts { get; set; }

        [XmlAttribute(AttributeName = "limit")]
        public int Limit { get; set; }

        [XmlAttribute(AttributeName = "offset")]
        public int Offset { get; set; }

        [XmlAttribute(AttributeName = "count")]
        public int Count { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "post")]
    public class GelbooruPost
    {

        [XmlElement(ElementName = "id")]
        public int Id { get; set; }

        [XmlElement(ElementName = "created_at")]
        public string CreatedAt { get; set; }

        [XmlElement(ElementName = "score")]
        public int Score { get; set; }

        [XmlElement(ElementName = "width")]
        public int Width { get; set; }

        [XmlElement(ElementName = "height")]
        public int Height { get; set; }

        [XmlElement(ElementName = "md5")]
        public string Md5 { get; set; }

        [XmlElement(ElementName = "directory")]
        public string Directory { get; set; }

        [XmlElement(ElementName = "image")]
        public string Image { get; set; }

        [XmlElement(ElementName = "rating")]
        public string Rating { get; set; }

        [XmlElement(ElementName = "source")]
        public string Source { get; set; }

        [XmlElement(ElementName = "change")]
        public int Change { get; set; }

        [XmlElement(ElementName = "owner")]
        public string Owner { get; set; }

        [XmlElement(ElementName = "creator_id")]
        public int CreatorId { get; set; }

        [XmlElement(ElementName = "parent_id")]
        public int ParentId { get; set; }

        [XmlElement(ElementName = "sample")]
        public int Sample { get; set; }

        [XmlElement(ElementName = "preview_height")]
        public int PreviewHeight { get; set; }

        [XmlElement(ElementName = "preview_width")]
        public int PreviewWidth { get; set; }

        [XmlElement(ElementName = "tags")]
        public string Tags { get; set; }

        [XmlElement(ElementName = "title")]
        public object Title { get; set; }

        [XmlElement(ElementName = "has_notes")]
        public bool HasNotes { get; set; }

        [XmlElement(ElementName = "has_comments")]
        public bool HasComments { get; set; }

        [XmlElement(ElementName = "file_url")]
        public string FileUrl { get; set; }

        [XmlElement(ElementName = "preview_url")]
        public string PreviewUrl { get; set; }

        [XmlElement(ElementName = "sample_url")]
        public string SampleUrl { get; set; }

        [XmlElement(ElementName = "sample_height")]
        public int SampleHeight { get; set; }

        [XmlElement(ElementName = "sample_width")]
        public int SampleWidth { get; set; }

        [XmlElement(ElementName = "status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "post_locked")]
        public int PostLocked { get; set; }

        [XmlElement(ElementName = "has_children")]
        public bool HasChildren { get; set; }
    }

}
