using System;
using System.Collections.Generic;

//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
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

/*
 * TODO: (API) Master todo list
 *      -   Go through each sync and async method and make sure all optional arguments are passed through (should already be correct, but just to make sure)
 *      -   Double check all GetPages() calls use the correct method (should already be correct, but just to make sure)
 *      -   Add in video upload endpoints and methods
 *      -   Implement custom exceptions for each failure case
 */

namespace TwitchLibrary.API
{
    public partial class TwitchApi : ITwitchRequest
    {
        #region Channels                DONE

        /// <summary>
        /// Gets the <see cref="Channel"/> object of a channel.
        /// </summary>
        public Channel GetChannel(string channel_id)
        {
            return GetChannelAsync(channel_id).Result;
        }

        /// <summary>
        /// Checks to see if the channel associated with the chanel_id is partnered.
        /// </summary>
        public bool isPartner(string channel_id)
        {
            return GetChannel(channel_id).partner;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of followers for a channel.
        /// <see cref="PagingChannelFollowers"/> can be specified to request a custom paged result.
        /// </summary>
        public FollowerPage GetChannelFollowersPage(string channel_id, PagingChannelFollowers paging = null)
        {
            return GetChannelFollowersPageAsync(channel_id, paging).Result;
        }

        /// <summary>
        /// Gets a complete list of all followers of a channel in descending order.
        /// </summary>
        public List<Follower> GetChannelFollowers(string channel_id, Direction direction = Direction.DESC)
        {
            return GetChannelFollowersAsync(channel_id, direction).Result;
        }

        /// <summary>
        /// Gets a complete list of all teams that a channel belongs to.
        /// </summary>
        public ChannelTeams GetChannelTeams(string channel_id)
        {
            return GetChannelTeamsAsync(channel_id).Result;
        }

        /// <summary>
        /// Gets a single paged list of videos for a channel.
        /// <see cref="PagingChannelVideos"/> can be specified to request a custom paged result.
        /// </summary>
        public VideosPage GetChannelVideosPage(string channel_id, PagingChannelVideos paging = null)
        {
            return GetChannelVideosPageAsync(channel_id, paging).Result;
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
        /// Gets a complete list of all the videos for a channel from oldest to newest.
        /// </summary>
        public List<Video> GetChannelVideos(string channel_id, Sort sort, BroadcastType[] broadcast_type)
        {
            return GetChannelVideosAsync(channel_id, sort, broadcast_type).Result;
        }

        #endregion

        #region Chat                    DONE

        /// <summary>
        /// Gets the chat badges that can be used in a channel.
        /// </summary>
        public Badges GetChatBadges(string channel_id)
        {
            return GetChatBadgesAsync(channel_id).Result;
        }

        /// <summary>
        /// Gets the emotes associated with an emoticon set.
        /// The emote data returned does not include images url's.
        /// </summary>
        public EmoteSet GetChatEmotes(string emote_set)
        {
            return GetChatEmotesAsync(emote_set).Result;
        }

        /// <summary>
        /// Gets a list of all valid emotes in Twitch.        
        /// The emote data returned does not include images url's.
        /// </summary>        
        public Emotes GetChatEmotes()
        {
            return GetChatEmotesAsync().Result;
        }


        /// <summary>
        /// Gets a list of all valid emotes in Twitch with their image link and size.
        /// </summary>        
        public EmoteImages GetChatEmoteImages()
        {
            return GetChatEmoteImagesAsync().Result;
        }

        #endregion

        #region Clips                   DONE

        /// <summary>
        /// Gets a single paged list of top clips on Twitch from highest to lowest view count.
        /// <see cref="PagingClips"/> can be specified to request a custom paged result.
        /// </summary>
        public ClipsPage GetTopClipsPage(PagingClips paging = null)
        {
            return GetTopClipsPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of top clips on Twitch from highest to lowest view count.     
        /// <see cref="PagingClips"/> can be specified to request a custom list.
        /// </summary>  
        public List<Clip> GetTopClips(PagingClips paging = null)
        {
            return GetTopClipsAsync(paging).Result;
        }

        /// <summary>
        /// Gets the information of a specified clip for a specific channel
        /// </summary>
        public Clip GetClip(string channel_name, string slug)
        {
            return GetClipAsync(channel_name, slug).Result;
        }

        #endregion

        #region Games                   DONE

        /// <summary>
        /// Gets a single paged list of top games currently being played on Twitch from highest to lowest view count.
        /// <see cref="PagingTopGames"/> can be specified to request a custom paged result.
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>        
        public TopGamesPage GetTopGamesPage(PagingTopGames paging = null)
        {
            return GetTopGamesPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of top games currently being played on Twitch from highest to lowest view count.        
        /// </summary>        
        public List<TopGame> GetTopGames()
        {
            return GetTopGamesAsync().Result;
        }

        #endregion

        #region Ingests                 DONE

        /// <summary>
        /// Gets a complete list of all Twitch servers to connect to.
        /// </summary>        
        public Ingests GetIngests()
        {
            return GetIngestsAsync().Result;
        }

        #endregion

        #region Search                  DONE

        /// <summary>
        /// Gets a single paged list of channels based on the name query.
        /// <see cref="PagingSearchChannels"/> can be specified to request a custom paged result.
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>        
        public SearchChannelsPage SearchChannelsPage(string channel_name, PagingSearchChannels paging = null)
        {
            return SearchChannelsPageAsync(channel_name, paging).Result;
        }

        /// <summary>
        /// Gets a complete list of channels  based on the name query.
        /// <see cref="PagingSearchChannels"/> can be specified to request a custom paged result.        
        /// </summary>        
        public List<Channel> SearchChannels(string channel_name)
        {
            return SearchChannelsAsync(channel_name).Result;
        }

        /// <summary>
        /// Gets a single paged list of streams based on the search query.
        /// <see cref="PagingSearchChannels"/> can be specified to request a custom paged result.
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>
        public SearchStreamsPage SearchStreamsPage(string search, PagingSearchStreams paging = null)
        {
            return SearchStreamsPageAsync(search, paging).Result;
        }

        /// <summary>
        /// Gets a complete list of streams based on the search query.
        /// <see cref="PagingSearchStreams"/> can be specified to request a custom paged result.        
        /// </summary>        
        public List<Stream> SearchStreams(string search)
        {
            return SearchStreamsAsync(search).Result;
        }

        /// <summary>
        /// Gets a complete list of all games based on the game query.
        /// When 'live' is set to 'true', only games that are live on at least one channel are returned. 
        /// Returns status '503' if the list cannot be retrieved.
        /// </summary>
        public SearchGames SearchGames(string game_name, bool live = false)
        {
            return SearchGamesAsync(game_name, live).Result;
        }

        #endregion

        #region Streams                 DONE

        /// <summary>
        /// Gets a stream object of the specified channel.
        /// </summary>
        public StreamResult GetStream(string channel_id, StreamType stream_type = StreamType.LIVE)
        {
            return GetStreamAsync(channel_id).Result;
        }

        /// <summary>
        /// Checks to see if the channel associated with the chanel_id is live.
        /// </summary>
        public bool isLive(string channel_id)
        {
            return isLiveAsync(channel_id).Result;
        }

        /// <summary>
        /// Gets how long the channel associated with the chanel_id has been live.
        /// Returns <see cref="TimeSpan.Zero"/> if the channel is offline.
        /// </summary>
        public TimeSpan GetUpTime(string channel_id)
        {
            return GetUpTimeAsync(channel_id).Result;
        }

        /// <summary>
        /// Gets a single paged list of streams.
        /// <see cref="PagingStreams"/> can be specified to request a custom paged result.
        /// </summary>
        public StreamsPage GetStreamsPage(PagingStreams paging = null)
        {
            return GetStreamsPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of all streams.
        /// <see cref="PagingStreams"/> can be specified to request a custom result.        
        /// </summary>        
        public List<Stream> GetStreams(PagingStreams paging = null)
        {
            return GetStreamsAsync(paging).Result;
        }

        /// <summary>
        /// Gets a single paged list of the featured streams.
        /// <see cref="PagingFeaturedStreams"/> can be specified to request a custom paged result.
        /// </summary>
        public FeaturedStreamsPage GetFeaturedStreamsPage(PagingFeaturedStreams paging = null)
        {
            return GetFeaturedStreamsPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of the featured streams.
        /// </summary>        
        public List<FeaturedStream> GetFeaturedStreams()
        {
            return GetFeaturedStreamsAsync().Result;
        }

        /// <summary>
        /// Gets the how many channels are streaming and how many viewers are watching.
        /// If a game is specified, only results for that game will be returned.
        /// </summary>
        public StreamSummary GetStreamsSummary(string game_name = null)
        {
            return GetStreamsSummaryAsync(game_name).Result;
        }

        #endregion

        #region Teams                   DONE

        /// <summary>
        /// Gets the <see cref="Team"/> object for a specified team name.
        /// NOTE: the team name is not always the same as the display name.
        /// </summary>        
        public Team GetTeam(string team_name)
        {
            return GetTeamAsync(team_name).Result;
        }

        /// <summary>
        /// Gets a single paged list of the twitch teams.
        /// <see cref="PagingTeams"/> can be specified to request a custom paged result.
        /// </summary>
        public TeamsPage GetTeamsPage(PagingTeams paging = null)
        {
            return GetTeamsPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of the twitch teams.
        /// </summary>
        public List<TeamBase> GetTeams()
        {
            return GetTeamsAsync().Result;
        }

        #endregion

        #region Users                   DONE

        /// <summary>
        /// Gets the <see cref="User"/> object for a user.
        /// Required scope: 'user_read'
        /// </summary>
        public User GetUser(string user_id)
        {
            return GetUserAsync(user_id).Result;
        }

        /// <summary>
        /// Gets a single paged list of channels a user is following.
        /// <see cref="PagingUserFollows"/> can be specified to request a custom paged result.
        /// </summary>
        public FollowsPage GetUserFollowsPage(string user_id, PagingUserFollows paging = null)
        {
            return GetUserFollowsPageAsync(user_id, paging).Result;
        }

        /// <summary>
        /// Gets a complete list of all channels a user is following sorted from newest oldest.
        /// </summary>
        public List<Follow> GetUserFollows(string user_id, PagingUserFollows paging = null)
        {
            return GetUserFollowsAsync(user_id, paging).Result;
        }

        /// <summary>
        /// Gets the follower relationship between a user and a channel.
        /// Returns status '404' error message if the user is not following the channel.
        /// </summary>
        private Follow GetUserFollowerRelationship(string user_id, string channel_id)
        {
            return GetUserFollowerRelationshipAsync(user_id, channel_id).Result;
        }

        /// <summary>
        /// Checks if a user is following a channel.
        /// </summary>
        public bool isUserFollowing(string user_id, string channel_id)
        {
            return isUserFollowingAsync(user_id, channel_id).Result;
        }

        /// <summary>
        /// Gets ther date when a user has been followed a channel.
        /// The date is converted to the broadcaster's local time zone.
        /// Returns <see cref="DateTime.MinValue"/> if the user is not following the channel.
        /// </summary>
        public DateTime GetHowlong(string user_id, string channel_id)
        {
            return GetHowlongAsync(user_id, channel_id).Result;
        }

        #endregion

        #region Videos                  DONE

        /// <summary>
        /// Gets the <see cref="Video"/> object for a specified video id. 
        /// The video id does not need to include the "v", but can also be included.
        /// </summary>
        public Video GetVideo(string video_id)
        {
            return GetVideoAsync(video_id).Result;
        }

        /// <summary>
        /// Gets a single paged list of the top videos on twitch.
        /// <see cref="PagingTopVideos"/> can be specified to request a custom paged result.        
        /// </summary>        
        public TopVideosPage GetTopVideosPage(PagingTopVideos paging = null)
        {
            return GetTopVideosPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of the top videos on twitch.
        /// </summary>
        public List<Video> GetTopVideos(PagingTopVideos paging = null)
        {
            return GetTopVideosAsync(paging).Result;
        }

        #endregion        
    }
}
