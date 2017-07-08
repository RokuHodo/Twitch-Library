// project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Interfaces.Helpers.Paging;

// imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Videos
{
    public class PagingUserFollowsVideos : PagingLimitOffset, ITwitchPaging
    {
        public BroadcastType[] broadcast_type;

        public PagingUserFollowsVideos() : base(10) 
        {
            broadcast_type = new BroadcastType[] { BroadcastType.HIGHLIGHT };            
        }

        public PagingUserFollowsVideos(int _limit, int _offset, BroadcastType[] _broadcast_type) : base(10)
        {
            limit = _limit;
            offset = _offset;            
            broadcast_type = _broadcast_type;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddQueryParameter("limit", limit.ToString());
            request.AddQueryParameter("offset", offset.ToString());
            request.AddQueryParameter("broadcast_type", string.Join(",", broadcast_type).ToLower());

            return request;
        }
    }
}
