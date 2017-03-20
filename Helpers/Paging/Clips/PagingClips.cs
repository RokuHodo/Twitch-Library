﻿//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Clips
{   
    public class PagingClips : PagingClipsGamesFollowed, ITwitchPaging
    {
        public string game;

        public string[] channel;

        public PeriodClips period;

        public PagingClips()
        {
            trending = false;
            game = string.Empty;
            cursor = string.Empty;
            channel = new string[0];
            period = PeriodClips.DAY;
        }

        public PagingClips(bool _trending, int _limit, string _cursor, string _game, string[] _channel, PeriodClips _period) : base(_trending, _limit, _cursor)
        {
            trending = _trending;
            limit = _limit;
            game = _game;
            cursor = _cursor;
            channel = _channel;
            period = _period;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddQueryParameter("trending", trending.ToString().ToLower());
            request.AddQueryParameter("limit", limit.ToString());         

            if (game.isValidString())
            {
                request.AddQueryParameter("game", game);
            }

            if (cursor.isValidString())
            {
                request.AddQueryParameter("cursor", cursor);
            }

            if (channel.isValidArray())
            {
                request.AddQueryParameter("channel", string.Join(",", channel));
            }

            request.AddQueryParameter("period", period.ToString().ToLower());

            return request;
        }
    }
}
