using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

//project namespaces
using TwitchLibrary.Enums.API;
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Extensions;
using TwitchLibrary.Helpers.Paging;
using TwitchLibrary.Helpers.Paging.Channels;
using TwitchLibrary.Helpers.Paging.Clips;
using TwitchLibrary.Helpers.Paging.Communities;
using TwitchLibrary.Helpers.Paging.Feed;
using TwitchLibrary.Helpers.Paging.Streams;
using TwitchLibrary.Helpers.Paging.Users;
using TwitchLibrary.Helpers.Paging.Videos;
using TwitchLibrary.Interfaces.API;
using TwitchLibrary.Models.API.Channels;
using TwitchLibrary.Models.API.Clips;
using TwitchLibrary.Models.API.Chat;
using TwitchLibrary.Models.API.Community;
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

        #region Channels

        /// <summary>
        /// Asynchronously gets the <see cref="ChannelOAuth"/> object associated with the client's authentication token.
        /// Required scope: 'channel_read'
        /// </summary>
        public async Task<ChannelOAuth> GetChannelAsync()
        {
            RestRequest request = Request("channel", Method.GET);

            IRestResponse<ChannelOAuth> response = await twitch_api_client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously sets the title of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<ChannelOAuth> SetTitleAsync(string status)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { status } });

            IRestResponse<ChannelOAuth> response = await twitch_api_client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously sets the game of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<ChannelOAuth> SetGameAsync(string game)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { game } });

            IRestResponse<ChannelOAuth> response = await twitch_api_client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously sets the delay of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<ChannelOAuth> SetDelayAsync(int delay)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { delay } });

            IRestResponse<ChannelOAuth> response = await twitch_api_client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously enables or disables the channel feed of the channel associated with the client's authentication token.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<ChannelOAuth> EnableChannelFeedAsync(bool channel_feed_enabled)
        {
            RestRequest request = Request("channels/{channel_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { channel_feed_enabled } });

            IRestResponse<ChannelOAuth> response = await twitch_api_client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all editors for the channel associated with the client's authentication token.
        /// Required scope: 'channel_read'
        /// </summary>
        public async Task<Editors> GetChannelEditorsAsync()
        {
            RestRequest request = Request("channels/{channel_id}/editors", Method.GET);
            request.AddUrlSegment("channel_id", api_id);

            IRestResponse<Editors> response = await twitch_api_client.ExecuteTaskAsync<Editors>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of subscribers for the channel associated with the client's authentication token.
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

            IRestResponse<ChannelSubscribersPage> response = await twitch_api_client.ExecuteTaskAsync<ChannelSubscribersPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all subscribers for the channel associated with the client's authentication token in ascending order.
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
        /// Asynchronously gets the subscriber relationship the channel associated with the client's authentication token and a user.
        /// Returns status '404' if the user is not subscribed to the channel.
        /// Required scope: 'channel_check_subscription'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>
        public async Task<ChannelSubscriber> GetChannelSubscriberRelationshipAsync(string user_id)
        {
            RestRequest request = Request("channels/{channel_id}/subscriptions/{user_id}", Method.GET);
            request.AddUrlSegment("user_id", user_id);
            request.AddUrlSegment("channel_id", api_id);

            IRestResponse<ChannelSubscriber> response = await twitch_api_client.ExecuteTaskAsync<ChannelSubscriber>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously checks to see if a user is subsdcribed to the channel associated with the client's authentication token.        
        /// Intended for use by the the channel owner.
        /// Required scope: 'channel_check_subscription'
        /// NOTE: Works in theory, can't test because I'm not a partner.
        /// </summary>        
        public async Task<bool> isUserSubscribedAsync(string user_id)
        {
            ChannelSubscriber subscriber = await GetChannelSubscriberRelationshipAsync(user_id);

            return subscriber.http_status != 404;
        }

        /// <summary>
        /// Asynchronously starts a commercial for the channel associated with the client's authentication token.        
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

            IRestResponse<CommercialResponse> response = await twitch_api_client.ExecuteTaskAsync<CommercialResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously resets the stream key for the channel associated with the client's authentication token.
        /// Required scope: 'channel_stream'
        /// </summary>
        public async Task<ChannelOAuth> ResetStreamKeyAsync()
        {
            RestRequest request = Request("channels/{channel_id}/stream_key", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);

            IRestResponse<ChannelOAuth> response = await twitch_api_client.ExecuteTaskAsync<ChannelOAuth>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously sets the community for the channel associated with the client's authentication token.        
        /// Returns status '204' if the community was successfully set.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<HttpStatusCode> SetChannelCommunityAsync(string community_id)
        {
            RestRequest request = Request("channels/{channel_id}/community/{community_id}", Method.PUT);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("community_id", community_id);

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously removes a channel from their community.        
        /// Intended for use by a channel in the community.
        /// Returns status '204' if the channel was successfully removed.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<HttpStatusCode> DeleteChannelFromCommunityAsync()
        {
            RestRequest request = Request("channels/{channel_id}/community", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously removes a channel from their community.        
        /// Intended for use by the community owner.
        /// Returns status '204' if the channel was successfully removed.
        /// Required scope: 'channel_editor'
        /// </summary>
        public async Task<HttpStatusCode> DeleteChannelFromCommunityAsync(string channel_id)
        {
            RestRequest request = Request("channels/{channel_id}/community", Method.DELETE);
            request.AddUrlSegment("channel_id", channel_id);            

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        #endregion

        #region Clips

        /// <summary>
        /// Asynchronously gets a single paged list of clips from the games the user associated with the client's authentication token is following highest to lowest view count.
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

            IRestResponse<ClipsPage> response = await twitch_api_client.ExecuteTaskAsync<ClipsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of clips from the games the user associated with the client's authentication token is following highest to lowest view count.
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

        #region Communities

        /// <summary>
        /// Asynchronously updates the summary of a community.
        /// The max summary size is 160 characters.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// </summary>
        public async Task<HttpStatusCode> UpdateCommunitySummaryAsync(string community_id, string summary)
        {
            RestRequest request = Request("communities/{community_id}", Method.PUT);
            request.AddUrlSegment("community_id", community_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { summary });

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously updates the description of a community.
        /// The max description size is 1572864 (1.5MB) characters.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// </summary>
        public async Task<HttpStatusCode> UpdateCommunityDescriptionAsync(string community_id, string description)
        {
            RestRequest request = Request("communities/{community_id}", Method.PUT);
            request.AddUrlSegment("community_id", community_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { description });

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously updates the rules of a community.
        /// The max rules size is 1572864 (1.5MB) characters.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// </summary>
        public async Task<HttpStatusCode> UpdateCommunityRulesAsync(string community_id, string rules)
        {
            RestRequest request = Request("communities/{community_id}", Method.PUT);
            request.AddUrlSegment("community_id", community_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { rules });

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously updates the email of a community.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// </summary>
        public async Task<HttpStatusCode> UpdateCommunityEmailAsync(string community_id, string email)
        {
            RestRequest request = Request("communities/{community_id}", Method.PUT);
            request.AddUrlSegment("community_id", community_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { email });

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of banned community users.
        /// <see cref="PagingBannedCommunityUsers"/> can be specified to request a custom paged result.
        /// Required scope: 'communities_moderate'
        /// </summary>
        public async Task<BannedCommunityUsersPage> GetBannedCommunityUsersPageAsync(string community_id, PagingBannedCommunityUsers paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingBannedCommunityUsers();
            }

            RestRequest request = Request("communities/{community_id}/bans", Method.GET);
            request.AddUrlSegment("community_id", community_id);
            request = paging.Add(request);

            IRestResponse<BannedCommunityUsersPage> response = await twitch_api_client.ExecuteTaskAsync<BannedCommunityUsersPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of banned community users.
        /// Required scope: 'communities_moderate'
        /// </summary>
        public async Task<List<BannedCommunityUser>> GetBannedCommunityUsersAsync(string community_id)
        {
            PagingBannedCommunityUsers paging = new PagingBannedCommunityUsers();
            paging.limit = 100;

            List<BannedCommunityUser> banned_users = await Paging.GetPagesByCursorAsync<BannedCommunityUser, BannedCommunityUsersPage, PagingBannedCommunityUsers>(GetBannedCommunityUsersPageAsync, community_id, paging, "banned_users");

            return banned_users;
        }

        /// <summary>
        /// Asynchronously bans a community user.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_moderate'
        /// </summary>
        public async Task<HttpStatusCode> BanCommunityUserAsync(string community_id, string user_id)
        {
            RestRequest request = Request("communities/{community_id}/bans/{user_id}", Method.PUT);
            request.AddUrlSegment("community_id", community_id);
            request.AddUrlSegment("user_id", user_id);

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously unbans a community user.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_moderate'
        /// </summary>
        public async Task<HttpStatusCode> UnbanCommunityUserAsync(string community_id, string user_id)
        {
            RestRequest request = Request("communities/{community_id}/bans/{user_id}", Method.DELETE);
            request.AddUrlSegment("community_id", community_id);
            request.AddUrlSegment("user_id", user_id);

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously creates a community avatar image.
        /// The image must be 600 x 800 and no larger than 1MB.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// <param name="avatar_image">64 bit encoded image string</param>
        /// </summary>
        public async Task<HttpStatusCode> CreateCommunityAvatarAsync(string community_id, string avatar_image)
        {
            RestRequest request = Request("communities/{community_id}/images/avatar", Method.POST);
            request.AddUrlSegment("community_id", community_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { avatar_image });

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously uploads a community avatar image from a local file path.
        /// The image must be 600 x 800 and no larger than 1MB.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// </summary>
        public async Task<HttpStatusCode> UploadCommunityAvatarAsync(string community_id, string file_path)
        {
            byte[] image_bytes = File.ReadAllBytes(file_path);
            string avatar_image = Convert.ToBase64String(image_bytes);

            HttpStatusCode status = await CreateCommunityAvatarAsync(community_id, avatar_image);

            return status;
        }

        /// <summary>
        /// Asynchronously creates a community avatar image.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// </summary>
        public async Task<HttpStatusCode> DeleteCommunityAvatarAsync(string community_id)
        {
            RestRequest request = Request("communities/{community_id}/images/avatar", Method.DELETE);
            request.AddUrlSegment("community_id", community_id);

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously creates a community cover image.        
        /// The image must be 1200 x 180 and no larger than 3MB.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// <param name="cover_image">64 bit encoded image string</param>
        /// </summary>
        public async Task<HttpStatusCode> CreateCommunityCoverAsync(string community_id, string cover_image)
        {
            RestRequest request = Request("communities/{community_id}/images/cover", Method.POST);
            request.AddUrlSegment("community_id", community_id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { cover_image });

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously uploads a community cover image from a local file path.
        /// The image must be 1200 x 180 and no larger than 3MB.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// </summary>
        public async Task<HttpStatusCode> UploadCommunityCoverAsync(string community_id, string file_path)
        {
            byte[] image_bytes = File.ReadAllBytes(file_path);
            string cover_image = Convert.ToBase64String(image_bytes);

            HttpStatusCode status = await CreateCommunityCoverAsync(community_id, cover_image);

            return status;
        }

        /// <summary>
        /// Asynchronously deletes a community cover image from a local file path.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// </summary>
        public async Task<HttpStatusCode> DeleteCommunityCoverAsync(string community_id)
        {
            RestRequest request = Request("communities/{community_id}/images/cover", Method.DELETE);
            request.AddUrlSegment("community_id", community_id);

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously adds a community moderator.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// </summary>
        public async Task<HttpStatusCode> AddCommunityModeratorAsync(string community_id, string user_id)
        {
            RestRequest request = Request("communities/{community_id}/moderators/{user_id}", Method.PUT);
            request.AddUrlSegment("community_id", community_id);
            request.AddUrlSegment("user_id", user_id);

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously deletes a community moderator.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_edit'
        /// </summary>
        public async Task<HttpStatusCode> DeleteCommunityModeratorAsync(string community_id, string user_id)
        {
            RestRequest request = Request("communities/{community_id}/moderators/{user_id}", Method.DELETE);
            request.AddUrlSegment("community_id", community_id);
            request.AddUrlSegment("user_id", user_id);

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously gets a list of actions users can perform in a specified community.
        /// Required scope: 'any'
        /// </summary>
        public async Task<CommunityPermissions> GetCommunityPermissionsAsync(string community_id)
        {
            RestRequest request = Request("communities/{community_id}/permissions", Method.GET);
            request.AddUrlSegment("community_id", community_id);

            IRestResponse<CommunityPermissions> response = await twitch_api_client.ExecuteTaskAsync<CommunityPermissions>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of timed out community users.
        /// <see cref="PagingTimedOutCommunityUsers"/> can be specified to request a custom paged result.
        /// Required scope: 'communities_moderate'
        /// </summary>
        public async Task<TimedOutCommunityUsersPage> GetTimedOutCommunityUsersPageAsync(string community_id, PagingTimedOutCommunityUsers paging = null)
        {
            if (paging.isNull())
            {
                paging = new PagingTimedOutCommunityUsers();
            }

            RestRequest request = Request("communities/{community_id}/timeouts", Method.GET);
            request.AddUrlSegment("community_id", community_id);
            request = paging.Add(request);

            IRestResponse<TimedOutCommunityUsersPage> response = await twitch_api_client.ExecuteTaskAsync<TimedOutCommunityUsersPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of timed out community users.
        /// Required scope: 'communities_moderate'
        /// </summary>
        public async Task<List<TimedOutCommunityUser>> GetTimedOutCommunityUsersAsync(string community_id)
        {
            PagingTimedOutCommunityUsers paging = new PagingTimedOutCommunityUsers();
            paging.limit = 100;

            List<TimedOutCommunityUser> timed_outs_users = await Paging.GetPagesByCursorAsync<TimedOutCommunityUser, TimedOutCommunityUsersPage, PagingTimedOutCommunityUsers>(GetTimedOutCommunityUsersPageAsync, community_id, paging, "timed_out_users");

            return timed_outs_users;
        }

        /// <summary>
        /// Asynchronously times out a community user for a number of hours.
        /// Minimum duration is 1 hour.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_moderate'
        /// </summary>
        public async Task<HttpStatusCode> TimeOutCommunityUserAsync(string community_id, string user_id, int duration, string reason = "")
        {            
            duration = duration.ClampMin(1);

            RestRequest request = Request("communities/{community_id}/timeouts/{user_id}", Method.PUT);
            request.AddUrlSegment("community_id", community_id);
            request.AddUrlSegment("user_id", user_id);
            request.RequestFormat = DataFormat.Json;
            if (reason.isValidString())
            {
                request.AddBody(new { duration, reason });
            }
            else
            {
                request.AddBody(new { duration });
            }            

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously deletes a times out a community user.
        /// Returns status '204' if the operation was successful.
        /// Required scope: 'communities_moderate'
        /// </summary>
        public async Task<HttpStatusCode> DeleteTimeOutCommunityUserAsync(string community_id, string user_id)
        {
            RestRequest request = Request("communities/{community_id}/timeouts/{user_id}", Method.DELETE);
            request.AddUrlSegment("community_id", community_id);
            request.AddUrlSegment("user_id", user_id);

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        #endregion

        #region Feed

        /// <summary>
        /// Asynchronously gets a single paged list of posts in the feed of the channel associated with the client's authentication token.
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

            IRestResponse<PostsPage> response = await twitch_api_client.ExecuteTaskAsync<PostsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of posts in the feed of the channel associated with the client's authentication token from newest to oldest.        
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
        /// Asynchronously gets a post in the feed of the channel associated with the client's authentication token.                
        /// Required scope: 'any'   
        /// </summary>
        public async Task<Post> GetChannelFeedPostAsync(string post_id, int comments = 5)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}", Method.GET);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request.AddParameter("comments", comments.Clamp(0, 5, 5));

            IRestResponse<Post> response = await twitch_api_client.ExecuteTaskAsync<Post>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously creates a post in the feed of the channel associated with the client's authentication token.
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

            IRestResponse<CreatedPost> response = await twitch_api_client.ExecuteTaskAsync<CreatedPost>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously deletes a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Post"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<Post> DeleteChannelFeedPostAsync(string post_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());

            IRestResponse<Post> response = await twitch_api_client.ExecuteTaskAsync<Post>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously creates a reaction to a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="CreateReactionResponse"/> object is returned;. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<CreateReactionResponse> CreateChannelFeedPostReactionAsync(string post_id, string emote_id = "endorse")
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/reactions", Method.POST);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);            
            request.AddQueryParameter("emote_id", emote_id);

            IRestResponse<CreateReactionResponse> response = await twitch_api_client.ExecuteTaskAsync<CreateReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously deletes a reaction to a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="DeleteReactionResponse"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<DeleteReactionResponse> DeleteChannelFeedPostReactionAsync(string post_id, string emote_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/reactions", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id);
            request.AddQueryParameter("emote_id", emote_id);

            IRestResponse<DeleteReactionResponse> response = await twitch_api_client.ExecuteTaskAsync<DeleteReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of comments to a post in the feed of the channel associated with the client's authentication token.
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

            IRestResponse<PostCommentsPage> response = await twitch_api_client.ExecuteTaskAsync<PostCommentsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of comments in a post in the feed of the channel associated with the client's authentication token.        
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
        /// Asynchronously creates a comment on a post in the feed of the channel associated with the client's authentication token.
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

            IRestResponse<Comment> response = await twitch_api_client.ExecuteTaskAsync<Comment>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously deletes a comment on a post in the feed of the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="Comment"/> object is returned with "deleted" = true. 
        /// Required scope: 'channel_feed_edit'
        /// </summary>
        public async Task<Comment> DeleteChannelFeedPostCommentAsync(string post_id, string comment_id)
        {
            RestRequest request = Request("feed/{channel_id}/posts/{post_id}/comments/{comment_id}", Method.DELETE);
            request.AddUrlSegment("channel_id", api_id);
            request.AddUrlSegment("post_id", post_id.ToString());
            request.AddUrlSegment("comment_id", comment_id.ToString());

            IRestResponse<Comment> response = await twitch_api_client.ExecuteTaskAsync<Comment>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously creates a reaction to a comment in the feed of the channel associated with the client's authentication token.
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

            IRestResponse<CreateReactionResponse> response = await twitch_api_client.ExecuteTaskAsync<CreateReactionResponse>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously deletes a reaction to a comment in the feed of the channel associated with the client's authentication token.
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

            IRestResponse<DeleteReactionResponse> response = await twitch_api_client.ExecuteTaskAsync<DeleteReactionResponse>(request);

            return response.Data;
        }

        #endregion

        #region Streams

        /// <summary>
        /// Asynchronously gets a single paged list of streams that the channel associated with the client's authentication token is following.
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

            IRestResponse<StreamsPage> response = await twitch_api_client.ExecuteTaskAsync<StreamsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of streams that the channel associated with the client's authentication token is following.        
        /// Required scope: 'user_read'
        /// </summary>        
        public async Task<List<Models.API.Streams.Stream>> GetStreamFollowsAsync(StreamType stream_type = StreamType.LIVE)
        {
            PagingStreamFollows paging = new PagingStreamFollows();
            paging.limit = 100;
            paging.stream_type = stream_type;

            List<Models.API.Streams.Stream> following = await Paging.GetPagesByTotalAsync<Models.API.Streams.Stream, StreamsPage, PagingStreamFollows>(GetStreamFollowsPageAsync, paging, "streams");

            return following;
        }

        #endregion

        #region Users

        /// <summary>
        /// Asynchronously gets the <see cref="UserOAuth"/> object associated with the client's authentication token.
        /// Required scope: 'user_read'
        /// </summary>
        public async Task<UserOAuth> GetUserAsync()
        {
            RestRequest request = Request("user", Method.GET);

            IRestResponse<UserOAuth> response = await twitch_api_client.ExecuteTaskAsync<UserOAuth>(request);

            return response.Data;            
        }

        /// <summary>
        /// Asynchronously gets all emote sets and sub emotes that the user associated with the client's authentication token can use in chat.
        /// Required scope: 'user_subscriptions'
        /// </summary>
        public async Task<EmoteSet> GetUserEmotesAsync()
        {
            RestRequest request = Request("users/{user_id}/emotes", Method.GET);
            request.AddUrlSegment("user_id", api_id);

            IRestResponse<EmoteSet> response = await twitch_api_client.ExecuteTaskAsync<EmoteSet>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets the subscriber relationship between a user and the channel associated with the client's authentication token.        
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

            IRestResponse<UserSubscriber> response = await twitch_api_client.ExecuteTaskAsync<UserSubscriber>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously checks to see if a user is subscribed to a channel.        
        /// If the client's authentication token does not have the authorization permission to make the request, the method will return "false" even if the user is actually subscribed to the channel.                 
        /// Intended for use by the the viewer/user, the one who would be subscribed.
        /// Required scope: 'user_subscriptions'
        /// </summary>
        public async Task<bool> isUserSubscribedAsync(string user_id, string channel_id)
        {
            UserSubscriber relationship = await GetUserSubscriberRelationshipAsync(user_id, channel_id);
            
            return relationship.http_status != 404 && relationship.http_status != 422 && relationship.http_status != 403;
        }

        /// <summary>
        /// Asynchronously makes the channel associated with the client's authentication token follow a user.
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

            IRestResponse<Follow> response = await twitch_api_client.ExecuteTaskAsync<Follow>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously makes the channel associated with the client's authentication token follow a user.
        /// Returns status '204' if the client successfully unfollowed the user.
        /// Required scope: 'user_follows_edit' 
        /// </summary>
        public async Task<HttpStatusCode> UnfollowAsync(string user_id)
        {
            RestRequest request = Request("users/{user_id}/follows/channels/{channel_id}", Method.DELETE);
            request.AddUrlSegment("user_id", api_id);
            request.AddUrlSegment("channel_id", user_id);

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously gets a single paged list of blocked users for the channel associated with the client's authentication token.
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

            IRestResponse<BlockedUsersPage> response = await twitch_api_client.ExecuteTaskAsync<BlockedUsersPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all blocked users for the channel associated with the client's authentication token in ascending order from newest to oldest.
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
        /// Asynchronously blocks a user for the channel associated with the client's authentication token.
        /// If the operation was sucessful, the <see cref="BlockedUser"/> object is returned;. 
        /// Required scope: 'user_blocks_edit'
        /// </summary>
        public async Task<BlockedUser> BlockAsync(string user_id)
        {
            RestRequest request = Request("users/{user_id}/blocks/{channel_id}", Method.PUT);
            request.AddUrlSegment("user_id", api_id);
            request.AddUrlSegment("channel_id", user_id);

            IRestResponse<BlockedUser> response = await twitch_api_client.ExecuteTaskAsync<BlockedUser>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously blocks a user for the channel associated with the client's authentication token.
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

            IRestResponse<object> response = await twitch_api_client.ExecuteTaskAsync<object>(request);

            return response.StatusCode;
        }

        #endregion

        #region Videos

        /// <summary>
        /// Asynchronously gets a single paged list of videos from the users the channel associated with the client's authentication token is following.
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

            IRestResponse<VideosFollowsPage> response = await twitch_api_client.ExecuteTaskAsync<VideosFollowsPage>(request);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all the archives from the users the channel associated with the client's authentication token is following.
        /// Required scope: 'user_read'
        /// </summary>        
        public async Task<List<Video>> GetUserFollowsArchivesAsync()
        {
            List<Video> archives = await GetUserFollowsVideosAsync(new BroadcastType[] { BroadcastType.ARCHIVE });

            return archives;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all the highlights from the users the channel associated with the client's authentication token is following.
        /// Required scope: 'user_read'
        /// </summary>        
        public async Task<List<Video>> GetUserFollowsHighlightsAsync()
        {
            List<Video> highlights = await GetUserFollowsVideosAsync(new BroadcastType[] { BroadcastType.HIGHLIGHT });

            return highlights;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all the uploads from the users the channel associated with the client's authentication token is following.
        /// Required scope: 'user_read'
        /// </summary>        
        public async Task<List<Video>> GetUserFollowsUploadsAsync()
        {
            List<Video> uploads = await GetUserFollowsVideosAsync(new BroadcastType[] { BroadcastType.UPLOAD });

            return uploads;
        }

        /// <summary>
        /// Asynchronously gets a complete list of all the videos (archives, uploads, and highlights) from the users the channel associated with the client's authentication token is following.
        /// Required scope: 'user_read'
        /// </summary>
        public async Task<List<Video>> GetUserFollowsVideosAsync()
        {
            List<Video> videos = await GetUserFollowsVideosAsync(new BroadcastType[] { BroadcastType.ARCHIVE, BroadcastType.HIGHLIGHT, BroadcastType.UPLOAD });

            return videos;
        }

        /// <summary>
        /// Asynchronously gets a complete list of videos from the users the channel associated with the client's authentication token is following.
        /// Required scope: 'user_read'
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

        #region Video Upload 

        /// <summary>
        /// Asynchronously creates a video upload request to a specified channel_name.
        /// The <see cref="CreatedVideo"/> response contains the video upload token and a partially filled out <see cref="Video"/> object is successfull.
        /// The operation can be retried up to 3 times if status 500 is returned from the server and "auto_retry" is set to true.
        /// Required scope: 'channel_editor'
        /// </summary>
        /// <returns></returns>
        public async Task<CreatedVideo> CreateVideoUploadAsync(string channel_name, string title, bool auto_retry = false)
        {
            CreatedVideo created_video = await CreateVideoUploadAsync(channel_name, title, "", "", auto_retry);

            return created_video;
        }

        /// <summary>
        /// Asynchronously creates a video upload request to a specified channel_name.
        /// The <see cref="CreatedVideo"/> response contains the video upload token and a partially filled out <see cref="Video"/> object is successfull.
        /// The operation can be retried up to 3 times if status 500 is returned from the server and "auto_retry" is set to true.
        /// Required scope: 'channel_editor'
        /// </summary>
        /// <returns></returns>
        public async Task<CreatedVideo> CreateVideoUploadAsync(string channel_name, string title, string description, bool auto_retry = false)
        {
            CreatedVideo created_video = await CreateVideoUploadAsync(channel_name, title, description, "", auto_retry);

            return created_video;
        }

        /// <summary>
        /// Asynchronously creates a video upload request to a specified channel_name.
        /// The <see cref="CreatedVideo"/> response contains the video upload token and a partially filled out <see cref="Video"/> object is successfull.
        /// The operation can be retried up to 3 times if status 500 is returned from the server and "auto_retry" is set to true.
        /// Required scope: 'channel_editor'
        /// </summary>
        /// <returns></returns>
        public async Task<CreatedVideo> CreateVideoUploadAsync(string channel_name, string title, string description, string tags, bool auto_retry = false)
        {
            int retry_limit = 3;
            int retry_count = 0;

            IRestResponse<CreatedVideo> response;

            do
            {
                RestRequest request = Request("videos", Method.POST, ApiVersion.v4);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new { channel_name, title, description, tags });

                response = await twitch_api_client.ExecuteTaskAsync<CreatedVideo>(request);

                if(response.StatusCode == HttpStatusCode.InternalServerError && auto_retry)
                {
                    ++retry_count;
                }
            }
            while (response.StatusCode == HttpStatusCode.InternalServerError &&
                   auto_retry &&
                   retry_count < retry_limit);

            return response.Data;
        }

        /// <summary>
        /// Asynchronously uploads a video part to the specified video_id.
        /// The video part data must be between 5MB and 25MB, unless it is the last part to be uploaded.
        /// This method does not inherently require an oauth token or a scope, but 'channel_editor' is required to initialize the upload.
        /// The operation can be retried up to 3 times if status 500 is returned from the server and "auto_retry" is set to true.
        /// Required scope: 'channel_editor'
        /// </summary>
        /// <returns></returns>
        public async Task<HttpStatusCode> UploadVideoPartAsync(string video_id, int part, string upload_token, byte[] data, bool auto_retry = false)
        {
            int retry_limit = 3;
            int retry_count = 0;

            IRestResponse<object> response;

            do
            {
                RestRequest request = Request("{video_id}", Method.PUT, ApiVersion.v4);
                       
                request.AddUrlSegment("video_id", video_id);
                request.AddQueryParameter("part", part.ToString());
                request.AddQueryParameter("upload_token", upload_token);
                request.AddHeader("Content-Length", data.Length.ToString());
                request.AddParameter("application/bson", data, ParameterType.RequestBody);

                response = await uploads_api_client.ExecuteTaskAsync<object>(request);

                if (response.StatusCode == HttpStatusCode.InternalServerError && auto_retry)
                {
                    ++retry_count;
                }
            }
            while (response.StatusCode == HttpStatusCode.InternalServerError &&
                   auto_retry &&
                   retry_count < retry_limit);

            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously completes the video upload to twitch once all video parts are uploaded to twitch.
        /// This method does not inherently require an oauth token or a scope, but 'channel_editor' is required to initialize the upload.
        /// The operation can be retried up to 3 times if status 500 is returned from the server and "auto_retry" is set to true.
        /// Required scope: 'channel_editor'
        /// </summary>
        /// <returns></returns>
        public async Task<HttpStatusCode> CompleteVideoUploadAsync(string video_id, string upload_token, bool auto_retry = false)
        {
            int retry_limit = 3;
            int retry_count = 0;

            IRestResponse<object> response;

            do
            {
                RestRequest request = Request("{video_id}/complete", Method.POST, ApiVersion.v4);
                request.AddUrlSegment("video_id", video_id);
                request.AddQueryParameter("upload_token", upload_token);

                response = await uploads_api_client.ExecuteTaskAsync<object>(request);

                if (response.StatusCode == HttpStatusCode.InternalServerError && auto_retry)
                {
                    ++retry_count;
                }
            }
            while (response.StatusCode == HttpStatusCode.InternalServerError &&
                   auto_retry &&
                   retry_count < retry_limit);


            return response.StatusCode;
        }

        /// <summary>
        /// Asynchronously uploads a video to twitch from a local file path to a specified channel.
        /// Automatically handles the video creation, part separation, part uploads, and upload completetion to Twitch.
        /// Video format:   MP4,
        /// Audio format:   AAC,
        /// Video codec:    H.264,
        /// Video bitrate:  up to 10Mbps,
        /// Resolution:     up to 1080p,
        /// FPS:            up to 60FPS.
        /// The operation can be retried up to 3 times if status 500 is returned from the server and "auto_retry" is set to true.
        /// Required scope: 'channel_editor'
        /// </summary>
        /// <returns></returns>
        public async Task<HttpStatusCode> UploadVideoAsync(string path, string channel_name, string video_title, bool auto_retry = false)
        {
            HttpStatusCode upload_video_status = await UploadVideoAsync(path, channel_name, video_title, "", "", auto_retry);

            return upload_video_status;
        }

        /// <summary>
        /// Asynchronously uploads a video to twitch from a local file path to a specified channel.
        /// Automatically handles the video creation, part separation, part uploads, and upload completetion to Twitch.
        /// Video format:   MP4,
        /// Audio format:   AAC,
        /// Video codec:    H.264,
        /// Video bitrate:  up to 10Mbps,
        /// Resolution:     up to 1080p,
        /// FPS:            up to 60FPS.
        /// The operation can be retried up to 3 times if status 500 is returned from the server and "auto_retry" is set to true.
        /// Required scope: 'channel_editor'
        /// </summary>
        /// <returns></returns>
        public async Task<HttpStatusCode> UploadVideoAsync(string path, string channel_name, string video_title, string description, bool auto_retry = false)
        {
            HttpStatusCode upload_video_status = await UploadVideoAsync(path, channel_name, video_title, description, "", auto_retry);

            return upload_video_status;
        }

        /// <summary>
        /// Asynchronously uploads a video to twitch from a local file path to a specified channel.
        /// Automatically handles the video creation, part separation, part uploads, and upload completetion to Twitch.
        /// Max video size: 10GB
        /// Video format:   MP4,
        /// Audio format:   AAC,
        /// Video codec:    H.264,
        /// Video bitrate:  up to 10Mbps,
        /// Resolution:     up to 1080p,
        /// FPS:            up to 60FPS.
        /// The operation can be retried up to 3 times if status 500 is returned from the server and "auto_retry" is set to true.
        /// Required scope: 'channel_editor'
        /// </summary>
        /// <returns></returns>
        public async Task<HttpStatusCode> UploadVideoAsync(string path, string channel_name, string video_title, string description, string tags, bool auto_retry = false)
        {
            long max_file_size = (long)10 * 1024 * 1024 * 1024;

            FileInfo info = new FileInfo(path);

            //file trying to be uploaded doesn't exist
            if (!info.Exists)
            {
                return HttpStatusCode.NotFound;
            }

            //file trying to be uploaded is larger than 10GB
            if (info.Length > max_file_size)
            {
                return HttpStatusCode.RequestEntityTooLarge;
            }

            //create the video upload request
            CreatedVideo created_video = await CreateVideoUploadAsync(channel_name, video_title, description, tags, auto_retry);

            int part = 1;
            int chunk_size = 20 * 1024 * 1024;

            //read the file into memory at 20MB parts in case the user doesn't haver a shit ton of RAM
            using (FileStream file = File.OpenRead(path))
            {
                byte[] buffer = new byte[chunk_size];

                while (file.Read(buffer, 0, buffer.Length) > 0)
                {
                    //upload each 20MB part
                    HttpStatusCode upload_part_status = await UploadVideoPartAsync(created_video.video._id, part, created_video.upload.token, buffer);

                    ++part;
                }
            }

            //finish the upload process to twitch
            HttpStatusCode upload_video_status = await CompleteVideoUploadAsync(created_video.video._id, created_video.upload.token);

            return upload_video_status;
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