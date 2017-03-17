
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Communities
{
    public class PagingBannedCommunityUsers : PagingLimit, ITwitchPaging
    {
        public string cursor;

        public PagingBannedCommunityUsers() : base(10)
        {            
            cursor = string.Empty;
        }

        public PagingBannedCommunityUsers(int _limit, string _cursor) : base(10)
        {
            limit = _limit;
            cursor = _cursor;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit);

            if (cursor.isValidString())
            {
                request.AddParameter("cursor", cursor);
            }

            return request;
        }
    }
}
