//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Streams
{
    public class PagingStreamFollows : PagingLimitOffset, ITwitchPaging
    {
        public StreamType stream_type;                     

        public PagingStreamFollows() : base(25)
        {            
            stream_type = StreamType.LIVE;            
        }

        public PagingStreamFollows(int _limit, int _offset, StreamType _stream_type) : base(25)
        {            
            limit = _limit;
            offset = _offset;
            stream_type = _stream_type;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit);
            request.AddParameter("offset", offset);
            request.AddParameter("stream_type", stream_type.ToString().ToLower());            

            return request;
        }
    }
}
