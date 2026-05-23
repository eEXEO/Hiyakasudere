using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hiyakasudere.Data.ExternalAPI.Yandere
{
    public partial class YandereTag
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("ambiguous")]
        public bool Ambiguous { get; set; }
    }
}