using System;

//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Channels
{
    public class PagingChannelVideos : ITwitchPaging
    {
        public int limit,       //max = 100         default = 10
                   offset;      //max = 1000        default = 0

        public BroadcastType[] broadcast_type;
        public BroadcasterLanguage[] language;
        public Sort sort;

        public PagingChannelVideos() 
        {
            limit = 10;
            offset = 0;
            language = Enum.GetValues(typeof(BroadcasterLanguage)) as BroadcasterLanguage[];
            broadcast_type = new BroadcastType[] { BroadcastType.HIGHLIGHT };
            sort = Sort.TIME;
        }

        public PagingChannelVideos(int _limit, int _offset, BroadcastType[] _broadcast_type, BroadcasterLanguage[] _language, Sort _sort)
        {
            limit = _limit;
            offset = _offset;
            broadcast_type = _broadcast_type;
            language = _language;
            sort = _sort;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 10));
            request.AddParameter("offset", offset.Clamp(0, 1000, 0));
            request.AddParameter("broadcast_type", string.Join(",", broadcast_type).ToLower());
            request.AddParameter("language", string.Join(",", language).ToLower());
            request.AddParameter("sort", sort.ToString().ToLower());

            return request;
        }
    }
}
