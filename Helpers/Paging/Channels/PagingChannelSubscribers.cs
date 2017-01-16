//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Channels
{
    public class PagingChannelSubscribers : ITwitchPaging
    {
        public int limit,      //max = 100      default = 25
                   offset;

        public Direction direction;

        public PagingChannelSubscribers()
        {
            limit = 25;
            offset = 0;
            direction = Direction.ASC;
        }

        public PagingChannelSubscribers(int _limit, int _offset, Direction _direction)
        {
            limit = _limit;
            offset = _offset;
            direction = _direction;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 25));
            request.AddParameter("offset", offset);
            request.AddParameter("direction", direction.ToString().ToLower());

            return request;
        }
    }
}
