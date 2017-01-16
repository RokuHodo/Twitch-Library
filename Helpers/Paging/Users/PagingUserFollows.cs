//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Users
{
    public class PagingUserFollows : ITwitchPaging
    {
        public int limit,      //max = 100      default = 25
                   offset;

        public Direction direction;
        public SortBy sort_by;

        public PagingUserFollows()
        {
            limit = 25;      
            offset = 0;
            direction = Direction.DESC;
            sort_by = SortBy.CREATED_AT;
        }

        public PagingUserFollows(int _limit, int _offset, Direction _direction, SortBy _sortby)
        {
            limit = _limit;
            offset = _offset;
            direction = _direction;
            sort_by = _sortby;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 25));
            request.AddParameter("offset", offset);            
            request.AddParameter("direction", direction.ToString().ToLower());
            request.AddParameter("sortby", sort_by.ToString().ToLower());

            return request;
        }
    }
}
