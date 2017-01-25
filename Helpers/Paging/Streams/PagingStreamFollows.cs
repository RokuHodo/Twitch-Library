//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Streams
{
    public class PagingStreamFollows : ITwitchPaging
    {
        public int limit,       //max = 100         default = 25
                   offset;      //max = 1000        default = 0
        
        public StreamType stream_type;                     

        public PagingStreamFollows()
        {            
            limit = 25;
            offset = 0;            
            stream_type = StreamType.LIVE;            
        }

        public PagingStreamFollows(int _limit, int _offset, StreamType _stream_type)
        {            
            limit = _limit;
            offset = _offset;
            stream_type = _stream_type;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 25));
            request.AddParameter("offset", offset.Clamp(0, 1000, 0));
            request.AddParameter("stream_type", stream_type.ToString().ToLower());            

            return request;
        }
    }
}
