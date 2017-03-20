//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Search
{   
    public class PagingSearchStreams : PagingLimitOffset, ITwitchPaging
    {
        public bool? hls;

        public PagingSearchStreams() : base(25)
        {
            hls = null;         
        }

        public PagingSearchStreams(bool _hls, int _limit, int _offset) : base(25)
        {
            hls = _hls;
            limit = _limit;
            offset = _offset;            
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            if (!hls.isNull())
            {
                request.AddQueryParameter("hls", hls.ToString());
            }
                        
            request.AddQueryParameter("limit", limit.ToString());
            request.AddQueryParameter("offset", offset.ToString());            

            return request;
        }
    }
}
