//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Search
{   
    public class PagingSearchStreams : ITwitchPaging
    {
        public bool? hls;

        public int limit,      //max = 100      default = 25
                   offset;   

        public PagingSearchStreams()
        {
            hls = null;
            limit = 25;
            offset = 0;            
        }

        public PagingSearchStreams(bool _hls, int _limit, int _offset)
        {
            hls = _hls;
            limit = _limit;
            offset = _offset;            
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            if (!hls.isNull())
            {
                request.AddParameter("hls", hls);
            }
                        
            request.AddParameter("limit", limit.Clamp(1, 100, 25));
            request.AddParameter("offset", offset);            

            return request;
        }
    }
}
