
//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging
{
    public class PagingLimit : ITwitchPaging
    {
        //private
        private int _limit;
        private int _limit_default;

        //protected
        protected int limit_default
        {
            get { return _limit_default; }
            set { _limit_default = value.Clamp(limit_min, limit_max); }
        }

        //public
        public readonly int limit_min = 1;
        public readonly int limit_max = 100;        
        public int limit
        {
            get { return _limit; }
            set { _limit = value.Clamp(limit_min, limit_max, limit_default); }
        }

        public PagingLimit(int _limit_default)
        {
            //this can vary depending on what is being requested
            limit_default = _limit_default.Clamp(limit_min, limit_max);
            limit = limit_default;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit);

            return request;
        }
    }
}
