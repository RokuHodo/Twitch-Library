//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Feed
{   
    public class PagingChannelFeedPosts : ITwitchPaging
    {
        public long limit,          //max = 100         default = 10
                    comments;       //max = 5           default = 5

        public string cursor;

        public PagingChannelFeedPosts()
        {
            limit = 10;     
            comments = 5;
            cursor = string.Empty;
        }

        public PagingChannelFeedPosts(long _limit, long _comments, string _cursor)
        {
            limit = _limit;
            comments = _comments;
            cursor = _cursor;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 10));
            request.AddParameter("comments", comments.Clamp(0, 5, 5));
            request.AddParameter("cursor", cursor);

            return request;
        }
    }
}
