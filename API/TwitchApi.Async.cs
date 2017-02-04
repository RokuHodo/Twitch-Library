using System;
using System.Collections.Generic;
using System.Threading.Tasks;

//project namespaces
using TwitchLibrary.Enums.API;
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Json;
using TwitchLibrary.Helpers.Paging;
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

namespace TwitchLibrary.API
{
    public partial class TwitchApi : ITwitchRequest
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

        #region Channels                DONE

        /// <summary>
        /// Asynchronously gets the <see cref="Channel"/> object of a channel.
        /// </summary>
        public async Task<Channel> GetChannelAsync(string channel_id)
        {
            RestRequest request = Request("channels/{channel_id}", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<Channel> response = await client.ExecuteTaskAsync<Channel>(request);

            return response.Data;
        }

        /// <summary>
        /// Checks to see if the channel associated with the chanel_id is partnered.
        /// </summary>
        public async Task<bool> isPartnerAsync(string channel_id)
        {
            Channel channel = await GetChannelAsync(channel_id);

            return channel.partner;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of followers for a channel.
        /// <see cref="PagingChannelFollowers"/> can be specified to request a custom paged result.
        /// </summary>
        public async Task<FollowerPage> GetChannelFollowersPageAsync(string channel_id, PagingChannelFollowers paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelFollowers();
            }

            RestRequest request = Request("channels/{channel_id}/follows", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);
            request = paging.Add(request);

            IRestResponse<FollowerPage> response = await client.ExecuteTaskAsync<FollowerPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all followers of a channel in descending order.
        /// </summary>
        public async Task<List<Follower>> GetChannelFollowersAsync(string channel_id, Direction direction = Direction.DESC)
        {
            PagingChannelFollowers paging = new PagingChannelFollowers();
            paging.limit = 100;            
            paging.direction = direction;

            List<Follower> followers = await Paging.GetPagesByCursorAsync<Follower, FollowerPage, PagingChannelFollowers>(GetChannelFollowersPageAsync, channel_id, paging, "follows");

            return followers;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all teams that a channel belongs to.
        /// </summary>
        public async Task<ChannelTeams> GetChannelTeamsAsync(string channel_id)
        {
            RestRequest request = Request("channels/{channel_id}/teams", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<ChannelTeams> response = await client.ExecuteTaskAsync<ChannelTeams>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of videos for a channel.
        /// <see cref="PagingChannelVideos"/> can be specified to request a custom paged result.
        /// </summary>
        public async Task<VideosPage> GetChannelVideosPageAsync(string channel_id, PagingChannelVideos paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelVideos();
            }

            RestRequest request = Request("channels/{channel_id}/videos", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);
            request = paging.Add(request);

            IRestResponse<VideosPage> response = await client.ExecuteTaskAsync<VideosPage>(request);

            return response.Data;
        }
        
        /// <summary>
        /// Asynchronously gets a complete list of all the archived broadcasts for a channel from oldest to newest.
        /// </summary>
        public async Task<List<Video>> GetChannelArchivesAsync(string channel_id, Sort sort = Sort.TIME)
        {
            List<Video> archives = await GetChannelVideosAsync(channel_id, sort, new BroadcastType[] { BroadcastType.ARCHIVE });

            return archives;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all the highlights for a channel from oldest to newest.
        /// </summary>
        public async Task<List<Video>> GetChannelHighlightsAsync(string channel_id, Sort sort = Sort.TIME)
        {
            List<Video> highlights = await GetChannelVideosAsync(channel_id, sort, new BroadcastType[] { BroadcastType.HIGHLIGHT });

            return highlights;
        }

        /// <summary>
        /// AsynchronouslygGets a complete list of all the uploads for a channel from oldest to newest.
        /// </summary>
        public async Task<List<Video>> GetChannelUploadsAsync(string channel_id, Sort sort = Sort.TIME)
        {
            List<Video> uploads = await GetChannelVideosAsync(channel_id, sort, new BroadcastType[] { BroadcastType.UPLOAD });

            return uploads;
        }

        /// <summary>
        /// Asynchronouslyg gets a complete list of all the videos (archives, uploads, and highlights) for a channel from oldest to newest.
        /// </summary>
        public async Task<List<Video>> GetChannelVideosAsync(string channel_id, Sort sort = Sort.TIME)
        {
            List<Video> videos = await GetChannelVideosAsync(channel_id, sort, new BroadcastType[] { BroadcastType.ARCHIVE, BroadcastType.HIGHLIGHT, BroadcastType.UPLOAD });

            return videos;
        }

        /// <summary>
        /// Asynchronously gets a complete list of videos for a channel in a specified sort order.
        /// </summary>
        public async Task<List<Video>> GetChannelVideosAsync(string channel_id, Sort sort, BroadcastType[] broadcast_type)
        {
            PagingChannelVideos paging = new PagingChannelVideos();
            paging.limit = 100;
            paging.sort = sort;
            paging.broadcast_type = broadcast_type;

            List<Video> videos = await Paging.GetPagesByTotalAsync<Video, VideosPage, PagingChannelVideos >(GetChannelVideosPageAsync, channel_id, paging, "videos");

            return videos;
        }

        #endregion

        #region Chat                    DONE

        /// <summary>
        /// Asynchronously gets the chat badges that can be used in a channel.
        /// </summary>
        public async Task<Badges> GetChatBadgesAsync(string channel_id)
        {
            RestRequest request = Request("chat/{channel_id}/badges", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<Badges> response = await client.ExecuteTaskAsync<Badges>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets the emotes associated with an emoticon set.
        /// The emote data returned does not include images url's.
        /// </summary>
        public async Task<EmoteSet> GetChatEmotesAsync(string emote_set)
        {
            RestRequest request = Request("chat/emoticon_images", Method.GET);
            request.AddQueryParameter("emotesets", emote_set);

            IRestResponse<EmoteSet> response = await client.ExecuteTaskAsync<EmoteSet>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a list of all valid emotes in Twitch.        
        /// The emote data returned does not include images url's.
        /// </summary>        
        public async Task<Emotes> GetChatEmotesAsync()
        {
            RestRequest request = Request("chat/emoticon_images", Method.GET);

            IRestResponse<Emotes> response = await client.ExecuteTaskAsync<Emotes>(request);

            return response.Data;
        }


        /// <summary>
        /// Asynchronously gets a list of all valid emotes in Twitch with their image link and size.
        /// </summary>        
        public async Task<EmoteImages> GetChatEmoteImagesAsync()
        {
            RestRequest request = Request("chat/emoticons", Method.GET);

            IRestResponse<EmoteImages> response = await client.ExecuteTaskAsync<EmoteImages>(request);

            return response.Data;
        }

        #endregion

        #region Clips                   DONE

        /// <summary>
        /// Asynchronously gets a single paged list of top clips on Twitch from highest to lowest view count.
        /// <see cref="PagingClips"/> can be specified to request a custom paged result.
        /// </summary>
        public async Task<ClipsPage> GetTopClipsPageAsync(PagingClips paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingClips();
            }

            RestRequest request = Request("clips/top", Method.GET, ApiVersion.v4);
            request = paging.Add(request);

            IRestResponse<ClipsPage> response = await client.ExecuteTaskAsync<ClipsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of top clips on Twitch from highest to lowest view count.     
        /// <see cref="PagingClips"/> can be specified to request a custom list.
        /// </summary>  
        public async Task<List<Clip>> GetTopClipsAsync(PagingClips paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingClips();
                paging.limit = 100;
                paging.period = PeriodClips.ALL;
            }

            List<Clip> clips = await Paging.GetPagesByCursorAsync<Clip, ClipsPage, PagingClips>(GetTopClipsPageAsync, paging, "clips");

            return clips;
        }

        /// <summary>
        /// Asynchronously gets the information of a specified clip for a specific channel
        /// </summary>
        public async Task<Clip> GetClipAsync(string channel_name, string slug)
        {
            RestRequest request = Request("clips/{channel_name}/{slug}", Method.GET, ApiVersion.v4);
            request.AddUrlSegment("channel_name", channel_name);
            request.AddUrlSegment("slug", slug);

            IRestResponse<Clip> response = await client.ExecuteTaskAsync<Clip>(request);

            return response.Data;
        }

        #endregion

        #region Games                   DONE

        /// <summary>
        /// Asynchronously gets a single paged list of top games currently being played on Twitch from highest to lowest view count.
        /// <see cref="PagingTopGames"/> can be specified to request a custom paged result.
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>        
        public async Task<TopGamesPage> GetTopGamesPageAsync(PagingTopGames paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingTopGames();
            }

            RestRequest request = Request("games/top", Method.GET);
            request = paging.Add(request);

            IRestResponse<TopGamesPage> response = await client.ExecuteTaskAsync<TopGamesPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of top games currently being played on Twitch from highest to lowest view count.        
        /// </summary>        
        public async Task<List<TopGame>> GetTopGamesAsync()
        {
            PagingTopGames paging = new PagingTopGames();
            paging.limit = 100;

            List<TopGame> top_games = await Paging.GetPagesByTotalAsync<TopGame, TopGamesPage, PagingTopGames>(GetTopGamesPageAsync, paging, "top");

            return top_games;
        }

        #endregion

        #region Ingests                 DONE

        /// <summary>
        /// Asynchronously gets a complete list of all Twitch servers to connect to.
        /// </summary>        
        public async Task<Ingests> GetIngestsAsync()
        {
            RestRequest request = Request("ingests", Method.GET);

            IRestResponse<Ingests> response = await client.ExecuteTaskAsync<Ingests>(request);

            return response.Data;
        }

        #endregion

        #region Search                  DONE

        /// <summary>
        /// Asynchronously gets a single paged list of channels based on the name query.
        /// <see cref="PagingSearchChannels"/> can be specified to request a custom paged result.
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>        
        public async Task<SearchChannelsPage> SearchChannelsPageAsync(string channel_name, PagingSearchChannels paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingSearchChannels();
            }

            RestRequest request = Request("search/channels", Method.GET);
            request.AddQueryParameter("query", channel_name.ToLower());
            request = paging.Add(request);

            IRestResponse<SearchChannelsPage> response = await client.ExecuteTaskAsync<SearchChannelsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of channels  based on the name query.
        /// <see cref="PagingSearchChannels"/> can be specified to request a custom paged result.        
        /// </summary>        
        public async Task<List<Channel>> SearchChannelsAsync(string channel_name)
        {
            PagingSearchChannels paging = new PagingSearchChannels();
            paging.limit = 100;

            List<Channel> search_channels = await Paging.GetPagesByTotalAsync<Channel, SearchChannelsPage, PagingSearchChannels>(SearchChannelsPageAsync, channel_name, paging, "channels");

            return search_channels;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of streams based on the search query.
        /// <see cref="PagingSearchChannels"/> can be specified to request a custom paged result.
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>
        public async Task<SearchStreamsPage> SearchStreamsPageAsync(string search, PagingSearchStreams paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingSearchStreams();
            }

            RestRequest request = Request("search/streams", Method.GET);
            request.AddQueryParameter("query", search.ToLower());
            request = paging.Add(request);

            IRestResponse<SearchStreamsPage> response = await client.ExecuteTaskAsync<SearchStreamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of streams based on the search query.
        /// <see cref="PagingSearchStreams"/> can be specified to request a custom paged result.        
        /// </summary>        
        public async Task<List<Stream>> SearchStreamsAsync(string search)
        {
            PagingSearchStreams paging = new PagingSearchStreams();
            paging.limit = 100;

            List<Stream> search_streams = await Paging.GetPagesByTotalAsync<Stream, SearchStreamsPage, PagingSearchStreams>(SearchStreamsPageAsync, search, paging, "streams");

            return search_streams;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all games based on the game query.
        /// When 'live' is set to 'true', only games that are live on at least one channel are returned. 
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>
        public async Task<SearchGames> SearchGamesAsync(string game_name, bool live = false)
        {
            RestRequest request = Request("search/games", Method.GET);
            request.AddQueryParameter("query", game_name);
            request.AddQueryParameter("live", live.ToString().ToLower());

            IRestResponse<SearchGames> response = await client.ExecuteTaskAsync<SearchGames>(request);

            return response.Data;
        }

        #endregion

        #region Streams                 DONE

        /// <summary>
        /// Asynchronously gets a stream object of the specified channel.
        /// </summary>
        public async Task<StreamResult> GetStreamAsync(string channel_id, StreamType stream_type = StreamType.LIVE)
        {
            RestRequest request = Request("streams/{channel_id}", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);
            request.AddQueryParameter("stream_type", stream_type.ToString().ToLower());

            IRestResponse<StreamResult> response = await client.ExecuteTaskAsync<StreamResult>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously checks to see if the channel associated with the chanel_id is live.
        /// </summary>
        public async Task<bool> isLiveAsync(string channel_id)
        {
            StreamResult stream = await GetStreamAsync(channel_id);

            return stream.stream.isNull();
        }

        /// <summary>
        /// Asynchronously gets how long the channel associated with the chanel_id has been live.
        /// Returns <see cref="TimeSpan.Zero"/> if the channel is offline.
        /// </summary>
        public async Task<TimeSpan> GetUpTimeAsync(string channel_id)
        {
            StreamResult stream = await GetStreamAsync(channel_id);

            return !stream.isNull() ? DateTime.Now.Subtract(stream.stream.created_at.ToLocalTime()) : TimeSpan.Zero;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of streams.
        /// <see cref="PagingStreams"/> can be specified to request a custom paged result.
        /// </summary>
        public async Task<StreamsPage> GetStreamsPageAsync(PagingStreams paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingStreams();
            }

            RestRequest request = Request("streams", Method.GET);
            request = paging.Add(request);

            IRestResponse<StreamsPage> response = await client.ExecuteTaskAsync<StreamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all streams.
        /// <see cref="PagingStreams"/> can be specified to request a custom result.        
        /// </summary>        
        public async Task<List<Stream>> GetStreamsAsync(PagingStreams paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingStreams();
                paging.limit = 100;
            }

            List<Stream> streams = await Paging.GetPagesByTotalAsync<Stream, StreamsPage, PagingStreams>(GetStreamsPageAsync, paging, "streams");

            return streams;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of the featured streams.
        /// <see cref="PagingFeaturedStreams"/> can be specified to request a custom paged result.
        /// </summary>
        public async Task<FeaturedStreamsPage> GetFeaturedStreamsPageAsync(PagingFeaturedStreams paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingFeaturedStreams();
            }

            RestRequest request = Request("streams/featured", Method.GET);
            request = paging.Add(request);

            IRestResponse<FeaturedStreamsPage> response = await client.ExecuteTaskAsync<FeaturedStreamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of the featured streams.
        /// </summary>        
        public async Task<List<FeaturedStream>> GetFeaturedStreamsAsync()
        {
            PagingFeaturedStreams paging = new PagingFeaturedStreams();
            paging.limit = 100;

            List<FeaturedStream> streams_featured = await Paging.GetPagesByOffsetAsync<FeaturedStream, FeaturedStreamsPage, PagingFeaturedStreams>(GetFeaturedStreamsPageAsync, paging, "featured");

            return streams_featured;
        }

        /// <summary>
        /// Asynchronously gets the how many channels are streaming and how many viewers are watching.
        /// If a game is specified, only results for that game will be returned.
        /// </summary>
        public async Task<StreamSummary> GetStreamsSummaryAsync(string game_name = null)
        {
            RestRequest request = Request("streams/summary", Method.GET);

            if (game_name.isValidString())
            {
                request.AddQueryParameter("game", game_name);
            }

            IRestResponse<StreamSummary> response = await client.ExecuteTaskAsync<StreamSummary>(request);

            return response.Data;
        }

        #endregion

        #region Teams                   DONE

        /// <summary>
        /// Asynchronously gets the <see cref="Team"/> object for a specified team name.
        /// NOTE: the team name is not always the same as the display name.
        /// </summary>        
        public async Task<Team> GetTeamAsync(string team_name)
        {
            RestRequest request = Request("teams/{team}", Method.GET);
            request.AddUrlSegment("team", team_name);

            IRestResponse<Team> response = await client.ExecuteTaskAsync<Team>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of the twitch teams.
        /// <see cref="PagingTeams"/> can be specified to request a custom paged result.
        /// </summary>
        public async Task<TeamsPage> GetTeamsPageAsync(PagingTeams paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingTeams();
                paging.limit = 100;
            }

            RestRequest request = Request("teams", Method.GET);
            request = paging.Add(request);

            IRestResponse<TeamsPage> response = await client.ExecuteTaskAsync<TeamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of the twitch teams.
        /// </summary>
        public async Task<List<TeamBase>> GetTeamsAsync()
        {
            PagingTeams paging = new PagingTeams();
            paging.limit = 100;

            List<TeamBase> teams = await Paging.GetPagesByOffsetAsync<TeamBase, TeamsPage, PagingTeams>(GetTeamsPageAsync, paging, "teams");

            return teams;
        }

        #endregion

        #region Users                   DONE

        /// <summary>
        /// Asynchronously gets the <see cref="User"/> object for a user.
        /// Required scope: 'user_read'
        /// </summary>
        public async Task<User> GetUserAsync(string user_id)
        {
            RestRequest request = Request("users/{user_id}", Method.GET);
            request.AddUrlSegment("user_id", user_id);

            IRestResponse<User> response = await client.ExecuteTaskAsync<User>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of channels a user is following.
        /// <see cref="PagingUserFollows"/> can be specified to request a custom paged result.
        /// </summary>
        public async Task<FollowsPage> GetUserFollowsPageAsync(string user_id, PagingUserFollows paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingUserFollows();
                paging.limit = 100;
            }

            RestRequest request = Request("users/{user_id}/follows/channels", Method.GET);
            request.AddUrlSegment("user_id", user_id);
            request = paging.Add(request);

            IRestResponse<FollowsPage> response = await client.ExecuteTaskAsync<FollowsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all channels a user is following sorted from newest oldest.
        /// </summary>
        public async Task<List<Follow>> GetUserFollowsAsync(string user_id, PagingUserFollows paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingUserFollows();
                paging.limit = 100;
            }

            List<Follow> following = await Paging.GetPagesByTotalAsync<Follow, FollowsPage, PagingUserFollows>(GetUserFollowsPageAsync, user_id, paging, "follows");

            return following;
        }

        /// <summary>
        /// Asynchronously gets the follower relationship between a user and a channel.
        /// Returns status '404' error message if the user is not following the channel.
        /// </summary>
        private async Task<Follow> GetUserFollowerRelationshipAsync(string user_id, string channel_id)
        {
            RestRequest request = Request("users/{user_id}/follows/channels/{channel_id}", Method.GET);
            request.AddUrlSegment("user_id", user_id);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<Follow> response = await client.ExecuteTaskAsync<Follow>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously checks if a user is following a channel.
        /// </summary>
        public async Task<bool> isUserFollowingAsync(string user_id, string channel_id)
        {
            Follow follow = await GetUserFollowerRelationshipAsync(user_id, channel_id);

            return follow.http_status != 404;
        }

        /// <summary>
        /// Asynchronously gets ther date when a user has been followed a channel.
        /// The date is converted to the broadcaster's local time zone.
        /// Returns <see cref="DateTime.MinValue"/> if the user is not following the channel.
        /// </summary>
        public async Task<DateTime> GetHowlongAsync(string user_id, string channel_id)
        {
            Follow follower = await GetUserFollowerRelationshipAsync(user_id, channel_id);

            return follower.http_status != 404 ? follower.created_at.ToLocalTime() : DateTime.MinValue;
        }

        #endregion

        #region Videos                  DONE

        /// <summary>
        /// Asynchronously gets the <see cref="Video"/> object for a specified video id. 
        /// The video id does not need to include the "v", but can also be included.
        /// </summary>
        public async Task<Video> GetVideoAsync(string video_id)
        {
            RestRequest request = Request("videos/{video_id}", Method.GET);
            request.AddUrlSegment("video_id", video_id);

            IRestResponse<Video> response = await client.ExecuteTaskAsync<Video>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of the top videos on twitch.
        /// <see cref="PagingTopVideos"/> can be specified to request a custom paged result.        
        /// </summary>        
        public async Task<TopVideosPage> GetTopVideosPageAsync(PagingTopVideos paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingTopVideos();
            }

            RestRequest request = Request("videos/top", Method.GET);
            request = paging.Add(request);

            IRestResponse<TopVideosPage> response = await client.ExecuteTaskAsync<TopVideosPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of the top videos on twitch.
        /// </summary>
        public async Task<List<Video>> GetTopVideosAsync(PagingTopVideos paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingTopVideos();
                paging.limit = 100;
            }

            List<Video> top_videos = await Paging.GetPagesByOffsetAsync<Video, TopVideosPage, PagingTopVideos>(GetTopVideosPageAsync, paging, "vods");

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