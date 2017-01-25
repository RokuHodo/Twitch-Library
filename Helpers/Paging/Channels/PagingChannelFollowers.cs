//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Channels
{
    public class PagingChannelFollowers : ITwitchPaging
    {
        public int limit,       //max = 100         default = 25
                   offset;      //max = 1000        default = 0

        public string cursor;

        public Direction direction = Direction.DESC;

        public PagingChannelFollowers()
        {
            limit = 25;
            offset = 0;
            cursor = string.Empty;
            direction = Direction.DESC;
        }

        public PagingChannelFollowers(int _limit, int _offset, string _cursor, Direction _direction)
        {
            limit = _limit;
            offset = _offset;
            cursor = _cursor;
            direction = _direction;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 25));
            request.AddParameter("offset", offset.Clamp(0, 1000, 0));
            request.AddParameter("cursor", cursor);
            request.AddParameter("direction", direction.ToString().ToLower());

            return request;
        }
    }
}
