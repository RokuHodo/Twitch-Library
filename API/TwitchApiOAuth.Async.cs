using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

//project namespaces
using TwitchLibrary.Enums.API;
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Paging;
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
using RestSharp;

namespace TwitchLibrary.API
{
    public partial class TwitchApiOAuth : TwitchApi, ITwitchRequest
    {
        private string api_id;                              
                
        public TwitchApiOAuth(string oauth_token, string client_id = "") : base(client_id, oauth_token)
        {
            base.oauth_token = oauth_token;

            api_id = GetUser()._id;
        }

        #region Channels                    DONE

        /// <summary>
        /// Gets the <see cref="ChannelOAuth"/> object associated with the client's authentication token.
        /// Required scope: 'channel_read'
        /// </summary>
        public async Task<ChannelOAuth> GetChannelAsync()
        {
            RestRequest request = Request("channel", Method.GET);

            IRestResponse<ChannelOAuth> response = await client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Sets the title of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<ChannelOAuth> SetTitleAsync(string status)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { status } });

            IRestResponse<ChannelOAuth> response = await client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Sets the game of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<ChannelOAuth> SetGameAsync(string game)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { game } });

            IRestResponse<ChannelOAuth> response = await client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Sets the delay of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<ChannelOAuth> SetDelayAsync(int delay)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { delay } });

            IRestResponse<ChannelOAuth> response = await client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Enables or disables the channel feed of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<ChannelOAuth> EnableChannelFeedAsync(bool channel_feed_enabled)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { channel_feed_enabled } });

            IRestResponse<ChannelOAuth> response = await client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all editors for the channel associated with the client's authentication token.
        /// Required scope: 'channel_read'
        /// </summary>
        public async Task<Editors> GetChannelEditorsAsync()
        {
            RestRequest request = Request("channels/{channel_id}/editors", Method.GET);
            request.AddUrlSegment("channel_id", api_id);

            IRestResponse<Editors> response = await client.ExecuteTaskAsync<Editors>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a single paged list of subscribers for the channel associated with the client's authentication token.
        /// <see cref="PagingChannelFollowers"/> can be specified to request a custom paged result.
        /// Required scope: 'channel_subscriptions'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>
        public async Task<ChannelSubscribersPage> GetChannelSubscribersPageAsync(PagingChannelSubscribers paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelSubscribers();
            }

            RestRequest request = Request("channels/{channel_id}/subscriptions", Method.GET);
            request.AddUrlSegment("channel_id", api_id);
            request = paging.Add(request);

            IRestResponse<ChannelSubscribersPage> response = await client.ExecuteTaskAsync<ChannelSubscribersPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all subscribers for the channel associated with the client's authentication token in ascending order.
        /// Required scope: channel_subscriptions
        /// NOTE: Works, in theory, can't test because I'm not a partner.
        /// </summary>
        public async Task<List<ChannelSubscriber>> GetChannelSubscribersAsync(Direction direction = Direction.ASC)
        {

            PagingChannelSubscribers paging = new PagingChannelSubscribers();            
            paging.limit = 100;
            paging.direction = direction;

            List<ChannelSubscriber> subscribers = await Paging.GetPagesByTotalAsync<ChannelSubscriber, ChannelSubscribersPage, PagingChannelSubscribers>(GetChannelSubscribersPageAsync, paging, "subscriptions");

            return subscribers;
        }

        /// <summary>
        /// Gets the subscriber relationship the channel associated with the client's authentication token and a user.
        /// Returns status '404' if the user is not subscribed to the channel.
        /// Required scope: 'channel_check_subscription'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>
        public async Task<ChannelSubscriber> GetChannelSubscriberRelationshipAsync(string user_id)
        {
            RestRequest request = Request("channels/{channel_id}/subscriptions/{user_id}", Method.GET);
            request.AddUrlSegment("user_id", user_id);
            request.AddUrlSegment("channel_id", api_id);

            IRestResponse<ChannelSubscriber> response = await client.ExecuteTaskAsync<ChannelSubscriber>(request);

            return response.Data;
        }

        /// <summary>
        /// Intended for use by the the channel owner.
        /// Checks to see if a user is subsdcribed to the channel associated with the client's authentication token.        
        /// Required scope: 'channel_check_subscription'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>        
        public async Task<bool> isUserSubscribedAsync(string user_id)
        {
            ChannelSubscriber subscriber = await GetChannelSubscriberRelationshipAsync(user_id);

            return subscriber.http_status != 404;
        }

        /// <summary>
        /// Starts a commercial for the channel associated with the client's authentication token.        
        /// Returns status '422' if an invalid length is specified, an attempt is made to start a commercial less than 8 minutes after the previous commercial, or the specified channel is not a Twitch partner.
        /// If the operation was sucessful, the <see cref="CommercialResponse"/> object is returned. 
        /// Required scope: 'channel_commercial'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>
        public async Task<CommercialResponse> StartCommercialAsync(CommercialLength length)
        {
            string duration = length.ToString().TextAfter("_");

            RestRequest request = Request("channels/{channel_id}/commercial", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { duration } });

            IRestResponse<CommercialResponse> response = await client.ExecuteTaskAsync<CommercialResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Resets the stream key for the channel associated with the client's authentication token.
        /// Required scope: 'channel_stream'
        /// </summary>
        public async Task<ChannelOAuth> ResetStreamKeyAsync()
        {
            RestRequest request = Request("channels/{channel_id}/stream_key", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);

            IRestResponse<ChannelOAuth> response = await client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        #endregion

        #region Clips                       DONE

        /// <summary>
        /// Gets a single paged list of clips from the games the user associated with the client's authentication token is following highest to lowest view count.
        /// <see cref="PagingClips"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>
        public async Task<ClipsPage> GetUserGamesFollowedClipsPageAsync(PagingClipsGamesFollowed paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingClips();
            }

            RestRequest request = Request("clips/followed", Method.GET, ApiVersion.v4);
            request = paging.Add(request);

            IRestResponse<ClipsPage> response = await client.ExecuteTaskAsync<ClipsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of clips from the games the user associated with the client's authentication token is following highest to lowest view count.
        /// <see cref="PagingClips"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>
        public async Task<List<Clip>> GetUserGamesFollowedClipsAsync(PagingClipsGamesFollowed paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingClipsGamesFollowed();
                paging.limit = 100;                
            }

            List<Clip> clips = await Paging.GetPagesByCursorAsync<Clip, ClipsPage, PagingClipsGamesFollowed>(GetUserGamesFollowedClipsPageAsync, paging, "clips");

            return clips;
        }

        #endregion

        #region Feed                        DONE

        /// <summary>
        /// Gets a single paged list of posts in the feed of the channel associated with the client's authentication token.
        /// <see cref="PagingChannelFeedPosts"/> can be specified to request a custom paged result.
        /// Required scope: 'any'       
        /// </summary>
        public async Task<PostsPage> GetChannelFeedPostsPageAsync(PagingChannelFeedPosts paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelFeedPosts();
            }

            RestRequest request = Request("feed/{channel_id}/posts", Method.GET);
            request.AddUrlSegment("channel_id", api_id);
            request = paging.Add(request);

            IRestResponse<PostsPage> response = await client.ExecuteTaskAsync<PostsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of posts in the feed of the channel associated with the client's authentication token from newest to oldest.        
        /// Required scope: 'any'        
        /// </summary>
        public async Task<List<Post>> GetChannelFeedPostsAsync()
        {
            PagingChannelFeedPosts paging = new PagingChannelFeedPosts();
            paging.limit = 100;

            List<Post> posts = await Paging.GetPagesByCursorAsync<Post, PostsPage, PagingChannelFeedPosts>(GetChannelFeedPostsPageAsync, paging, "posts");

            return posts;
        }

        /// <summary>
        /// Gets a post in the feed of the channel associated with the client's authentication token.                
        /// Required scope: 'any'   
        /// </summary>
        public async Task<Post> GetChannelFeedPostAsync(string post_id, int comments = 5)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}", Method.GET);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request.AddParameter("comments", comments.Clamp(0, 5, 5));

            IRestResponse<Post> response = await client.ExecuteTaskAsync<Post>(request);

            return response.Data;
        }

        /// <summary>
        /// Creates a post in the feed of the channel associated with the client's authentication token.
        /// If share is set to true and the channel has a ocnnected twitter account, the post will be also tweeted.
        /// If the operation was sucessful, the <see cref="CreatedPost"/> object is returned. 
        /// Required scope: 'channel_feed_edit' 
        /// </summary>
        public async Task<CreatedPost> CreateChannelFeedPostAsync(string content, bool share = false)
        {
            RestRequest request = Request("feed/{channel_id}/posts", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddQueryParameter("share", share.ToString().ToLower());
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { content });

            IRestResponse<CreatedPost> response = await client.ExecuteTaskAsync<CreatedPost>(request);

            return response.Data;
        }

        /// <summary>
        /// Deletes a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Post"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<Post> DeleteChannelFeedPostAsync(string post_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());

            IRestResponse<Post> response = await client.ExecuteTaskAsync<Post>(request);

            return response.Data;
        }

        /// <summary>
        /// Creates a reaction to a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="CreateReactionResponse"/> object is returned;. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<CreateReactionResponse> CreateChannelFeedPostReactionAsync(string post_id, string emote_id = "endorse")
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/reactions", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);            
            request.AddQueryParameter("emote_id", emote_id);

            IRestResponse<CreateReactionResponse> response = await client.ExecuteTaskAsync<CreateReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Deletes a reaction to a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="DeleteReactionResponse"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<DeleteReactionResponse> DeleteChannelFeedPostReactionAsync(string post_id, string emote_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/reactions", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);
            request.AddQueryParameter("emote_id", emote_id);

            IRestResponse<DeleteReactionResponse> response = await client.ExecuteTaskAsync<DeleteReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a single paged list of comments to a post in the feed of the channel associated with the client's authentication token.
        /// <see cref="PagingFeedPostComments"/> can be specified to request a custom paged result.
        /// Required scope: 'any'
        /// </summary>
        public async Task<PostCommentsPage> GetChannelFeedPostCommentsPageAsync(string post_id, PagingChannelFeedPostComments paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelFeedPostComments();
            }

            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments", Method.GET);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request = paging.Add(request);

            IRestResponse<PostCommentsPage> response = await client.ExecuteTaskAsync<PostCommentsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of comments in a post in the feed of the channel associated with the client's authentication token.        
        /// Required scope: 'any'
        /// </summary>        
        public async Task<List<Comment>> GetChannelFeedPostCommentsAsync(string post_id)
        {   
            PagingChannelFeedPostComments paging = new PagingChannelFeedPostComments();
            paging.limit = 100;

            List<Comment> comments = await Paging.GetPagesByCursorAsync<Comment, PostCommentsPage, PagingChannelFeedPostComments>(GetChannelFeedPostCommentsPageAsync, post_id, paging, "comments");

            return comments;
        }

        /// <summary>
        /// Creates a comment on a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Comment"/> object is returned. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<Comment> CreateChannelFeedPostCommentAsync(string post_id, string content)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { content });

            IRestResponse<Comment> response = await client.ExecuteTaskAsync<Comment>(request);

            return response.Data;
        }

        /// <summary>
        /// Deletes a comment on a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Comment"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<Comment> DeleteChannelFeedPostCommentAsync(string post_id, string comment_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments/{comment_id}", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request.AddUrlSegment("comment_id", comment_id.ToString());

            IRestResponse<Comment> response = await client.ExecuteTaskAsync<Comment>(request);

            return response.Data;
        }

        /// <summary>
        /// Creates a reaction to a comment in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="CreateReactionResponse"/> object is returned. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<CreateReactionResponse> CreateChannelFeedPostCommentReactionAsync(string post_id, string comment_id, string emote_id = "endorse")
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments/{comment_id}/reactions", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);
            request.AddUrlSegment("comment_id", comment_id);
            request.AddQueryParameter("emote_id", emote_id);

            IRestResponse<CreateReactionResponse> response = await client.ExecuteTaskAsync<CreateReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Deletes a reaction to a comment in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="DeleteReactionResponse"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<DeleteReactionResponse> DeleteChannelFeedPostCommentReactionAsync(string post_id, string comment_id, string emote_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments/{comment_id}/reactions", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);
            request.AddUrlSegment("comment_id", comment_id);
            request.AddQueryParameter("emote_id", emote_id);

            IRestResponse<DeleteReactionResponse> response = await client.ExecuteTaskAsync<DeleteReactionResponse>(request);

            return response.Data;
        }

        #endregion

        #region Streams                     DONE

        /// <summary>
        /// Gets a single paged list of streams that the channel associated with the client's authentication token is following.
        /// <see cref="PagingStreamFollows"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>        
        public async Task<StreamsPage> GetStreamFollowsPageAsync(PagingStreamFollows paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingStreamFollows();
            }

            RestRequest request = Request("streams/followed", Method.GET);
            request = paging.Add(request);

            IRestResponse<StreamsPage> response = await client.ExecuteTaskAsync<StreamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of streams that the channel associated with the client's authentication token is following.        
        /// Required scope: 'user_read'
        /// </summary>        
        public async Task<List<Stream>> GetStreamFollowsAsync(StreamType stream_type = StreamType.LIVE)
        {
            PagingStreamFollows paging = new PagingStreamFollows();
            paging.limit = 100;
            paging.stream_type = stream_type;

            List<Stream> following = await Paging.GetPagesByTotalAsync<Stream, StreamsPage, PagingStreamFollows>(GetStreamFollowsPageAsync, paging, "streams");

            return following;
        }

        #endregion

        #region Users                       DONE

        /// <summary>
        /// Gets the <see cref="UserOAuth"/> object associated with the client's authentication token.
        /// Required scope: 'user_read'
        /// </summary>
        public async Task<UserOAuth> GetUserAsync()
        {
            RestRequest request = Request("user", Method.GET);

            IRestResponse<UserOAuth> response = await client.ExecuteTaskAsync<UserOAuth>(request);

            return response.Data;            
        }

        /// <summary>
        /// Gets all emote sets and sub emotes that the user associated with the client's authentication token can use in chat.
        /// Required scope: 'user_subscriptions'
        /// </summary>
        public async Task<EmoteSet> GetUserEmotesAsync()
        {
            RestRequest request = Request("users/{user_id}/emotes", Method.GET);
            request.AddUrlSegment("user_id", api_id);

            IRestResponse<EmoteSet> response = await client.ExecuteTaskAsync<EmoteSet>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets the subscriber relationship between a user and the channel associated with the client's authentication token.        
        /// Returns status '403' if the oauth token doesn't have proper authorization to make the request.
        /// Returns status '404' if the user is not subscribed to the channel.
        /// Returns status '422' if the channel does not have a subscription program.
        /// Required scope: 'user_subscriptions'    
        /// </summary>
        public async Task<UserSubscriber> GetUserSubscriberRelationshipAsync(string user_id, string channel_id)
        {
            RestRequest request = Request("users/{user_id}/subscriptions/{channel_id}", Method.GET);
            request.AddUrlSegment("user_id", user_id);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<UserSubscriber> response = await client.ExecuteTaskAsync<UserSubscriber>(request);

            return response.Data;
        }

        /// <summary>
        /// Intended for use by the the viewer/user, the one who would be subscribed.
        /// Checks to see if a user is subscribed to a channel.        
        /// If the client's authentication token does not have the authorization permission to make the request, the method will return "false" even if the user is actually subscribed to the channel.                 
        /// Required scope: 'user_subscriptions'
        /// </summary>
        public async Task<bool> isUserSubscribedAsync(string user_id, string channel_id)
        {
            UserSubscriber relationship = await GetUserSubscriberRelationshipAsync(user_id, channel_id);
            
            return relationship.http_status != 404 && relationship.http_status != 422 && relationship.http_status != 403;
        }

        /// <summary>
        /// Makes the channel associated with the client's authentication token follow a user.
        /// Returns status '422' if the client could not follow the user.
        /// If the operation was sucessful, the <see cref="Models.Users.Follow"/> object is returned;. 
        /// Required scope: 'user_follows_edit' 
        /// </summary>
        public async Task<Follow> FollowAsync(string user_id, bool notifications = false)
        {
            RestRequest request = Request("users/{user_id}/follows/channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("user_id", api_id);
            request.AddUrlSegment("channel_id", user_id);            
            request.AddParameter("notifications", notifications);

            IRestResponse<Follow> response = await client.ExecuteTaskAsync<Follow>(request);

            return response.Data;
        }

        /// <summary>
        /// Makes the channel associated with the client's authentication token follow a user.
        /// Returns status '204' if the client successfully unfollowed the user.
        /// Required scope: 'user_follows_edit' 
        /// </summary>
        public async Task<HttpStatusCode> UnfollowAsync(string user_id)
        {
            RestRequest request = Request("users/{user_id}/follows/channels/{channel_id}", Method.DELETE);
            request.AddUrlSegment("user_id", api_id);
            request.AddUrlSegment("channel_id", user_id);

            IRestResponse<object> response = await client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Gets a single paged list of blocked users for the channel associated with the client's authentication token.
        /// <see cref="PagingBlockedUsers"/> can be specified to request a custom paged result.
        /// Required scope: 'user_blocks_read'
        /// </summary>        
        public async Task<BlockedUsersPage> GetBlockedUsersPageAsync(PagingBlockedUsers paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingBlockedUsers();
            }

            RestRequest request = Request("users/{user_id}/blocks", Method.GET);
            request.AddUrlSegment("user_id", api_id);
            request = paging.Add(request);

            IRestResponse<BlockedUsersPage> response = await client.ExecuteTaskAsync<BlockedUsersPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all blocked users for the channel associated with the client's authentication token in ascending order from newest to oldest.
        /// Required scope: 'user_blocks_read'
        /// </summary>
        public async Task<List<BlockedUser>> GetBlockedUsersAsync()
        {
            PagingBlockedUsers paging = new PagingBlockedUsers();
            paging.limit = 100;

            List<BlockedUser> blocks = await Paging.GetPagesByTotalAsync<BlockedUser, BlockedUsersPage, PagingBlockedUsers>(GetBlockedUsersPageAsync, paging, "blocks");

            return blocks;
        }

        /// <summary>
        /// Blocks a user for the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="BlockedUser"/> object is returned;. 
        /// Required scope: 'user_blocks_edit'
        /// </summary>
        public async Task<BlockedUser> BlockAsync(string user_id)
        {
            RestRequest request = Request("users/{user_id}/blocks/{channel_id}", Method.PUT);
            request.AddUrlSegment("user_id", api_id);
            request.AddUrlSegment("channel_id", user_id);

            IRestResponse<BlockedUser> response = await client.ExecuteTaskAsync<BlockedUser>(request);

            return response.Data;
        }

        /// <summary>
        /// Blocks a user for the channel associated with the client's authentication token.
        /// Returns status '204' if the user was successfully unblocked.
        /// Returns status '404' if the user is not blocked by the client.
        /// Returns status '422' if the user could not be unblocked.
        /// Required scope: 'user_blocks_edit'
        /// </summary>
        public async Task<HttpStatusCode> UnblockAsync(string user_id)
        {
            RestRequest request = Request("users/{user_id}/blocks/{channel_id}", Method.DELETE);
            request.AddUrlSegment("user_id", api_id);
            request.AddUrlSegment("channel_id", user_id);

            IRestResponse<object> response = await client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        #endregion

        #region Videos                      DONE

        /// <summary>
        /// Gets a single paged list of videos from the users the channel associated with the client's authentication token is following.
        /// <see cref="PagingUserFollowsVideos"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>
        public async Task<VideosFollowsPage> GetUserFollowsVideosPageAsync(PagingUserFollowsVideos paging = null)
        {   
            if (paging.isNull())
            {
                paging = new PagingUserFollowsVideos();              
            }
            
            RestRequest request = Request("videos/followed", Method.GET);
            request = paging.Add(request);

            IRestResponse<VideosFollowsPage> response = await client.ExecuteTaskAsync<VideosFollowsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all the archives from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public async Task<List<Video>> GetUserFollowsArchivesAsync()
        {
            List<Video> archives = await GetUserFollowsVideosAsync(new BroadcastType[] { BroadcastType.ARCHIVE });

            return archives;
        }

        /// <summary>
        /// Gets a complete list of all the highlights from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public async Task<List<Video>> GetUserFollowsHighlightsAsync()
        {
            List<Video> highlights = await GetUserFollowsVideosAsync(new BroadcastType[] { BroadcastType.HIGHLIGHT });

            return highlights;
        }

        /// <summary>
        /// Gets a complete list of all the uploads from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public async Task<List<Video>> GetUserFollowsUploadsAsync()
        {
            List<Video> uploads = await GetUserFollowsVideosAsync(new BroadcastType[] { BroadcastType.UPLOAD });

            return uploads;
        }

        /// <summary>
        /// Gets a complete list of all the videos (archives, uploads, and highlights) from the users the channel associated with the client's authentication token is following.
        /// </summary>
        public async Task<List<Video>> GetUserFollowsVideosAsync()
        {
            List<Video> videos = await GetUserFollowsVideosAsync(new BroadcastType[] { BroadcastType.ARCHIVE, BroadcastType.HIGHLIGHT, BroadcastType.UPLOAD });

            return videos;
        }

        /// <summary>
        /// Gets a complete list of videos from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public async Task<List<Video>> GetUserFollowsVideosAsync(BroadcastType[] broadcast_type)
        {
            PagingUserFollowsVideos paging = new PagingUserFollowsVideos();
            //paging.limit = 50;        for some reaosn, setting this to anything but the default breaks things
            paging.broadcast_type = broadcast_type;

            List<Video> follows_videos = await Paging.GetPagesByOffsetAsync<Video, VideosFollowsPage, PagingUserFollowsVideos>(GetUserFollowsVideosPageAsync, paging, "videos");

            return follows_videos;
        }


        #endregion

        #region Rest Request

        /// <summary>
        /// Send the request to the api.
        /// Requires the authentication token of the broadcaster and the client id of the application from Twitch.
        /// </summary>
        public new RestRequest Request(string endpoint, Method method, ApiVersion api_version = ApiVersion.v5)
        {
            RestRequest request = new RestRequest(endpoint, method);

            if (client_id.isValidString())
            {
                request.AddHeader("Client-ID", client_id);
            }
            
            request.AddHeader("Authorization", "OAuth " + oauth_token);
            request.AddQueryParameter("api_version", api_version.ToString().TextAfter("v"));                    

            return request;
        }

        #endregion
    }
}