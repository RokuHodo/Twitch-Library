//project namespaces
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's

namespace TwitchLibrary.Helpers.Paging.Search
{
    public class PagingSearchChannels : PagingLimitOffset, ITwitchPaging
    {
        public PagingSearchChannels() : base(25)
        {
          
        }

        public PagingSearchChannels(int _limit, int _offset) : base(25)
        {
            limit = _limit;
            offset = _offset;            
        }
    }
}
