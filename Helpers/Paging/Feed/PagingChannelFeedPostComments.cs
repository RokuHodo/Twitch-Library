//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Feed
{   
    public class PagingChannelFeedPostComments : PagingLimit, ITwitchPaging
    {
        public string cursor;

        public PagingChannelFeedPostComments() : base(10)
        {
            cursor = string.Empty;
        }

        public PagingChannelFeedPostComments(int _limit, string _cursor) : base(10)
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
            
            if(cursor.isValidString())
            {
                request.AddParameter("cursor", cursor);
            }            

            return request;
        }
    }
}
