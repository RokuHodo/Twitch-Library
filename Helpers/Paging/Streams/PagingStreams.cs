//standard namespaces
using System;

//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Streams
{   
    public class PagingStreams : PagingLimitOffset, ITwitchPaging
    {
        public int[] channel;

        public string game;                      

        public StreamType stream_type;
        public StreamLanguage[] language;                      

        public PagingStreams() :base(25)
        {
            channel = null;
            game = null;
            stream_type = StreamType.LIVE;
            language = Enum.GetValues(typeof(StreamLanguage)) as StreamLanguage[];
        }

        public PagingStreams(int[] _channel, int _limit, int _offset, string _game, StreamType _stream_type, StreamLanguage[] _language) : base(25)
        {
            channel = _channel;
            limit = _limit;
            offset = _offset;
            game = _game;
            stream_type = _stream_type;
            language = _language;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            if (channel.isValidArray())
            {
                request.AddParameter("channel", string.Join(",", channel));
            }

            request.AddParameter("limit", limit);
            request.AddParameter("offset", offset);

            if (game.isValidString())
            {
                request.AddParameter("game", game);
            }

            request.AddParameter("stream_type", stream_type.ToString().ToLower());
            request.AddParameter("language", string.Join(",", language).ToLower().Replace("_", "-"));

            return request;
        }
    }
}
