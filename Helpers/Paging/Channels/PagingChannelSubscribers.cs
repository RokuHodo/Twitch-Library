// project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Interfaces.Helpers.Paging;

// imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Channels
{
    public class PagingChannelSubscribers : PagingLimitOffset, ITwitchPaging
    {
        public Direction direction;

        public PagingChannelSubscribers() : base(25)
        {
            direction = Direction.ASC;
        }

        public PagingChannelSubscribers(int _limit, int _offset, Direction _direction) : base(25)
        {
            limit = _limit;
            offset = _offset;
            direction = _direction;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddQueryParameter("limit", limit.ToString());
            request.AddQueryParameter("offset", offset.ToString());
            request.AddQueryParameter("direction", direction.ToString().ToLower());

            return request;
        }
    }
}
