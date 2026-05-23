using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Hiyakasudere.Data.ExternalAPI.Rule34
{
    [XmlRoot(ElementName = "post")]
    public class Rule34Post
    {

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }

        [XmlAttribute(AttributeName = "score")]
        public int Score { get; set; }

        [XmlAttribute(AttributeName = "file_url")]
        public string FileUrl { get; set; }

        [XmlAttribute(AttributeName = "parent_id")]
        public string ParentId { get; set; }

        [XmlAttribute(AttributeName = "sample_url")]
        public string SampleUrl { get; set; }

        [XmlAttribute(AttributeName = "sample_width")]
        public int SampleWidth { get; set; }

        [XmlAttribute(AttributeName = "sample_height")]
        public int SampleHeight { get; set; }

        [XmlAttribute(AttributeName = "preview_url")]
        public string PreviewUrl { get; set; }

        [XmlAttribute(AttributeName = "rating")]
        public string Rating { get; set; }

        [XmlAttribute(AttributeName = "tags")]
        public string Tags { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        [XmlAttribute(AttributeName = "change")]
        public int Change { get; set; }

        [XmlAttribute(AttributeName = "md5")]
        public string Md5 { get; set; }

        [XmlAttribute(AttributeName = "creator_id")]
        public int CreatorId { get; set; }

        [XmlAttribute(AttributeName = "has_children")]
        public bool HasChildren { get; set; }

        [XmlAttribute(AttributeName = "created_at")]
        public string CreatedAt { get; set; }

        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }

        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "has_notes")]
        public bool HasNotes { get; set; }

        [XmlAttribute(AttributeName = "has_comments")]
        public bool HasComments { get; set; }

        [XmlAttribute(AttributeName = "preview_width")]
        public int PreviewWidth { get; set; }

        [XmlAttribute(AttributeName = "preview_height")]
        public int PreviewHeight { get; set; }
    }

    [XmlRoot(ElementName = "posts")]
    public class Rule34Posts
    {

        [XmlElement(ElementName = "post")]
        public List<Rule34Post> Posts { get; set; }

        [XmlAttribute(AttributeName = "count")]
        public int Count { get; set; }

        [XmlAttribute(AttributeName = "offset")]
        public int Offset { get; set; }
    }

}
