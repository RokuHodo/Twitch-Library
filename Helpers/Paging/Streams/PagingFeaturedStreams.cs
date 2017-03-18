//project namespaces
using TwitchLibrary.Interfaces.Helpers.Paging;

namespace TwitchLibrary.Helpers.Paging.Streams
{
    public class PagingFeaturedStreams : PagingLimitOffset, ITwitchPaging
    {
        public PagingFeaturedStreams() : base(25)
        {
          
        }

        public PagingFeaturedStreams(int _limit, int _offset) : base(25)
        {
            limit = _limit;
            offset = _offset;            
        }
    }
}
