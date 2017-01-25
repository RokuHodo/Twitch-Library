//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Feed
{   
    public class PagingChannelFeedPostComments : ITwitchPaging
    {
        public long limit;      //max = 100         default = 10                   

        public string cursor;

        public PagingChannelFeedPostComments()
        {
            limit = 10;
            cursor = string.Empty;
        }

        public PagingChannelFeedPostComments(long _limit, string _cursor)
        {
            limit = _limit;            
            cursor = _cursor;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 10));            
            request.AddParameter("cursor", cursor);

            return request;
        }
    }
}
