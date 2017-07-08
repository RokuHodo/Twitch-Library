// project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Interfaces.Helpers.Paging;

// imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Clips
{   
    public class PagingClips : PagingClipsGamesFollowed, ITwitchPaging
    {
        public string[] game;
        public string[] channel;
        public BroadcasterLanguage[] language;

        public PeriodClips period;

        public PagingClips()
        {
            trending = false;

            game = new string[0];
            channel = new string[0];
            language = new BroadcasterLanguage[0];

            cursor = string.Empty;
            
            period = PeriodClips.DAY;
        }

        public PagingClips(bool _trending, int _limit, string _cursor, string[] _game, string[] _channel, BroadcasterLanguage[] _language, PeriodClips _period) : base(_trending, _limit, _cursor)
        {
            trending = _trending;
            limit = _limit;
            game = _game;
            cursor = _cursor;
            channel = _channel;
            period = _period;
            language = _language;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddQueryParameter("trending", trending.ToString().ToLower());
            request.AddQueryParameter("limit", limit.ToString());         

            if (game.isValid())
            {
                request.AddQueryParameter("game", string.Join(",", game));
            }

            if (cursor.isValid())
            {
                request.AddQueryParameter("cursor", cursor);
            }

            if (channel.isValid())
            {
                request.AddQueryParameter("channel", string.Join(",", channel));
            }

            if (language.isValid())
            {
                request.AddQueryParameter("language", string.Join(",", language));
            }

            request.AddQueryParameter("period", period.ToString().ToLower());

            return request;
        }
    }
}
