//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Users
{
    public class PagingBlockedUsers : ITwitchPaging
    {
        public int limit,     //max = 100       default = 25
                   offset;

        public PagingBlockedUsers() 
        {
            limit = 25;
            offset = 0;

        }

        public PagingBlockedUsers(int _limit, int _offset)
        {
            limit = _limit;
            offset = _offset;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 25));
            request.AddParameter("offset", offset);

            return request;
        }
    }
}
