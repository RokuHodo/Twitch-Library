//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Channels
{
    public class PagingChannelFollowers : PagingLimitOffset, ITwitchPaging
    {
        public string cursor;

        public Direction direction = Direction.DESC;

        public PagingChannelFollowers() : base(25)
        {
            cursor = string.Empty;
            direction = Direction.DESC;
        }

        public PagingChannelFollowers(int _limit, int _offset, string _cursor, Direction _direction) : base(25)
        {
            limit = _limit;
            offset = _offset;
            cursor = _cursor;
            direction = _direction;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddQueryParameter("limit", limit.ToString());
            request.AddQueryParameter("offset", offset.ToString());

            if (cursor.isValidString())
            {
                request.AddQueryParameter("cursor", cursor);
            }

            request.AddQueryParameter("direction", direction.ToString().ToLower());

            return request;
        }
    }
}
