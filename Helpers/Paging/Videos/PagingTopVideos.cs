//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Videos
{
    public class PagingTopVideos : ITwitchPaging
    {
        public int limit,       //max = 100         default = 10
                   offset;      //max = 1000        default = 0

        public string game;

        public PeriodVideos period;
        public BroadcastType[] broadcast_type;

        public PagingTopVideos() 
        {
            limit = 10;
            offset = 0;
            game = null;
            period = PeriodVideos.WEEK;      
            broadcast_type = new BroadcastType[] { BroadcastType.HIGHLIGHT };            
        }

        public PagingTopVideos(int _limit, int _offset, string _game, PeriodVideos _period, BroadcastType[] _broadcast_type)
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
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 10));
            request.AddParameter("offset", offset.Clamp(0, 1000, 0));

            if (game.isValidString())
            {
                request.AddParameter("game", game);
            }
            
            request.AddParameter("period", period.ToString().ToLower());
            request.AddParameter("broadcast_type", string.Join(",", broadcast_type).ToLower());

            return request;
        }
    }
}
