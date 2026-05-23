using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.Data.Post
{
    public class TagInternal
    {
        public TagInternal(long id, string name, long count, long type, bool ambiguous)
        {
            Id = id;
            Name = name;
            Count = count;
            Type = type;
            Ambiguous = ambiguous;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public long Count { get; set; }

        public long Type { get; set; }

        public bool Ambiguous { get; set; }
    }
}
