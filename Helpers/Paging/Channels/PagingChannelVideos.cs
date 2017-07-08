// standard namespaces
using System;

// project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Interfaces.Helpers.Paging;

// imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Channels
{
    public class PagingChannelVideos : PagingLimitOffset, ITwitchPaging
    {
        public BroadcastType[] broadcast_type;
        public BroadcasterLanguage[] language;
        public Sort sort;

        public PagingChannelVideos() : base(10)
        {
            language = Enum.GetValues(typeof(BroadcasterLanguage)) as BroadcasterLanguage[];
            broadcast_type = new BroadcastType[] { BroadcastType.HIGHLIGHT };
            sort = Sort.TIME;
        }

        public PagingChannelVideos(int _limit, int _offset, BroadcastType[] _broadcast_type, BroadcasterLanguage[] _language, Sort _sort) : base(10)
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
        public new RestRequest Add(RestRequest request)
        {
            request.AddQueryParameter("limit", limit.ToString());
            request.AddQueryParameter("offset", offset.ToString());
            request.AddQueryParameter("broadcast_type", string.Join(",", broadcast_type).ToLower());
            request.AddQueryParameter("language", string.Join(",", language).ToLower());
            request.AddQueryParameter("sort", sort.ToString().ToLower());

            return request;
        }
    }
}
