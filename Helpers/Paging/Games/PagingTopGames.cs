// project namespaces
using TwitchLibrary.Interfaces.Helpers.Paging;

namespace TwitchLibrary.Helpers.Paging.Games
{
    public class PagingTopGames : PagingLimitOffset, ITwitchPaging
    {
        public PagingTopGames() : base(10)
        {

        }

        public PagingTopGames(int _limit, int _offset) : base(10)
        {
            limit   = _limit;
            offset  = _offset;            
        }
    }
}
