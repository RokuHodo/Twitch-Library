// project namespaces
using TwitchLibrary.Interfaces.Helpers.Paging;

namespace TwitchLibrary.Helpers.Paging.Communities
{
    public class PagingCommunityStreams : PagingLimitOffset, ITwitchPaging
    {
        public PagingCommunityStreams() : base(25)
        {
                   
        }

        public PagingCommunityStreams(int _limit, int _offset) : base(25)
        {
            limit = _limit;
            offset = _offset;
        }
    }
}
