// project namespaces
using TwitchLibrary.Interfaces.Helpers.Paging;

namespace TwitchLibrary.Helpers.Paging.Teams
{
    public class PagingTeams : PagingLimitOffset, ITwitchPaging
    {
        public PagingTeams() : base(25)
        {
       
        }

        public PagingTeams(int _limit, int _offset) : base(25)
        {
            limit   = _limit;
            offset  = _offset;            
        }
    }
}
