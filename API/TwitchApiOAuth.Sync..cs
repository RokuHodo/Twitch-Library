﻿using System.Collections.Generic;
using System.Net;

//project namespaces
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Helpers.Paging.Channels;
using TwitchLibrary.Helpers.Paging.Clips;
using TwitchLibrary.Helpers.Paging.Feed;
using TwitchLibrary.Helpers.Paging.Streams;
using TwitchLibrary.Helpers.Paging.Users;
using TwitchLibrary.Helpers.Paging.Videos;
using TwitchLibrary.Interfaces.API;
using TwitchLibrary.Models.API.Channels;
using TwitchLibrary.Models.API.Clips;
using TwitchLibrary.Models.API.Chat;
using TwitchLibrary.Models.API.Feed;
using TwitchLibrary.Models.API.Streams;
using TwitchLibrary.Models.API.Users;
using TwitchLibrary.Models.API.Videos;

//imported .dll's

namespace TwitchLibrary.API
{
    public partial class TwitchApiOAuth : TwitchApi, ITwitchRequest
    {
        #region Channels                    DONE

        /// <summary>
        /// Gets the <see cref="ChannelOAuth"/> object associated with the client's authentication token.
        /// Required scope: 'channel_read'
        /// </summary>
        public ChannelOAuth GetChannel()
        {
            return GetChannelAsync().Result;
        }

        /// <summary>
        /// Sets the title of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public ChannelOAuth SetTitle(string status)
        {
            return SetTitleAsync(status).Result;
        }

        /// <summary>
        /// Sets the game of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public ChannelOAuth SetGame(string game)
        {
            return SetGameAsync(game).Result;
        }

        /// <summary>
        /// Sets the delay of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public ChannelOAuth SetDelay(int delay)
        {
            return SetDelayAsync(delay).Result;
        }

        /// <summary>
        /// Enables or disables the channel feed of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public ChannelOAuth EnableChannelFeed(bool channel_feed_enabled)
        {
            return EnableChannelFeedAsync(channel_feed_enabled).Result;
        }

        /// <summary>
        /// Gets a complete list of all editors for the channel associated with the client's authentication token.
        /// Required scope: 'channel_read'
        /// </summary>
        public Editors GetChannelEditors()
        {
            return GetChannelEditorsAsync().Result;
        }

        /// <summary>
        /// Gets a single paged list of subscribers for the channel associated with the client's authentication token.
        /// <see cref="PagingChannelFollowers"/> can be specified to request a custom paged result.
        /// Required scope: 'channel_subscriptions'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>
        public ChannelSubscribersPage GetChannelSubscribersPage(PagingChannelSubscribers paging = null)
        {
            return GetChannelSubscribersPageAsync().Result;
        }

        /// <summary>
        /// Gets a complete list of all subscribers for the channel associated with the client's authentication token in ascending order.
        /// Required scope: channel_subscriptions
        /// NOTE: Works, in theory, can't test because I'm not a partner.
        /// </summary>
        public List<ChannelSubscriber> GetChannelSubscribers(Direction direction = Direction.ASC)
        {
            return GetChannelSubscribersAsync(direction).Result;
        }

        /// <summary>
        /// Gets the subscriber relationship the channel associated with the client's authentication token and a user.
        /// Returns status '404' if the user is not subscribed to the channel.
        /// Required scope: 'channel_check_subscription'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>
        public ChannelSubscriber GetChannelSubscriberRelationship(string user_id)
        {
            return GetChannelSubscriberRelationshipAsync(user_id).Result;
        }

        /// <summary>
        /// Intended for use by the the channel owner.
        /// Checks to see if a user is subsdcribed to the channel associated with the client's authentication token.        
        /// Required scope: 'channel_check_subscription'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>        
        public bool isUserSubscribed(string user_id)
        {
            return isUserSubscribedAsync(user_id).Result;
        }

        /// <summary>
        /// Starts a commercial for the channel associated with the client's authentication token.        
        /// Returns status '422' if an invalid length is specified, an attempt is made to start a commercial less than 8 minutes after the previous commercial, or the specified channel is not a Twitch partner.
        /// If the operation was sucessful, the <see cref="CommercialResponse"/> object is returned. 
        /// Required scope: 'channel_commercial'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>
        public CommercialResponse StartCommercial(CommercialLength length)
        {
            return StartCommercialAsync(length).Result;
        }

        /// <summary>
        /// Resets the stream key for the channel associated with the client's authentication token.
        /// Required scope: 'channel_stream'
        /// </summary>
        public ChannelOAuth ResetStreamKey()
        {
            return ResetStreamKeyAsync().Result;
        }

        #endregion

        #region Clips                       DONE

        /// <summary>
        /// Gets a single paged list of clips from the games the user associated with the client's authentication token is following highest to lowest view count.
        /// <see cref="PagingClips"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>
        public ClipsPage GetUserGamesFollowedClipsPage(PagingClipsGamesFollowed paging = null)
        {
            return GetUserGamesFollowedClipsPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of clips from the games the user associated with the client's authentication token is following highest to lowest view count.
        /// <see cref="PagingClips"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>
        public List<Clip> GetUserGamesFollowedClips(PagingClipsGamesFollowed paging = null)
        {
            return GetUserGamesFollowedClipsAsync(paging).Result;
        }

        #endregion

        #region Feed                        DONE

        /// <summary>
        /// Gets a single paged list of posts in the feed of the channel associated with the client's authentication token.
        /// <see cref="PagingChannelFeedPosts"/> can be specified to request a custom paged result.
        /// Required scope: 'any'       
        /// </summary>
        public PostsPage GetChannelFeedPostsPage(PagingChannelFeedPosts paging = null)
        {
            return GetChannelFeedPostsPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of posts in the feed of the channel associated with the client's authentication token from newest to oldest.        
        /// Required scope: 'any'        
        /// </summary>
        public List<Post> GetChannelFeedPosts()
        {
            return GetChannelFeedPostsAsync().Result;
        }

        /// <summary>
        /// Gets a post in the feed of the channel associated with the client's authentication token.                
        /// Required scope: 'any'   
        /// </summary>
        public Post GetChannelFeedPost(string post_id, int comments = 5)
        {
            return GetChannelFeedPostAsync(post_id, comments).Result;
        }

        /// <summary>
        /// Creates a post in the feed of the channel associated with the client's authentication token.
        /// If share is set to true and the channel has a ocnnected twitter account, the post will be also tweeted.
        /// If the operation was sucessful, the <see cref="CreatedPost"/> object is returned. 
        /// Required scope: 'channel_feed_edit' 
        /// </summary>
        public CreatedPost CreateChannelFeedPost(string content, bool share = false)
        {
            return CreateChannelFeedPostAsync(content, share).Result;
        }

        /// <summary>
        /// Deletes a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Post"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public Post DeleteChannelFeedPost(string post_id)
        {
            return DeleteChannelFeedPostAsync(post_id).Result;
        }

        /// <summary>
        /// Creates a reaction to a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="CreateReactionResponse"/> object is returned;. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public CreateReactionResponse CreateChannelFeedPostReaction(string post_id, string emote_id = "endorse")
        {
            return CreateChannelFeedPostReactionAsync(post_id, emote_id).Result;
        }

        /// <summary>
        /// Deletes a reaction to a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="DeleteReactionResponse"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public DeleteReactionResponse DeleteChannelFeedPostReaction(string post_id, string emote_id)
        {
            return DeleteChannelFeedPostReactionAsync(post_id, emote_id).Result;
        }

        /// <summary>
        /// Gets a single paged list of comments to a post in the feed of the channel associated with the client's authentication token.
        /// <see cref="PagingFeedPostComments"/> can be specified to request a custom paged result.
        /// Required scope: 'any'
        /// </summary>
        public PostCommentsPage GetChannelFeedPostCommentsPage(string post_id, PagingChannelFeedPostComments paging = null)
        {
            return GetChannelFeedPostCommentsPageAsync(post_id, paging).Result;
        }

        /// <summary>
        /// Gets a complete list of comments in a post in the feed of the channel associated with the client's authentication token.        
        /// Required scope: 'any'
        /// </summary>        
        public List<Comment> GetChannelFeedPostComments(string post_id)
        {   
            return GetChannelFeedPostCommentsAsync(post_id).Result;
        }

        /// <summary>
        /// Creates a comment on a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Comment"/> object is returned. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public Comment CreateChannelFeedPostComment(string post_id, string content)
        {
            return CreateChannelFeedPostCommentAsync(post_id, content).Result;
        }

        /// <summary>
        /// Deletes a comment on a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Comment"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public Comment DeleteChannelFeedPostComment(string post_id, string comment_id)
        {
            return DeleteChannelFeedPostCommentAsync(post_id, comment_id).Result;
        }

        /// <summary>
        /// Creates a reaction to a comment in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="CreateReactionResponse"/> object is returned. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public CreateReactionResponse CreateChannelFeedPostCommentReaction(string post_id, string comment_id, string emote_id)
        {
            return CreateChannelFeedPostCommentReactionAsync(post_id, comment_id, emote_id).Result;
        }

        /// <summary>
        /// Deletes a reaction to a comment in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="DeleteReactionResponse"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public DeleteReactionResponse DeleteChannelFeedPostCommentReaction(string post_id, string comment_id, string emote_id)
        {
            return DeleteChannelFeedPostCommentReactionAsync(post_id, comment_id, emote_id).Result;
        }

        #endregion

        #region Streams                     DONE

        /// <summary>
        /// Gets a single paged list of streams that the channel associated with the client's authentication token is following.
        /// <see cref="PagingStreamFollows"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>        
        public StreamsPage GetStreamFollowsPage(PagingStreamFollows paging = null)
        {
            return GetStreamFollowsPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of streams that the channel associated with the client's authentication token is following.        
        /// Required scope: 'user_read'
        /// </summary>        
        public List<Stream> GetStreamFollows(StreamType stream_type = StreamType.LIVE)
        {
            return GetStreamFollowsAsync(stream_type).Result;
        }

        #endregion

        #region Users                       DONE

        /// <summary>
        /// Gets the <see cref="UserOAuth"/> object associated with the client's authentication token.
        /// Required scope: 'user_read'
        /// </summary>
        public UserOAuth GetUser()
        {
            return GetUserAsync().Result;
        }

        /// <summary>
        /// Gets all emote sets and sub emotes that the user associated with the client's authentication token can use in chat.
        /// Required scope: 'user_subscriptions'
        /// </summary>
        public EmoteSet GetUserEmotes()
        {
            return GetUserEmotesAsync().Result;
        }

        /// <summary>
        /// Gets the subscriber relationship between a user and the channel associated with the client's authentication token.        
        /// Returns status '403' if the oauth token doesn't have proper authorization to make the request.
        /// Returns status '404' if the user is not subscribed to the channel.
        /// Returns status '422' if the channel does not have a subscription program.
        /// Required scope: 'user_subscriptions'    
        /// </summary>
        private UserSubscriber GetUserSubscriberRelationship(string user_id, string channel_id)
        {
            return GetUserSubscriberRelationshipAsync(user_id, channel_id).Result;
        }

        /// <summary>
        /// Intended for use by the the viewer/user, the one who would be subscribed.
        /// Checks to see if a user is subscribed to a channel.        
        /// If the client's authentication token does not have the authorization permission to make the request, the method will return "false" even if the user is actually subscribed to the channel.                 
        /// Required scope: 'user_subscriptions'
        /// </summary>
        public bool isUserSubscribed(string user_id, string channel_id)
        {
            return isUserSubscribedAsync(user_id, channel_id).Result;
        }

        /// <summary>
        /// Makes the channel associated with the client's authentication token follow a user.
        /// Returns status '422' if the client could not follow the user.
        /// If the operation was sucessful, the <see cref="Models.Users.Follow"/> object is returned;. 
        /// Required scope: 'user_follows_edit' 
        /// </summary>
        public Follow Follow(string user_id, bool notifications = false)
        {
            return FollowAsync(user_id, notifications).Result;
        }

        /// <summary>
        /// Makes the channel associated with the client's authentication token follow a user.
        /// Returns status '204' if the client successfully unfollowed the user.
        /// Required scope: 'user_follows_edit' 
        /// </summary>
        public HttpStatusCode Unfollow(string user_id)
        {
            return UnfollowAsync(user_id).Result;
        }

        /// <summary>
        /// Gets a single paged list of blocked users for the channel associated with the client's authentication token.
        /// <see cref="PagingBlockedUsers"/> can be specified to request a custom paged result.
        /// Required scope: 'user_blocks_read'
        /// </summary>        
        public BlockedUsersPage GetBlockedUsersPage(PagingBlockedUsers paging = null)
        {
            return GetBlockedUsersPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of all blocked users for the channel associated with the client's authentication token in ascending order from newest to oldest.
        /// Required scope: 'user_blocks_read'
        /// </summary>
        public List<BlockedUser> GetBlockedUsers()
        {
            return GetBlockedUsersAsync().Result;
        }

        /// <summary>
        /// Blocks a user for the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="BlockedUser"/> object is returned;. 
        /// Required scope: 'user_blocks_edit'
        /// </summary>
        public BlockedUser Block(string user_id)
        {
            return BlockAsync(user_id).Result;
        }

        /// <summary>
        /// Blocks a user for the channel associated with the client's authentication token.
        /// Returns status '204' if the user was successfully unblocked.
        /// Returns status '404' if the user is not blocked by the client.
        /// Returns status '422' if the user could not be unblocked.
        /// Required scope: 'user_blocks_edit'
        /// </summary>
        public HttpStatusCode Unblock(string user_id)
        {
            return UnblockAsync(user_id).Result;
        }

        #endregion

        #region Videos

        /// <summary>
        /// Gets a single paged list of videos from the users the channel associated with the client's authentication token is following.
        /// <see cref="PagingUserFollowsVideos"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>
        public VideosFollowsPage GetUserFollowsVideosPage(PagingUserFollowsVideos paging = null)
        {   
            return GetUserFollowsVideosPageAsync(paging).Result;
        }

        /// <summary>
        /// Gets a complete list of all the archives from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public List<Video> GetUserFollowsArchives()
        {
            return GetUserFollowsArchivesAsync().Result;
        }

        /// <summary>
        /// Gets a complete list of all the highlights from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public List<Video> GetUserFollowsHighlights()
        {
            return GetUserFollowsHighlightsAsync().Result;
        }

        /// <summary>
        /// Gets a complete list of all the uploads from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public List<Video> GetUserFollowsUploads()
        {
            return GetUserFollowsUploadsAsync().Result;
        }

        /// <summary>
        /// Gets a complete list of all the videos (archives, uploads, and highlights) from the users the channel associated with the client's authentication token is following.
        /// </summary>
        public List<Video> GetUserFollowsVideos()
        {
            return GetUserFollowsVideosAsync().Result;
        }

        /// <summary>
        /// Gets a complete list of videos from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public List<Video> GetUserFollowsVideos(BroadcastType[] broadcast_type)
        {
            return GetUserFollowsVideosAsync(broadcast_type).Result;
        }

        #endregion        
    }
}