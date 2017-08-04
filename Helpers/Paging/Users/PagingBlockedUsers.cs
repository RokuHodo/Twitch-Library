// project namespaces
using TwitchLibrary.Interfaces.Helpers.Paging;

namespace TwitchLibrary.Helpers.Paging.Users
{
    public class PagingBlockedUsers : PagingLimitOffset, ITwitchPaging
    {
        public PagingBlockedUsers() : base(25)
        {

        }

        public PagingBlockedUsers(int _limit, int _offset) : base(25)
        {
            limit   = _limit;
            offset  = _offset;
        }
    }
}
