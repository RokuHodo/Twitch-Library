//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Videos
{
    public class PagingUserFollowsVideos : ITwitchPaging
    {
        public int limit,      //max = 100      default = 10
                   offset;

        public BroadcastType[] broadcast_type;

        public PagingUserFollowsVideos() 
        {
            limit = 10;
            offset = 0;   
            broadcast_type = new BroadcastType[] { BroadcastType.HIGHLIGHT };            
        }

        public PagingUserFollowsVideos(int _limit, int _offset, BroadcastType[] _broadcast_type)
        {
            limit = _limit;
            offset = _offset;            
            broadcast_type = _broadcast_type;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 10));
            request.AddParameter("offset", offset);
            request.AddParameter("broadcast_type", string.Join(",", broadcast_type).ToLower());

            return request;
        }
    }
}
