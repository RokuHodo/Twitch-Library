//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging
{
    public class PagingLimitOffset : PagingLimit, ITwitchPaging
    {
        //private
        private int _offset;

        //protected
        protected readonly int offset_default = 0;

        //public
        public readonly int offset_min = 0;
        public readonly int offset_max = 1000;        
        public int offset
        {
            get { return _offset; }
            set { _offset = value.Clamp(offset_min, offset_max, offset_default); }
        }

        public PagingLimitOffset(int _limit_default) : base(_limit_default)
        {
            offset = 0;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit);
            request.AddParameter("offset", offset);

            return request;
        }
    }
}
