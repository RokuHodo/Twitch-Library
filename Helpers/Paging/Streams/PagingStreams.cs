using System;

//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Streams
{   
    public class PagingStreams : ITwitchPaging
    {
        public int[] channel;

        public int limit,       //max = 100         default = 25
                   offset;      //max = 1000        default = 0

        public string game;                      

        public StreamType stream_type;
        public StreamLanguage[] language;                      

        public PagingStreams()
        {
            channel = null;
            limit = 25;
            offset = 0;
            game = null;
            stream_type = StreamType.LIVE;
            language = Enum.GetValues(typeof(StreamLanguage)) as StreamLanguage[];
        }

        public PagingStreams(int[] _channel, int _limit, int _offset, string _game, StreamType _stream_type, StreamLanguage[] _language)
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
        public RestRequest Add(RestRequest request)
        {
            if (channel.isValidArray())
            {
                request.AddParameter("channel", string.Join(",", channel));
            }

            request.AddParameter("limit", limit.Clamp(1, 100, 25));
            request.AddParameter("offset", offset.Clamp(0, 1000, 0));

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
