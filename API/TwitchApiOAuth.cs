using System;
using System.Collections.Generic;
using System.Net;

//project namespaces
using TwitchLibrary.Enums.API;
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
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
    public class TwitchApiOAuth : TwitchApi, ITwitchRequest
    {
        private string api_id;                              
                
        public TwitchApiOAuth(string oauth_token, string client_id = "") : base(client_id, oauth_token)
        {
            base.oauth_token = oauth_token;

            api_id = GetUser()._id;
        }

        #region Channels

        /// <summary>
        /// Gets the <see cref="ChannelOAuth"/> object associated with the client's authentication token.
        /// Required scope: 'channel_read'
        /// </summary>
        public ChannelOAuth GetChannel()
        {
            RestRequest request = Request("channel", Method.GET);

            IRestResponse<ChannelOAuth> response = client.Execute<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Sets the title of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public ChannelOAuth SetTitle(string status)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { status } });

            IRestResponse<ChannelOAuth> response = client.Execute<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Sets the game of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public ChannelOAuth SetGame(string game)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { game } });

            IRestResponse<ChannelOAuth> response = client.Execute<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Sets the delay of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public ChannelOAuth SetDelay(int delay)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { delay } });

            IRestResponse<ChannelOAuth> response = client.Execute<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Enables or disables the channel feed of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public ChannelOAuth EnableChannelFeed(bool channel_feed_enabled)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { channel_feed_enabled } });

            IRestResponse<ChannelOAuth> response = client.Execute<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all editors for the channel associated with the client's authentication token.
        /// Required scope: 'channel_read'
        /// </summary>
        public Editors GetChannelEditors()
        {
            RestRequest request = Request("channels/{channel_id}/editors", Method.GET);
            request.AddUrlSegment("channel_id", api_id);

            IRestResponse<Editors> response = client.Execute<Editors>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a single paged list of subscribers for the channel associated with the client's authentication token.
        /// <see cref="PagingChannelFollowers"/> can be specified to request a custom paged result.
        /// Required scope: 'channel_subscriptions'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>
        public ChannelSubscribersPage GetChannelSubscribersPage(PagingChannelSubscribers paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelSubscribers();
            }

            RestRequest request = Request("channels/{channel_id}/subscriptions", Method.GET);
            request.AddUrlSegment("channel_id", api_id);
            request = paging.Add(request);

            IRestResponse<ChannelSubscribersPage> response = client.Execute<ChannelSubscribersPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all subscribers for the channel associated with the client's authentication token in ascending order.
        /// Required scope: channel_subscriptions
        /// NOTE: Works, in theory, can't test because I'm not a partner.
        /// </summary>
        public List<ChannelSubscriber> GetChannelSubscribers(Direction direction = Direction.ASC)
        {
            List<ChannelSubscriber> subscribers = new List<ChannelSubscriber>();

            PagingChannelSubscribers paging = new PagingChannelSubscribers();            
            paging.limit = 100;
            paging.direction = direction;

            ChannelSubscribersPage subscribers_page = GetChannelSubscribersPage(paging);

            if (!isPartner(api_id) || subscribers_page._total == 0)
            {
                return subscribers;
            }

            decimal pages_dec = subscribers_page._total / paging.limit;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));

            for (int page = 0; page < pages + 1; ++page)
            {
                //don't request the first page again because we already have it
                if (page != 0)
                {
                    paging.offset = paging.limit * page;
                    subscribers_page = GetChannelSubscribersPage(paging);
                }

                foreach (ChannelSubscriber follow in subscribers_page.subscriptions)
                {
                    subscribers.Add(follow);
                }
            }

            return subscribers;
        }

        /// <summary>
        /// Gets the subscriber relationship the channel associated with the client's authentication token and a user.
        /// Returns status '404' if the user is not subscribed to the channel.
        /// Required scope: 'channel_check_subscription'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>
        private ChannelSubscriber GetChannelSubscriberRelationship(string user_id)
        {
            RestRequest request = Request("channels/{channel_id}/subscriptions/{user_id}", Method.GET);
            request.AddUrlSegment("user_id", user_id);
            request.AddUrlSegment("channel_id", api_id);

            IRestResponse<ChannelSubscriber> response = client.Execute<ChannelSubscriber>(request);

            return response.Data;
        }

        /// <summary>
        /// Intended for use by the the channel owner.
        /// Checks to see if a user is subsdcribed to the channel associated with the client's authentication token.        
        /// Required scope: 'channel_check_subscription'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>        
        public bool isUserSubscribed(string user_id)
        {
            return GetChannelSubscriberRelationship(user_id).http_status != 404;
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
            string duration = length.ToString().TextAfter("_");

            RestRequest request = Request("channels/{channel_id}/commercial", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { duration } });

            IRestResponse<CommercialResponse> response = client.Execute<CommercialResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Resets the stream key for the channel associated with the client's authentication token.
        /// Required scope: 'channel_stream'
        /// </summary>
        public ChannelOAuth ResetStreamKey()
        {
            RestRequest request = Request("channels/{channel_id}/stream_key", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);

            IRestResponse<ChannelOAuth> response = client.Execute<ChannelOAuth>(request);

            return response.Data;
        }

        #endregion

        #region Clips

        /// <summary>
        /// Gets a single paged list of clips from the games the user associated with the client's authentication token is following highest to lowest view count.
        /// <see cref="PagingClips"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>
        public ClipsPage GetUserGamesFollowedClipsPage(PagingClipsGamesFollowed paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingClips();
            }

            RestRequest request = Request("clips/followed", Method.GET, ApiVersion.v4);
            request = paging.Add(request);

            IRestResponse<ClipsPage> response = client.Execute<ClipsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of clips from the games the user associated with the client's authentication token is following highest to lowest view count.
        /// <see cref="PagingClips"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>
        public List<Clip> GetUserGamesFollowedClips(PagingClipsGamesFollowed paging = null)
        {
            List<Clip> clips = new List<Clip>();

            if (paging.isNull())
            {
                paging = new PagingClipsGamesFollowed();
                paging.limit = 100;                
            }

            ClipsPage clips_page = GetUserGamesFollowedClipsPage(paging);

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

                    clips_page = GetUserGamesFollowedClipsPage(paging);
                }
                else
                {
                    searching = false;
                }
            }
            while (searching);

            return clips;
        }

        #endregion

        #region Feed

        /// <summary>
        /// Gets a single paged list of posts in the feed of the channel associated with the client's authentication token.
        /// <see cref="PagingChannelFeedPosts"/> can be specified to request a custom paged result.
        /// Required scope: 'any'       
        /// </summary>
        public PostsPage GetChannelFeedPostsPage(PagingChannelFeedPosts paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelFeedPosts();
            }

            RestRequest request = Request("feed/{channel_id}/posts", Method.GET);
            request.AddUrlSegment("channel_id", api_id);
            request = paging.Add(request);

            IRestResponse<PostsPage> response = client.Execute<PostsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of posts in the feed of the channel associated with the client's authentication token from newest to oldest.        
        /// Required scope: 'any'        
        /// </summary>
        public List<Post> GetChannelFeedPosts()
        {
            List<Post> posts = new List<Post>();

            PagingChannelFeedPosts paging = new PagingChannelFeedPosts();
            paging.limit = 100;

            PostsPage posts_page = GetChannelFeedPostsPage(paging);

            if (posts_page.isNull() && !posts_page._cursor.isValidString())
            {
                return posts;
            }

            bool searching = true;

            do
            {
                foreach (Post post in posts_page.posts)
                {
                    posts.Add(post);
                }

                if (posts_page._cursor.isValidString())
                {
                    paging.cursor = posts_page._cursor;

                    posts_page = GetChannelFeedPostsPage(paging);
                }
                else
                {
                    searching = false;
                }
            }
            while (searching);

            return posts;
        }

        /// <summary>
        /// Gets a post in the feed of the channel associated with the client's authentication token.                
        /// Required scope: 'any'   
        /// </summary>
        public Post GetChannelFeedPost(string post_id, long comments = 5)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}", Method.GET);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request.AddParameter("comments", comments.Clamp(0, 5, 5));

            IRestResponse<Post> response = client.Execute<Post>(request);

            return response.Data;
        }

        /// <summary>
        /// Creates a post in the feed of the channel associated with the client's authentication token.
        /// If share is set to true and the channel has a ocnnected twitter account, the post will be also tweeted.
        /// If the operation was sucessful, the <see cref="CreatedPost"/> object is returned. 
        /// Required scope: 'channel_feed_edit' 
        /// </summary>
        public CreatedPost CreateChannelFeedPost(string content, bool share = false)
        {
            RestRequest request = Request("feed/{channel_id}/posts", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddQueryParameter("share", share.ToString().ToLower());
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { content });

            IRestResponse<CreatedPost> response = client.Execute<CreatedPost>(request);

            return response.Data;
        }

        /// <summary>
        /// Deletes a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Post"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public Post DeleteChannelFeedPost(string post_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());

            IRestResponse<Post> response = client.Execute<Post>(request);

            return response.Data;
        }

        /// <summary>
        /// Creates a reaction to a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="CreateReactionResponse"/> object is returned;. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public CreateReactionResponse CreateChannelFeedPostReaction(string post_id, string emote_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/reactions", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);            
            request.AddQueryParameter("emote_id", emote_id);

            IRestResponse<CreateReactionResponse> response = client.Execute<CreateReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Reacts to a post in the feed of the channel associated with the client's authentication token with the default reaction, "endorse".
        /// If the operation was sucessful, the <see cref="CreateReactionResponse"/> object is returned;. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public CreateReactionResponse CreateChannelFeedPostReaction(string post_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/reactions", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request.AddQueryParameter("emote_id", "endorse");

            IRestResponse<CreateReactionResponse> response = client.Execute<CreateReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Deletes a reaction to a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="DeleteReactionResponse"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public DeleteReactionResponse DeleteChannelFeedPostReaction(string post_id, string emote_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/reactions", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);
            request.AddQueryParameter("emote_id", emote_id);

            IRestResponse<DeleteReactionResponse> response = client.Execute<DeleteReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a single paged list of comments to a post in the feed of the channel associated with the client's authentication token.
        /// <see cref="PagingFeedPostComments"/> can be specified to request a custom paged result.
        /// Required scope: 'any'
        /// </summary>
        public PostCommentsPage GetChannelFeedPostCommentsPage(string post_id, PagingChannelFeedPostComments paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingChannelFeedPostComments();
            }

            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments", Method.GET);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request = paging.Add(request);

            IRestResponse<PostCommentsPage> response = client.Execute<PostCommentsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of comments in a post in the feed of the channel associated with the client's authentication token.        
        /// Required scope: 'any'
        /// </summary>        
        public List<Comment> GetChannelFeedPostComments(string post_id)
        {   
            List<Comment> comments = new List<Comment>();

            PagingChannelFeedPostComments paging = new PagingChannelFeedPostComments();
            paging.limit = 100;

            PostCommentsPage post_comment_page = GetChannelFeedPostCommentsPage(post_id, paging);

            if(post_comment_page._total == 0)
            {
                return comments;
            }

            bool searching = true;

            do
            {
                foreach (Comment post in post_comment_page.comments)
                {
                    comments.Add(post);
                }

                if (post_comment_page._cursor.isValidString())
                {
                    paging.cursor = post_comment_page._cursor;

                    post_comment_page = GetChannelFeedPostCommentsPage(post_id, paging);
                }
                else
                {
                    searching = false;
                }
            }
            while (searching);

            return comments;
        }

        /// <summary>
        /// Creates a comment on a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Comment"/> object is returned. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public Comment CreateChannelFeedPostComment(string post_id, string content)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { content });

            IRestResponse<Comment> response = client.Execute<Comment>(request);

            return response.Data;
        }

        /// <summary>
        /// Deletes a comment on a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Comment"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public Comment DeleteChannelFeedPostComment(string post_id, string comment_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments/{comment_id}", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request.AddUrlSegment("comment_id", comment_id.ToString());

            IRestResponse<Comment> response = client.Execute<Comment>(request);

            return response.Data;
        }

        /// <summary>
        /// Creates a reaction to a comment in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="CreateReactionResponse"/> object is returned. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public CreateReactionResponse CreateChannelFeedPostCommentReaction(string post_id, string comment_id, string emote_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments/{comment_id}/reactions", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);
            request.AddUrlSegment("comment_id", comment_id);
            request.AddQueryParameter("emote_id", emote_id);

            IRestResponse<CreateReactionResponse> response = client.Execute<CreateReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Reacts to a comment in the feed of the channel associated with the client's authentication token with the default reaction, "endorse".
        /// If the operation was sucessful, the <see cref="CreateReactionResponse"/> object is returned. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public CreateReactionResponse CreateChannelFeedPostCommentReaction(string post_id, string comment_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments/{comment_id}/reactions", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);
            request.AddUrlSegment("comment_id", comment_id);
            request.AddQueryParameter("emote_id", "endorse");

            IRestResponse<CreateReactionResponse> response = client.Execute<CreateReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Deletes a reaction to a comment in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="DeleteReactionResponse"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public DeleteReactionResponse DeleteChannelFeedPostCommentReaction(string post_id, string comment_id, string emote_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments/{comment_id}/reactions", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);
            request.AddUrlSegment("comment_id", comment_id);
            request.AddQueryParameter("emote_id", emote_id);

            IRestResponse<DeleteReactionResponse> response = client.Execute<DeleteReactionResponse>(request);

            return response.Data;
        }

        #endregion

        #region Streams

        /// <summary>
        /// Gets a single paged list of streams that the channel associated with the client's authentication token is following.
        /// <see cref="PagingStreamFollows"/> can be specified to request a custom paged result.
        /// Required scope: 'user_read'
        /// </summary>        
        public StreamsPage GetStreamFollowsPage(PagingStreamFollows paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingStreamFollows();
            }

            RestRequest request = Request("streams/followed", Method.GET);
            request = paging.Add(request);

            IRestResponse<StreamsPage> response = client.Execute<StreamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of streams that the channel associated with the client's authentication token is following.        
        /// Required scope: 'user_read'
        /// </summary>        
        public List<Stream> GetStreamFollows(StreamType stream_type = StreamType.LIVE)
        {
            List<Stream> following = new List<Stream>();

            PagingStreamFollows paging = new PagingStreamFollows();
            paging.limit = 100;
            paging.stream_type = stream_type;

            StreamsPage following_page = GetStreamFollowsPage(paging);

            if (following_page._total == 0)
            {
                return following;
            }

            decimal pages_dec = following_page._total / paging.limit;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));

            for (int page = 0; page < pages + 1; ++page)
            {
                //don't request the first page again because we already have it
                if (page != 0)
                {
                    paging.offset = paging.limit * page;
                    following_page = GetStreamFollowsPage(paging);
                }

                foreach (Stream stream in following_page.streams)
                {
                    following.Add(stream);
                }
            }

            return following;
        }

        #endregion

        #region Users

        /// <summary>
        /// Gets the <see cref="UserOAuth"/> object associated with the client's authentication token.
        /// Required scope: 'user_read'
        /// </summary>
        public UserOAuth GetUser()
        {
            RestRequest request = Request("user", Method.GET);

            IRestResponse<UserOAuth> response = client.Execute<UserOAuth>(request);

            return response.Data;            
        }

        /// <summary>
        /// Gets all emote sets and sub emotes that the user associated with the client's authentication token can use in chat.
        /// Required scope: 'user_subscriptions'
        /// </summary>
        public EmoteSet GetUserEmotes()
        {
            RestRequest request = Request("users/{user_id}/emotes", Method.GET);
            request.AddUrlSegment("user_id", api_id);

            IRestResponse<EmoteSet> response = client.Execute<EmoteSet>(request);

            return response.Data;
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
            RestRequest request = Request("users/{user_id}/subscriptions/{channel_id}", Method.GET);
            request.AddUrlSegment("user_id", user_id);
            request.AddUrlSegment("channel_id", channel_id);

            IRestResponse<UserSubscriber> response = client.Execute<UserSubscriber>(request);

            return response.Data;
        }

        /// <summary>
        /// Intended for use by the the viewer/user, the one who would be subscribed.
        /// Checks to see if a user is subscribed to a channel.        
        /// If the client's authentication token does not have the authorization permission to make the request, the method will return "false" even if the user is actually subscribed to the channel.                 
        /// Required scope: 'user_subscriptions'
        /// </summary>
        public bool isUserSubscribed(string user_id, string channel_id)
        {
            UserSubscriber relationship = GetUserSubscriberRelationship(user_id, channel_id);
            
            return relationship.http_status != 404 && relationship.http_status != 422 && relationship.http_status != 403;
        }

        /// <summary>
        /// Makes the channel associated with the client's authentication token follow a user.
        /// Returns status '422' if the client could not follow the user.
        /// If the operation was sucessful, the <see cref="Models.Users.Follow"/> object is returned;. 
        /// Required scope: 'user_follows_edit' 
        /// </summary>
        public Follow Follow(string user_id, bool notifications = false)
        {
            RestRequest request = Request("users/{user_id}/follows/channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("user_id", api_id);
            request.AddUrlSegment("channel_id", user_id);            
            request.AddParameter("notifications", notifications);

            IRestResponse<Follow> response = client.Execute<Follow>(request);

            return response.Data;
        }

        /// <summary>
        /// Makes the channel associated with the client's authentication token follow a user.
        /// Returns status '204' if the client successfully unfollowed the user.
        /// Required scope: 'user_follows_edit' 
        /// </summary>
        public HttpStatusCode Unfollow(string user_id)
        {
            RestRequest request = Request("users/{user_id}/follows/channels/{channel_id}", Method.DELETE);
            request.AddUrlSegment("user_id", api_id);
            request.AddUrlSegment("channel_id", user_id);

            IRestResponse<object> response = client.Execute<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Gets a single paged list of blocked users for the channel associated with the client's authentication token.
        /// <see cref="PagingBlockedUsers"/> can be specified to request a custom paged result.
        /// Required scope: 'user_blocks_read'
        /// </summary>        
        public BlockedUsersPage GetBlockedUsersPage(PagingBlockedUsers paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingBlockedUsers();
            }

            RestRequest request = Request("users/{user_id}/blocks", Method.GET);
            request.AddUrlSegment("user_id", api_id);
            request = paging.Add(request);

            IRestResponse<BlockedUsersPage> response = client.Execute<BlockedUsersPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all blocked users for the channel associated with the client's authentication token in ascending order from newest to oldest.
        /// Required scope: 'user_blocks_read'
        /// </summary>
        public List<BlockedUser> GetBlockedUsers()
        {
            List<BlockedUser> blocks = new List<BlockedUser>();

            PagingBlockedUsers paging = new PagingBlockedUsers();
            paging.limit = 100;

            BlockedUsersPage blocks_page = GetBlockedUsersPage(paging);

            if (blocks_page._total == 0)
            {
                return blocks;
            }

            decimal pages_dec = blocks_page._total / paging.limit;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));

            for (int page = 0; page < pages + 1; ++page)
            {
                //don't request the first page again because we already have it
                if (page != 0)
                {
                    paging.offset = paging.limit * page;
                    blocks_page = GetBlockedUsersPage(paging);
                }

                foreach (BlockedUser block in blocks_page.blocks)
                {
                    blocks.Add(block);
                }
            }

            return blocks;
        }

        /// <summary>
        /// Blocks a user for the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="BlockedUser"/> object is returned;. 
        /// Required scope: 'user_blocks_edit'
        /// </summary>
        public BlockedUser Block(string user_id)
        {
            RestRequest request = Request("users/{user_id}/blocks/{channel_id}", Method.PUT);
            request.AddUrlSegment("user_id", api_id);
            request.AddUrlSegment("channel_id", user_id);

            IRestResponse<BlockedUser> response = client.Execute<BlockedUser>(request);

            return response.Data;
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
            RestRequest request = Request("users/{user_id}/blocks/{channel_id}", Method.DELETE);
            request.AddUrlSegment("user_id", api_id);
            request.AddUrlSegment("channel_id", user_id);

            IRestResponse<object> response = client.Execute<object>(request);

            return response.StatusCode;
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
            if (paging.isNull())
            {
                paging = new PagingUserFollowsVideos();              
            }
            
            RestRequest request = Request("videos/followed", Method.GET);
            request = paging.Add(request);

            IRestResponse<VideosFollowsPage> response = client.Execute<VideosFollowsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Gets a complete list of all the archives from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public List<Video> GetUserFollowsArchives()
        {
            return GetUserFollowsVideos(new BroadcastType[] { BroadcastType.ARCHIVE });
        }

        /// <summary>
        /// Gets a complete list of all the highlights from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public List<Video> GetUserFollowsHighlights()
        {
            return GetUserFollowsVideos(new BroadcastType[] { BroadcastType.HIGHLIGHT });
        }

        /// <summary>
        /// Gets a complete list of all the uploads from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public List<Video> GetUserFollowsUploads()
        {
            return GetUserFollowsVideos(new BroadcastType[] { BroadcastType.UPLOAD });
        }

        /// <summary>
        /// Gets a complete list of all the videos (archives, uploads, and highlights) from the users the channel associated with the client's authentication token is following.
        /// </summary>
        public List<Video> GetUserFollowsVideos()
        {
            return GetUserFollowsVideos(new BroadcastType[] { BroadcastType.ARCHIVE, BroadcastType.HIGHLIGHT, BroadcastType.UPLOAD });
        }

        /// <summary>
        /// Gets a complete list of videos from the users the channel associated with the client's authentication token is following.
        /// </summary>        
        public List<Video> GetUserFollowsVideos(BroadcastType[] broadcast_type)
        {
            List<Video> follows_videos = new List<Video>();

            PagingUserFollowsVideos paging = new PagingUserFollowsVideos();
            //paging.limit = 50;        for some reaosn, setting this to anything but the default breaks things
            paging.broadcast_type = broadcast_type;

            VideosFollowsPage follows_videos_page = GetUserFollowsVideosPage(paging);

            if (!follows_videos_page.videos.isValidList())
            {
                return follows_videos;
            }

            int page = 0;

            //Twitch droppped the ball and didn't include _total or _cursor, just loop until we find nothing
            do
            {
                foreach (Video video in follows_videos_page.videos)
                {
                    follows_videos.Add(video);
                }

                ++page;
                paging.offset = paging.limit * page;

                follows_videos_page = GetUserFollowsVideosPage(paging);
            }
            while (follows_videos_page.videos.isValidList());

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