//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Videos
{
    public class PagingTopVideos : PagingLimitOffset, ITwitchPaging
    {
        public string game;

        public PeriodVideos period;
        public BroadcastType[] broadcast_type;

        public PagingTopVideos() : base(10)
        {
            game = null;
            period = PeriodVideos.WEEK;      
            broadcast_type = new BroadcastType[] { BroadcastType.HIGHLIGHT };            
        }

        public PagingTopVideos(int _limit, int _offset, string _game, PeriodVideos _period, BroadcastType[] _broadcast_type) : base(10)
        {
            limit = _limit;
            offset = _offset;
            game = _game;
            period = _period;
            broadcast_type = _broadcast_type;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddQueryParameter("limit", limit.ToString());
            request.AddQueryParameter("offset", offset.ToString());

            if (game.isValidString())
            {
                request.AddQueryParameter("game", game);
            }
            
            request.AddQueryParameter("period", period.ToString().ToLower());
            request.AddQueryParameter("broadcast_type", string.Join(",", broadcast_type).ToLower());

            return request;
        }
    }
}
