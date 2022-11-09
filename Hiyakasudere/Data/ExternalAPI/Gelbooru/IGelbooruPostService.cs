using Hiyakasudere.Data.ExternalAPI.Gelbooru;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.ExternalAPI.Gelbooru
{
    public interface IGelbooruPostService
    {
        string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags);
        Task<IEnumerable<GelbooruPost>> GetGelbooruData(string request);
        Task<int> GetGelbooruPostCount(List<string> tags);
    }
}
