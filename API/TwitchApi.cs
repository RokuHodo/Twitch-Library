using System;
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Enums.API;
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Json;
using TwitchLibrary.Helpers.Paging.Channels;
using TwitchLibrary.Helpers.Paging.Clips;
using TwitchLibrary.Helpers.Paging.Games;
using TwitchLibrary.Helpers.Paging.Search;
using TwitchLibrary.Helpers.Paging.Streams;
using TwitchLibrary.Helpers.Paging.Teams;
using TwitchLibrary.Helpers.Paging.Users;
using TwitchLibrary.Helpers.Paging.Videos;
using TwitchLibrary.Interfaces.API;
using TwitchLibrary.Models.API.Channels;
using TwitchLibrary.Models.API.Chat;
using TwitchLibrary.Models.API.Clips;
using TwitchLibrary.Models.API.Games;
using TwitchLibrary.Models.API.Ingests;
using TwitchLibrary.Models.API.Search;
using TwitchLibrary.Models.API.Streams;
using TwitchLibrary.Models.API.Teams;
using TwitchLibrary.Models.API.Users;
using TwitchLibrary.Models.API.Videos;

//imported .dll's
using RestSharp;

/*
 * TODO: (API) Master todo list
 *      -   Convert all id's to string from their current type (including models like emotes, stream, etc?)
 *      -   Add in video upload endpoints and methods
 *      -   Implement custom exceptions for each failure case
 *      
 */

namespace TwitchLibrary.API
{
    public class TwitchApi : ITwitchRequest
    {
        protected string client_id,
                         oauth_token;

        protected RestClient client;

        public TwitchApi(string client_id, string oauth_token = "")
        {
            this.client_id = client_id;
            this.oauth_token = oauth_token;
                  
            client = new RestClient("https://api.twitch.tv/kraken");                                    
            client.AddHandler("application/json", new CustomJsonDeserializer());
            client.AddHandler("text/html", new CustomJsonDeserializer());
            client.AddDefaultHeader("Accept", "application/vnd.twitchtv.v5+json");
        }

        #region Channels

        /// <summary>
        /// Gets the <see cref="Channel"/> object of a channel.
        /// </summary>
        public Channel GetChannel(string channel_id)
        {
            RestRequest request = Request("channels/{channel_id}", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<Channel> response = client.Execute<Channel>(request);

            return response.Data;
        }

        /// <summary>
        /// Checks to see if the channel associated with the chanel_id is partnered.
        /// </summary>
        public bool isPartner(string channel_id)
        {
            return GetChannel(channel_id).partner;
        }

        /// <summary>
        /// Gets a single paged list of followers for a channel.
        /// <see cref="PagingChannelFollowers"/> can be specified to request a custom paged result.
        /// </summary>
        public FollowerPage GetChannelFollowersPage(string channel_id, PagingChannelFollowers paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelFollowers();
            }

            RestRequest request = Request("channels/{channel_id}/follows", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);
            request = paging.Add(request);

            IRestResponse<FollowerPage> response = client.Execute<FollowerPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all followers of a channel in descending order.
        /// </summary>
        public List<Follower> GetChannelFollowers(string channel_id, Direction direction = Direction.DESC)
        {
            List<Follower> followers = new List<Follower>();

            PagingChannelFollowers paging = new PagingChannelFollowers();
            paging.limit = 100;
            paging.direction = direction;

            FollowerPage follower_page = GetChannelFollowersPage(channel_id, paging);

            if (follower_page._total == 0)
            {
                return followers;
            }

            bool searching = true;

            do
            {
                foreach (Follower follower in follower_page.follows)
                {
                    followers.Add(follower);
                }

                if (follower_page._cursor.isValidString())
                {
                    paging.cursor = follower_page._cursor;

                    follower_page = GetChannelFollowersPage(channel_id, paging);
                }
                else
                {
                    searching = false;
                }
            }
            while (searching);

            return followers;
        }

        /// <summary>
        /// Gets a complete list of all teams that a channel belongs to.
        /// </summary>
        public ChannelTeams GetChannelTeams(string channel_id)
        {   
            RestRequest request = Request("channels/{channel_id}/teams", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<ChannelTeams> response = client.Execute<ChannelTeams>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a single paged list of videos for a channel.
        /// <see cref="PagingChannelVideos"/> can be specified to request a custom paged result.
        /// </summary>
        public VideosPage GetChannelVideosPage(string channel_id, PagingChannelVideos paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelVideos();
            }

            RestRequest request = Request("channels/{channel_id}/videos", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);
            request = paging.Add(request);

            IRestResponse<VideosPage> response = client.Execute<VideosPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all the archived broadcasts for a channel from oldest to newest.
        /// </summary>
        public List<Video> GetChannelArchives(string channel_id, Sort sort = Sort.TIME)
        {
            return GetChannelVideos(channel_id, sort, new BroadcastType[] { BroadcastType.ARCHIVE });
        }

        /// <summary>
        /// Gets a complete list of all the highlights for a channel from oldest to newest.
        /// </summary>
        public List<Video> GetChannelHighlights(string channel_id, Sort sort = Sort.TIME)
        {
            return GetChannelVideos(channel_id, sort, new BroadcastType[] { BroadcastType.HIGHLIGHT });
        }

        /// <summary>
        /// Gets a complete list of all the uploads for a channel from oldest to newest.
        /// </summary>
        public List<Video> GetChannelUploads(string channel_id, Sort sort = Sort.TIME)
        {
            return GetChannelVideos(channel_id, sort, new BroadcastType[] { BroadcastType.UPLOAD });
        }

        /// <summary>
        /// Gets a complete list of all the videos (archives, uploads, and highlights) for a channel from oldest to newest.
        /// </summary>
        public List<Video> GetChannelVideos(string channel_id, Sort sort = Sort.TIME)
        {
            return GetChannelVideos(channel_id, sort, new BroadcastType[] { BroadcastType.ARCHIVE, BroadcastType.HIGHLIGHT, BroadcastType.UPLOAD });
        }

        /// <summary>
        /// Gets a complete list of filtered videos for a channel in a specified sort order.
        /// </summary>
        public List<Video> GetChannelVideos(string channel_id, Sort sort, BroadcastType[] broadcast_type)
        {
            List<Video> videos = new List<Video>();

            PagingChannelVideos paging = new PagingChannelVideos();
            paging.limit = 100;
            paging.broadcast_type = broadcast_type;

            VideosPage videos_page = GetChannelVideosPage(channel_id, paging);

            if (videos_page._total == 0)
            {
                return videos;
            }

            decimal pages_dec = videos_page._total / paging.limit;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));

            for (int page = 0; page < pages + 1; ++page)
            {
                //don't request the first page again because we already have it
                if (page != 0)
                {
                    paging.offset = paging.limit * page;
                    videos_page = GetChannelVideosPage(channel_id, paging);
                }

                foreach (Video video in videos_page.videos)
                {
                    videos.Add(video);
                }
            }

            return videos;
        }

        #endregion

        #region Chat

        /// <summary>
        /// Gets the chat badges that can be used in a channel.
        /// </summary>
        public Badges GetChatBadges(string channel_id)
        {
            RestRequest request = Request("chat/{channel_id}/badges", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<Badges> response = client.Execute<Badges>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets the emotes associated with an emoticon set.
        /// The emote data returned does not include images url's.
        /// </summary>
        public EmoteSet GetChatEmotes(string emote_set)
        {
            RestRequest request = Request("chat/emoticon_images", Method.GET);
            request.AddQueryParameter("emotesets", emote_set);

            IRestResponse<EmoteSet> response = client.Execute<EmoteSet>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a list of all valid emotes in Twitch.        
        /// The emote data returned does not include images url's.
        /// </summary>        
        public Emotes GetChatEmotes()
        {
            RestRequest request = Request("chat/emoticon_images", Method.GET);            

            IRestResponse<Emotes> response = client.Execute<Emotes>(request);

            return response.Data;
        }


        /// <summary>
        /// Gets a list of all valid emotes in Twitch with their image link and size.
        /// </summary>        
        public EmoteImages GetChatEmoteImages()
        {
            RestRequest request = Request("chat/emoticons", Method.GET);                        

            IRestResponse<EmoteImages> response = client.Execute<EmoteImages>(request);            

            return response.Data;
        }

        #endregion

        #region Clips

        /// <summary>
        /// Gets a single paged list of top clips on Twitch from highest to lowest view count.
        /// <see cref="PagingClips"/> can be specified to request a custom paged result.
        /// </summary>
        public ClipsPage GetTopClipsPage(PagingClips paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingClips();
            }

            RestRequest request = Request("clips/top", Method.GET, ApiVersion.v4);
            request = paging.Add(request);

            IRestResponse<ClipsPage> response = client.Execute<ClipsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of top clips on Twitch from highest to lowest view count.     
        /// <see cref="PagingClips"/> can be specified to request a custom list.
        /// </summary>  
        public List<Clip> GetTopClips(PagingClips paging = null)
        {
            List<Clip> clips = new List<Clip>();

            if (paging.isNull())
            {
                paging = new PagingClips();
                paging.limit = 100;
                paging.period = PeriodClips.ALL;
            }

            ClipsPage clips_page = GetTopClipsPage(paging);

            if (!clips_page.clips.isValidList())
            {
                return clips;
            }

            bool searching = true;

            do
            {
                foreach (Clip clip in clips_page.clips)
                {
                    clips.Add(clip);
                }

                if (clips_page._cursor.isValidString())
                {
                    paging.cursor = clips_page._cursor;

                    clips_page = GetTopClipsPage(paging);
                }
                else
                {
                    searching = false;
                }
            }
            while (searching);

            return clips;
        }

        /// <summary>
        /// Gets the information of a specified clip for a specific channel
        /// </summary>
        public Clip GetClip(string channel_name, string slug)
        {
            RestRequest request = Request("clips/{channel_name}/{slug}", Method.GET, ApiVersion.v4);
            request.AddUrlSegment("channel_name", channel_name);
            request.AddUrlSegment("slug", slug);

            IRestResponse<Clip> response = client.Execute<Clip>(request);

            return response.Data;
        }

        #endregion

        #region Games

        /// <summary>
        /// Gets a single paged list of top games currently being played on Twitch from highest to lowest view count.
        /// <see cref="PagingTopGames"/> can be specified to request a custom paged result.
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>        
        public TopGamesPage GetTopGamesPage(PagingTopGames paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingTopGames();
            }

            RestRequest request = Request("games/top", Method.GET);
            request = paging.Add(request);

            IRestResponse<TopGamesPage> response = client.Execute<TopGamesPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of top games currently being played on Twitch from highest to lowest view count.        
        /// </summary>        
        public List<TopGame> GetTopGames()
        {
            List<TopGame> top_games = new List<TopGame>();

            PagingTopGames paging = new PagingTopGames();
            paging.limit = 100;

            TopGamesPage top_games_page = GetTopGamesPage(paging);

            if (top_games_page._total == 0)
            {
                return top_games;
            }

            decimal pages_dec = top_games_page._total / paging.limit;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));

            for (int page = 0; page < pages + 1; ++page)
            {
                //don't request the first page again because we already have it
                if (page != 0)
                {
                    paging.offset = paging.limit * page;
                    top_games_page = GetTopGamesPage(paging);
                }

                foreach (TopGame follow in top_games_page.top)
                {
                    top_games.Add(follow);
                }
            }

            return top_games;
        }

        #endregion

        #region Ingests

        /// <summary>
        /// Gets a complete list of all Twitch servers to connect to.
        /// </summary>        
        public Ingests GetIngests()
        {
            RestRequest request = Request("ingests", Method.GET);            

            IRestResponse<Ingests> response = client.Execute<Ingests>(request);

            return response.Data;
        }

        #endregion

        #region Search

        /// <summary>
        /// Gets a single paged list of channels based on the name query.
        /// <see cref="PagingSearchChannels"/> can be specified to request a custom paged result.
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>        
        public SearchChannelsPage SearchChannelsPage(string channel_name, PagingSearchChannels paging = null)
        {            
            if (paging.isNull())
            {
                paging = new PagingSearchChannels();
            }            

            RestRequest request = Request("search/channels", Method.GET);
            request.AddQueryParameter("query", channel_name.ToLower());
            request = paging.Add(request);

            IRestResponse<SearchChannelsPage> response = client.Execute<SearchChannelsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of channels  based on the name query.
        /// <see cref="PagingSearchChannels"/> can be specified to request a custom paged result.        
        /// </summary>        
        public List<Channel> SearchChannels(string channel_name)
        {
            List<Channel> search_channels = new List<Channel>();

            PagingSearchChannels paging = new PagingSearchChannels();
            paging.limit = 100;

            SearchChannelsPage search_channels_page = SearchChannelsPage(channel_name, paging);

            if (search_channels_page._total == 0)
            {
                return search_channels;
            }

            decimal pages_dec = search_channels_page._total / paging.limit;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));

            for (int page = 0; page < pages + 1; ++page)
            {
                //don't request the first page again because we already have it 
                if (page != 0)
                {
                    paging.offset = paging.limit * page - 1;
                    search_channels_page = SearchChannelsPage(channel_name, paging);
                }

                foreach (Channel channel in search_channels_page.channels)
                {
                    search_channels.Add(channel);
                }
            }

            return search_channels;
        }

        /// <summary>
        /// Gets a single paged list of streams based on the search query.
        /// <see cref="PagingSearchChannels"/> can be specified to request a custom paged result.
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>
        public SearchStreamsPage SearchStreamsPage(string search, PagingSearchStreams paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingSearchStreams();
            }

            RestRequest request = Request("search/streams", Method.GET);
            request.AddQueryParameter("query", search.ToLower());
            request = paging.Add(request);

            IRestResponse<SearchStreamsPage> response = client.Execute<SearchStreamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of streams based on the search query.
        /// <see cref="PagingSearchStreams"/> can be specified to request a custom paged result.        
        /// </summary>        
        public List<Stream> SearchStreams(string search)
        {
            List<Stream> search_streams = new List<Stream>();

            PagingSearchStreams paging = new PagingSearchStreams();
            paging.limit = 100;

            SearchStreamsPage search_streams_page = SearchStreamsPage(search, paging);

            if (search_streams_page._total == 0)
            {
                return search_streams;
            }

            decimal pages_dec = search_streams_page._total / paging.limit;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));

            for (int page = 0; page < pages + 1; ++page)
            {
                //don't request the first page again because we already have it 
                if (page != 0)
                {
                    paging.offset = paging.limit * page - 1;
                    search_streams_page = SearchStreamsPage(search, paging);
                }

                foreach (Stream stream in search_streams_page.streams)
                {
                    search_streams.Add(stream);
                }
            }

            return search_streams;
        }

        /// <summary>
        /// Gets a complete list of all games based on the game query.
        /// When 'live' is set to 'true', only games that are live on at least one channel are returned. 
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>
        public SearchGames SearchGames(string game_name, bool live = false)
        {
            RestRequest request = Request("search/games", Method.GET);
            request.AddQueryParameter("query", game_name);
            request.AddQueryParameter("live", live.ToString().ToLower());

            IRestResponse<SearchGames> response = client.Execute<SearchGames>(request);

            return response.Data;
        }

        #endregion

        #region Streams

        /// <summary>
        /// Gets a stream object of the specified channel.
        /// </summary>
        public StreamResult GetStream(string channel_id, StreamType stream_type = StreamType.LIVE)
        {
            RestRequest request = Request("streams/{channel_id}", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);
            request.AddQueryParameter("stream_type", stream_type.ToString().ToLower());

            IRestResponse<StreamResult> response = client.Execute<StreamResult>(request);

            return response.Data;
        }

        /// <summary>
        /// Checks to see if the channel associated with the chanel_id is live.
        /// </summary>
        public bool isLive(string channel_id)
        {
            return !GetStream(channel_id).stream.isNull();
        }

        /// <summary>
        /// Gets how long the channel associated with the chanel_id has been live.
        /// Returns <see cref="TimeSpan.Zero"/> if the channel is offline.
        /// </summary>
        public TimeSpan GetUpTime(string channel_id)
        {
            StreamResult stream = GetStream(channel_id);

            return !stream.isNull() ? DateTime.Now.Subtract(stream.stream.created_at.ToLocalTime()) : TimeSpan.Zero;
        }

        /// <summary>
        /// Gets a single paged list of streams.
        /// <see cref="PagingStreams"/> can be specified to request a custom paged result.
        /// </summary>
        public StreamsPage GetStreamsPage(PagingStreams paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingStreams();                
            }

            RestRequest request = Request("streams", Method.GET);
            request = paging.Add(request);

            IRestResponse<StreamsPage> response = client.Execute<StreamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all streams.
        /// <see cref="PagingStreams"/> can be specified to request a custom result.        
        /// </summary>        
        public List<Stream> GetStreams(PagingStreams paging = null)
        {
            List<Stream> streams = new List<Stream>();

            if (paging.isNull())
            {
                paging = new PagingStreams();
                paging.limit = 100;                
            }            

            StreamsPage stream_page = GetStreamsPage(paging);

            if (stream_page._total == 0)
            {
                return streams;
            }

            decimal pages_dec = stream_page._total / paging.limit;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));

            for (int page = 0; page < pages + 1; ++page)
            {
                //don't request the first page again because we already have it
                if (page != 0)
                {
                    paging.offset = paging.limit * page;
                    stream_page = GetStreamsPage(paging);
                }

                foreach (Stream stream in stream_page.streams)
                {
                    streams.Add(stream);
                }
            }

            return streams;
        }

        /// <summary>
        /// Gets a single paged list of the featured streams.
        /// <see cref="PagingFeaturedStreams"/> can be specified to request a custom paged result.
        /// </summary>
        public FeaturedStreamsPage GetFeaturedStreamsPage(PagingFeaturedStreams paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingFeaturedStreams();
            }

            RestRequest request = Request("streams/featured", Method.GET);
            request = paging.Add(request);

            IRestResponse<FeaturedStreamsPage> response = client.Execute<FeaturedStreamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of the featured streams.
        /// </summary>        
        public List<FeaturedStream> GetFeaturedStreams()
        {
            List<FeaturedStream> streams_featured = new List<FeaturedStream>();

            PagingFeaturedStreams paging = new PagingFeaturedStreams();
            paging.limit = 100;

            FeaturedStreamsPage stream_page = GetFeaturedStreamsPage(paging);

            if (!stream_page.featured.isValidList())
            {
                return streams_featured;
            }

            int page = 0;

            //Twitch droppped the ball and didn't include _total or _cursor, just loop until we find nothing
            do
            {
                foreach (FeaturedStream stream in stream_page.featured)
                {
                    streams_featured.Add(stream);
                }

                ++page;
                paging.offset = paging.limit * page;

                stream_page = GetFeaturedStreamsPage(paging);

            }
            while (stream_page.featured.isValidList());

            return streams_featured;
        }

        /// <summary>
        /// Gets the how many channels are streaming and how many viewers are watching.
        /// If a game is specified, only results for that game will be returned.
        /// </summary>
        public StreamSummary GetStreamsSummary(string game_name = null)
        {
            RestRequest request = Request("streams/summary", Method.GET); 
            
            if(game_name.isValidString())
            {
                request.AddQueryParameter("game", game_name);
            }

            IRestResponse<StreamSummary> response = client.Execute<StreamSummary>(request);

            return response.Data;
        }

        #endregion

        #region Teams

        /// <summary>
        /// Gets the <see cref="Team"/> object for a specified team name.
        /// NOTE: the team name is not always the same as the display name.
        /// </summary>        
        public Team GetTeam(string team_name)
        {
            RestRequest request = Request("teams/{team}", Method.GET);
            request.AddUrlSegment("team", team_name);

            IRestResponse<Team> response = client.Execute<Team>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a single paged list of the twitch teams.
        /// <see cref="PagingTeams"/> can be specified to request a custom paged result.
        /// </summary>
        public TeamsPage GetTeamsPage(PagingTeams paging = null)
        {
            RestRequest request = Request("teams", Method.GET);

            if (paging.isNull())
            {
                paging = new PagingTeams();
                paging.limit = 100;
            }

            request = paging.Add(request);

            IRestResponse<TeamsPage> response = client.Execute<TeamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of the twitch teams.
        /// </summary>
        public List<TeamBase> GetTeams()
        {
            List<TeamBase> teams = new List<TeamBase>();

            PagingTeams paging = new PagingTeams();
            paging.limit = 100;

            TeamsPage teams_page = GetTeamsPage(paging);

            if (!teams_page.teams.isValidList())
            {
                return teams;
            }

            int page = 0;

            //Twitch droppped the ball and didn't include _total or _cursor, just loop until we find nothing
            do
            {
                foreach (TeamBase stream in teams_page.teams)
                {
                    teams.Add(stream);
                }

                ++page;
                paging.offset = paging.limit * page;

                teams_page = GetTeamsPage(paging);

            }
            while (teams_page.teams.isValidList());

            return teams;
        }

        #endregion

        #region Users

        /// <summary>
        /// Gets the <see cref="User"/> object for a user.
        /// Required scope: 'user_read'
        /// </summary>
        public User GetUser(string user_id)
        {
            RestRequest request = Request("users/{user_id}", Method.GET);
            request.AddUrlSegment("user_id", user_id);

            IRestResponse<User> response = client.Execute<User>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a single paged list of channels a user is following.
        /// <see cref="PagingUserFollows"/> can be specified to request a custom paged result.
        /// </summary>
        public FollowsPage GetUserFollowsPage(string user_id, PagingUserFollows paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingUserFollows();
                paging.limit = 1;
            }

            RestRequest request = Request("users/{user_id}/follows/channels", Method.GET);
            request.AddUrlSegment("user_id", user_id);
            request = paging.Add(request);

            IRestResponse<FollowsPage> response = client.Execute<FollowsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all channels a user is following sorted from newest oldest.
        /// </summary>
        public List<Follow> GetUserFollows(string user_id, PagingUserFollows paging = null)
        {
            List<Follow> following = new List<Follow>();

            if (paging.isNull())
            {
                paging = new PagingUserFollows();
                paging.limit = 100;
            }            

            FollowsPage follow_page = GetUserFollowsPage(user_id, paging);

            if (follow_page._total == 0)
            {
                return following;
            }

            decimal pages_dec = follow_page._total / paging.limit;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));

            for (int page = 0; page < pages + 1; ++page)
            {
                //don't request the first page again because we already have it
                if (page != 0)
                {
                    paging.offset = paging.limit * page;
                    follow_page = GetUserFollowsPage(user_id, paging);
                }

                foreach (Follow follow in follow_page.follows)
                {
                    following.Add(follow);
                }
            }

            return following;
        }

        /// <summary>
        /// Gets the follower relationship between a user and a channel.
        /// Returns status '404' error message if the user is not following the channel.
        /// </summary>
        private Follow GetUserFollowerRelationship(string user_id, string channel_id)
        {
            RestRequest request = Request("users/{user_id}/follows/channels/{channel_id}", Method.GET);
            request.AddUrlSegment("user_id", user_id);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<Follow> response = client.Execute<Follow>(request);

            return response.Data;
        }

        /// <summary>
        /// Checks if a user is following a channel.
        /// </summary>
        public bool isUserFollowing(string user_id, string channel_id)
        {
            return GetUserFollowerRelationship(user_id, channel_id).http_status != 404;
        }

        /// <summary>
        /// Gets ther date when a user has been followed a channel.
        /// The date is converted to the broadcaster's local time zone.
        /// Returns <see cref="DateTime.MinValue"/> if the user is not following the channel.
        /// </summary>
        public DateTime GetHowlong(string user_id, string channel_id)
        {
            Follow follower = GetUserFollowerRelationship(user_id, channel_id);

            return follower.http_status != 404 ? follower.created_at.ToLocalTime() : DateTime.MinValue;
        }

        #endregion

        #region Videos

        /// <summary>
        /// Gets the <see cref="Video"/> object for a specified video id. 
        /// The video id does not need to include the "v", but can also be included.
        /// </summary>
        public Video GetVideo(string video_id)
        {
            RestRequest request = Request("videos/{video_id}", Method.GET);
            request.AddUrlSegment("video_id", video_id);

            IRestResponse<Video> response = client.Execute<Video>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a single paged list of the top videos on twitch.
        /// <see cref="PagingTopVideos"/> can be specified to request a custom paged result.        
        /// </summary>        
        public TopVideosPage GetTopVideosPage(PagingTopVideos paging = null)
        {
            RestRequest request = Request("videos/top", Method.GET);

            if (paging.isNull())
            {
                paging = new PagingTopVideos();                                
            }

            request = paging.Add(request);

            IRestResponse<TopVideosPage> response = client.Execute<TopVideosPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of the top videos on twitch.
        /// </summary>
        public List<Video> GetTopVideos(PagingTopVideos paging = null)
        {
            List<Video> top_videos = new List<Video>();

            if (paging.isNull())
            {
                paging = new PagingTopVideos();
                paging.limit = 100;
            }            

            TopVideosPage top_videos_page = GetTopVideosPage(paging);

            if (!top_videos_page.vods.isValidList())
            {
                return top_videos;
            }

            int page = 0;

            //Twitch droppped the ball and didn't include _total or _cursor, just loop until we find nothing
            do
            {
                foreach (Video video in top_videos_page.vods)
                {
                    top_videos.Add(video);
                }

                ++page;
                paging.offset = paging.limit * page;

                top_videos_page = GetTopVideosPage(paging);

            }
            while (top_videos_page.vods.isValidList());

            return top_videos;
        }

        #endregion

        #region Rest Request

        /// <summary>
        /// Send the endpoint requests to the api.
        /// </summary>
        public RestRequest Request(string endpoint, Method method, ApiVersion api_version = ApiVersion.v5)
        {
            RestRequest request = new RestRequest(endpoint, method);            

            if (oauth_token.isValidString())
            {
                request.AddHeader("Authorization", "OAuth " + oauth_token);
            }
            else
            {
                request.AddHeader("Client-ID", client_id);
            }

            request.AddQueryParameter("api_version", api_version.ToString().TextAfter("v"));                      

            return request;
        }

        #endregion
    }
}
