//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Users
{
    public class PagingUserFollows : PagingLimitOffset, ITwitchPaging
    {
        public Direction direction;
        public SortBy sort_by;

        public PagingUserFollows() : base(25)
        {
            direction = Direction.DESC;
            sort_by = SortBy.CREATED_AT;
        }

        public PagingUserFollows(int _limit, int _offset, Direction _direction, SortBy _sortby) : base(25)
        {
            limit = _limit;
            offset = _offset;
            direction = _direction;
            sort_by = _sortby;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit);
            request.AddParameter("offset", offset);            
            request.AddParameter("direction", direction.ToString().ToLower());
            request.AddParameter("sortby", sort_by.ToString().ToLower());

            return request;
        }
    }
}
