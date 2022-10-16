using System.Xml.Serialization;

namespace Hiyakasudere.Data.ExternalAPI.Safebooru
{
    [XmlRoot(ElementName = "posts")]
    public class SafebooruPosts
    {
        public SafebooruPosts()
        {
            Posts = new List<SafebooruPost>();
        }

        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
        [XmlAttribute(AttributeName = "offset")]
        public string Offset { get; set; }
        [XmlElement(ElementName = "post")]
        public List<SafebooruPost> Posts { get; set; }
    }
    
    public class SafebooruPost
    {
        [XmlAttribute(AttributeName = "height")]
        public string Height { get; set; }
        [XmlAttribute(AttributeName = "score")]
        public string Score { get; set; }
        [XmlAttribute(AttributeName = "file_url")]
        public string File_url { get; set; }
        [XmlAttribute(AttributeName = "parent_id")]
        public string Parent_id { get; set; }
        [XmlAttribute(AttributeName = "sample_url")]
        public string Sample_url { get; set; }
        [XmlAttribute(AttributeName = "sample_width")]
        public string Sample_width { get; set; }
        [XmlAttribute(AttributeName = "sample_height")]
        public string Sample_height { get; set; }
        [XmlAttribute(AttributeName = "preview_url")]
        public string Preview_url { get; set; }
        [XmlAttribute(AttributeName = "rating")]
        public string Rating { get; set; }
        [XmlAttribute(AttributeName = "tags")]
        public string Tags { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "width")]
        public string Width { get; set; }
        [XmlAttribute(AttributeName = "change")]
        public string Change { get; set; }
        [XmlAttribute(AttributeName = "md5")]
        public string Md5 { get; set; }
        [XmlAttribute(AttributeName = "creator_id")]
        public string Creator_id { get; set; }
        [XmlAttribute(AttributeName = "has_children")]
        public string Has_children { get; set; }
        [XmlAttribute(AttributeName = "created_at")]
        public string Created_at { get; set; }
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "has_notes")]
        public string Has_notes { get; set; }
        [XmlAttribute(AttributeName = "has_comments")]
        public string Has_comments { get; set; }
        [XmlAttribute(AttributeName = "preview_width")]
        public string Preview_width { get; set; }
        [XmlAttribute(AttributeName = "preview_height")]
        public string Preview_height { get; set; }
    }
}
