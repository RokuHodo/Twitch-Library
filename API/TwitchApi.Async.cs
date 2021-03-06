﻿// standard namespaces
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

// project namespaces
using TwitchLibrary.Enums.API;
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Json;
using TwitchLibrary.Helpers.Paging;
using TwitchLibrary.Helpers.Paging.Channels;
using TwitchLibrary.Helpers.Paging.Clips;
using TwitchLibrary.Helpers.Paging.Collections;
using TwitchLibrary.Helpers.Paging.Communities;
using TwitchLibrary.Helpers.Paging.Games;
using TwitchLibrary.Helpers.Paging.Search;
using TwitchLibrary.Helpers.Paging.Streams;
using TwitchLibrary.Helpers.Paging.Teams;
using TwitchLibrary.Helpers.Paging.Users;
using TwitchLibrary.Helpers.Paging.Videos;
using TwitchLibrary.Interfaces.API;
using TwitchLibrary.Models.API.Bits;
using TwitchLibrary.Models.API.Channels;
using TwitchLibrary.Models.API.Chat;
using TwitchLibrary.Models.API.Clips;
using TwitchLibrary.Models.API.Collections;
using TwitchLibrary.Models.API.Community;
using TwitchLibrary.Models.API.Games;
using TwitchLibrary.Models.API.Ingests;
using TwitchLibrary.Models.API.Search;
using TwitchLibrary.Models.API.Streams;
using TwitchLibrary.Models.API.Teams;
using TwitchLibrary.Models.API.Users;
using TwitchLibrary.Models.API.Videos;

// imported .dll's
using RestSharp;

namespace TwitchLibrary.API
{
    public partial class TwitchApi : ITwitchRequest
    {
        protected string        client_id;
        protected string        oauth_token;

        protected RestClient    twitch_api_client;
        protected RestClient    uploads_api_client;

        public TwitchApi(string _client_id, string _oauth_token = "")
        {
            client_id   = _client_id;
            oauth_token = _oauth_token;
                  
            // generic twitch api endpoints
            twitch_api_client = new RestClient("https://api.twitch.tv/kraken");                                    
            twitch_api_client.AddHandler("application/json", new CustomJsonDeserializer());
            twitch_api_client.AddHandler("application/xml", new CustomJsonDeserializer());
            twitch_api_client.AddDefaultHeader("Accept", "application/vnd.twitchtv.v5+json");

            // video upload endpoints
            uploads_api_client = new RestClient("https://uploads.twitch.tv/upload");
            uploads_api_client.AddHandler("application/json", new CustomJsonDeserializer());
            uploads_api_client.AddDefaultHeader("Accept", "application/vnd.twitchtv.v4+json");
            uploads_api_client.AddDefaultHeader("Content-Type", "application/bson");
        }

        #region Bits

        /// <summary>
        /// Asynchronously gets a list of available cheermotes that can be used in chat.
        /// The cheermotes returned are available in all bits-enabled channels.
        /// If the "channel_id" is included, the custom cheermotes for the specified channel is included if applicable and valid.
        /// </summary>
        /// <returns></returns>
        public async Task<Cheermotes> GetCheermotesAsync(string channel_id = "")
        {
            RestRequest request = Request("bits/actions", Method.GET);
            if (channel_id.isValid())
            {
                request.AddQueryParameter("channel_id", channel_id);
            }

            IRestResponse<Cheermotes> response = await twitch_api_client.ExecuteTaskAsync<Cheermotes>(request);

            return response.Data;
        }

        #endregion

        #region Channels

        /// <summary>
        /// Asynchronously gets the <see cref="Channel"/> object of a channel.
        /// </summary>
        public async Task<Channel> GetChannelAsync(string channel_id)
        {
            RestRequest request = Request("channels/{channel_id}", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<Channel> response = await twitch_api_client.ExecuteTaskAsync<Channel>(request);

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

            IRestResponse<FollowerPage> response = await twitch_api_client.ExecuteTaskAsync<FollowerPage>(request);

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

            IRestResponse<ChannelTeams> response = await twitch_api_client.ExecuteTaskAsync<ChannelTeams>(request);

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

            IRestResponse<VideosPage> response = await twitch_api_client.ExecuteTaskAsync<VideosPage>(request);

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

        /// <summary>
        /// Asynchronously gets the community that a channel belongs to.
        /// </summary>
        public async Task<Community> GetChannelCommunityAsync(string channel_id)
        {
            RestRequest request = Request("channels/{channel_id}/community", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<Community> response = await twitch_api_client.ExecuteTaskAsync<Community>(request);

            return response.Data;
        }        

        #endregion

        #region Chat

        /// <summary>
        /// Asynchronously gets the chat badges that can be used in a channel.
        /// </summary>
        public async Task<Badges> GetChatBadgesAsync(string channel_id)
        {
            RestRequest request = Request("chat/{channel_id}/badges", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<Badges> response = await twitch_api_client.ExecuteTaskAsync<Badges>(request);

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

            IRestResponse<EmoteSet> response = await twitch_api_client.ExecuteTaskAsync<EmoteSet>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a list of all valid emotes in Twitch.        
        /// The emote data returned does not include images url's.
        /// </summary>        
        public async Task<Emotes> GetChatEmotesAsync()
        {
            RestRequest request = Request("chat/emoticon_images", Method.GET);

            IRestResponse<Emotes> response = await twitch_api_client.ExecuteTaskAsync<Emotes>(request);

            return response.Data;
        }


        /// <summary>
        /// Asynchronously gets a list of all valid emotes in Twitch with their image link and size.
        /// </summary>        
        public async Task<EmoteImages> GetChatEmoteImagesAsync()
        {
            RestRequest request = Request("chat/emoticons", Method.GET);

            IRestResponse<EmoteImages> response = await twitch_api_client.ExecuteTaskAsync<EmoteImages>(request);

            return response.Data;
        }

        #endregion

        #region Clips

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

            RestRequest request = Request("clips/top", Method.GET);
            request = paging.Add(request);

            IRestResponse<ClipsPage> response = await twitch_api_client.ExecuteTaskAsync<ClipsPage>(request);

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
        /// Asynchronously gets the information of a specified clip.
        /// </summary>
        public async Task<Clip> GetClipAsync(string channel_name, string slug)
        {
            RestRequest request = Request("clips/{slug}", Method.GET);
            request.AddUrlSegment("channel_name", channel_name);
            request.AddUrlSegment("slug", slug);

            IRestResponse<Clip> response = await twitch_api_client.ExecuteTaskAsync<Clip>(request);

            return response.Data;
        }

        #endregion

        #region Collections

        /// <summary>
        /// Asynchronously gets the metadata for a specified collection.
        /// </summary>
        public async Task<CollectionMetadata> GetCollectionMetadataAsync(string collection_id)
        {
            RestRequest request = Request("collections/{collection_id}", Method.GET);
            request.AddUrlSegment("collection_id", collection_id);

            IRestResponse<CollectionMetadata> response = await twitch_api_client.ExecuteTaskAsync<CollectionMetadata>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a specific collection.
        /// If "include_all_items" is set to true, all unwatchable VODs (private and/or in-process) are included in the response.
        /// </summary>
        public async Task<Collection> GetCollectionAsync(string collection_id, bool include_all_items = false)
        {
            RestRequest request = Request("collections/{collection_id}/items", Method.GET);
            request.AddUrlSegment("collection_id", collection_id);
            request.AddUrlSegment("include_all_items", include_all_items.ToString());

            IRestResponse<Collection> response = await twitch_api_client.ExecuteTaskAsync<Collection>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of collections that a channel has created.
        /// <see cref="PagingChannelCollections"/> can be specified to request a custom paged result.
        /// </summary>
        public async Task<ChannelCollectionsPage> GetChannelCollectionsPageAsync(string channel_id, PagingChannelCollections paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelCollections();
            }

            RestRequest request = Request("channels/{channel_id}/collections", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);
            request = paging.Add(request);

            IRestResponse<ChannelCollectionsPage> response = await twitch_api_client.ExecuteTaskAsync<ChannelCollectionsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of collections that a channel has created.
        /// If "video_id" is included, only colelctions containing the video will be returned.
        /// </summary>
        public async Task<List<CollectionMetadata>> GetChannelCollectionsAsync(string channel_id, string video_id = "")
        {
            PagingChannelCollections paging = new PagingChannelCollections();
            paging.limit = 100;
            paging.containing_item = video_id;

            List<CollectionMetadata> collections = await Paging.GetPagesByCursorAsync<CollectionMetadata, ChannelCollectionsPage, PagingChannelCollections>(GetChannelCollectionsPageAsync, channel_id, paging, "collections");

            return collections;
        }

        #endregion

        #region Communities

        /// <summary>
        /// Asynchronously gets a <see cref="Community"/> by its name.
        /// The name must be 3 to 25 characters.
        /// </summary>
        public async Task<Community> GetCommunityByNameAsync(string community_name)
        {
            // string is less than minumim length, return an empty model
            if(community_name.Length < 3 || community_name.Length > 25)
            {
                return default(Community);
            }

            RestRequest request = Request("communities", Method.GET);
            request.AddQueryParameter("name", community_name.ToLower());

            IRestResponse<Community> response = await twitch_api_client.ExecuteTaskAsync<Community>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a <see cref="Community"/> by its ID.
        /// </summary>
        public async Task<Community> GetCommunityByIDAsync(string community_id)
        {
            RestRequest request = Request("communities/{community_id}", Method.GET);
            request.AddUrlSegment("community_id", community_id);

            IRestResponse<Community> response = await twitch_api_client.ExecuteTaskAsync<Community>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of top communities based on views.
        /// <see cref="PagingTopCommunities"/> can be specified to request a custom paged result.
        /// </summary>
        public async Task<TopCommunityPage> GetTopCommunitiesPageAsync(PagingTopCommunities paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingTopCommunities();
            }

            RestRequest request = Request("communities/top", Method.GET);
            request = paging.Add(request);

            IRestResponse<TopCommunityPage> response = await twitch_api_client.ExecuteTaskAsync<TopCommunityPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of top communities based on views.
        /// </summary>
        public async Task<List<TopCommunity>> GetTopCommunitiesAsync()
        {
            PagingTopCommunities paging = new PagingTopCommunities();
            paging.limit = 100;

            List<TopCommunity> communities = await Paging.GetPagesByCursorAsync<TopCommunity, TopCommunityPage, PagingTopCommunities>(GetTopCommunitiesPageAsync, paging, "communities");
            
            return communities;
        }

        /// <summary>
        /// Asynchronously gets a complete list of a community's moderators.
        /// </summary>
        public async Task<CommunityModerators> GetCommunityModeratorsAsync(string community_id)
        {
            RestRequest request = Request("communities/{community_id}/moderators", Method.GET);
            request.AddUrlSegment("community_id", community_id);

            IRestResponse<CommunityModerators> response = await twitch_api_client.ExecuteTaskAsync<CommunityModerators>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously reports a channel for violating community rules.
        /// Returns status '204' if the operation was successful.
        /// </summary>
        public async Task<HttpStatusCode> ReportCommunityViolationAsync(string community_id, string channel_id)
        {
            RestRequest request = Request("communities/{community_id}/report_channel", Method.POST);
            request.AddUrlSegment("community_id", community_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel_id });

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }        

        #endregion

        #region Games

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

            IRestResponse<TopGamesPage> response = await twitch_api_client.ExecuteTaskAsync<TopGamesPage>(request);

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

        #region Ingests

        /// <summary>
        /// Asynchronously gets a complete list of all Twitch servers to connect to.
        /// </summary>        
        public async Task<Ingests> GetIngestsAsync()
        {
            RestRequest request = Request("ingests", Method.GET);

            IRestResponse<Ingests> response = await twitch_api_client.ExecuteTaskAsync<Ingests>(request);

            return response.Data;
        }

        #endregion

        #region Search

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

            IRestResponse<SearchChannelsPage> response = await twitch_api_client.ExecuteTaskAsync<SearchChannelsPage>(request);

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

            IRestResponse<SearchStreamsPage> response = await twitch_api_client.ExecuteTaskAsync<SearchStreamsPage>(request);

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

            IRestResponse<SearchGames> response = await twitch_api_client.ExecuteTaskAsync<SearchGames>(request);

            return response.Data;
        }

        #endregion

        #region Streams

        /// <summary>
        /// Asynchronously gets a stream object of the specified channel.
        /// </summary>
        public async Task<StreamResult> GetStreamAsync(string channel_id, StreamType stream_type = StreamType.LIVE)
        {
            RestRequest request = Request("streams/{channel_id}", Method.GET);
            request.AddUrlSegment("channel_id", channel_id);
            request.AddQueryParameter("stream_type", stream_type.ToString().ToLower());

            IRestResponse<StreamResult> response = await twitch_api_client.ExecuteTaskAsync<StreamResult>(request);

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

            IRestResponse<StreamsPage> response = await twitch_api_client.ExecuteTaskAsync<StreamsPage>(request);

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

            IRestResponse<FeaturedStreamsPage> response = await twitch_api_client.ExecuteTaskAsync<FeaturedStreamsPage>(request);

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

            if (game_name.isValid())
            {
                request.AddQueryParameter("game", game_name);
            }

            IRestResponse<StreamSummary> response = await twitch_api_client.ExecuteTaskAsync<StreamSummary>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of all streams currently streaming in a community.
        /// <see cref="PagingCommunityStreams"/> can be specified to request a custom paged result.
        /// </summary>
        public async Task<CommunityStreamsPage> GetCommunityStreamsPageAsync(string community_id, PagingCommunityStreams paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingCommunityStreams();
            }

            RestRequest request = Request("streams", Method.GET);
            request.AddQueryParameter("community_id", community_id);
            request = paging.Add(request);

            IRestResponse<CommunityStreamsPage> response = await twitch_api_client.ExecuteTaskAsync<CommunityStreamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all streams currently streaming in a community.
        /// </summary>
        public async Task<List<Stream>> GetCommunityStreamsAsync(string community_id)
        {
            PagingCommunityStreams paging = new PagingCommunityStreams();
            paging.limit = 100;

            List<Stream> response = await Paging.GetPagesByTotalAsync<Stream, CommunityStreamsPage, PagingCommunityStreams>(GetCommunityStreamsPageAsync, community_id, paging, "streams");

            return response;
        }

        #endregion

        #region Teams

        /// <summary>
        /// Asynchronously gets the <see cref="Team"/> object for a specified team name.
        /// NOTE: The team name is not always the same as the display name.
        /// </summary>        
        public async Task<Team> GetTeamAsync(string team_name)
        {
            RestRequest request = Request("teams/{team}", Method.GET);
            request.AddUrlSegment("team", team_name);

            IRestResponse<Team> response = await twitch_api_client.ExecuteTaskAsync<Team>(request);

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

            IRestResponse<TeamsPage> response = await twitch_api_client.ExecuteTaskAsync<TeamsPage>(request);

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

        #region Users

        /// <summary>
        /// Asynchronously gets the <see cref="User"/> object for a user.
        /// Required scope: 'user_read'
        /// </summary>
        public async Task<User> GetUserAsync(string user_id)
        {
            RestRequest request = Request("users/{user_id}", Method.GET);
            request.AddUrlSegment("user_id", user_id);

            IRestResponse<User> response = await twitch_api_client.ExecuteTaskAsync<User>(request);

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

            IRestResponse<FollowsPage> response = await twitch_api_client.ExecuteTaskAsync<FollowsPage>(request);

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

            IRestResponse<Follow> response = await twitch_api_client.ExecuteTaskAsync<Follow>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously checks if a user is following a channel.
        /// </summary>
        public async Task<bool> isUserFollowingAsync(string user_id, string channel_id)
        {
            Follow follow = await GetUserFollowerRelationshipAsync(user_id, channel_id);

            return follow.status_code != 404;
        }

        /// <summary>
        /// Asynchronously gets ther date when a user has been followed a channel.
        /// The date is converted to the broadcaster's local time zone.
        /// Returns <see cref="DateTime.MinValue"/> if the user is not following the channel.
        /// </summary>
        public async Task<DateTime> GetHowlongAsync(string user_id, string channel_id)
        {
            Follow follower = await GetUserFollowerRelationshipAsync(user_id, channel_id);

            return follower.status_code != 404 ? follower.created_at.ToLocalTime() : DateTime.MinValue;
        }

        #endregion

        #region Videos

        /// <summary>
        /// Asynchronously gets the <see cref="Video"/> object for a specified video id. 
        /// The video id does not need to include the "v", but can also be included.
        /// </summary>
        public async Task<Video> GetVideoAsync(string video_id)
        {
            RestRequest request = Request("videos/{video_id}", Method.GET);
            request.AddUrlSegment("video_id", video_id);

            IRestResponse<Video> response = await twitch_api_client.ExecuteTaskAsync<Video>(request);

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

            IRestResponse<TopVideosPage> response = await twitch_api_client.ExecuteTaskAsync<TopVideosPage>(request);

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

            if (oauth_token.isValid())
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