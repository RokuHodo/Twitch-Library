//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Feed
{   
    public class PagingChannelFeedPosts : PagingLimit, ITwitchPaging
    {
        //private
        private int _comments;
        private int comments_default = 5;

        //public
        public readonly int comments_min = 5;
        public readonly int comments_max = 5;
        public int comments
        {
            get { return _comments; }
            set { _comments = value.Clamp(comments_min, comments_max, comments_default); }
        }

        public string cursor;

        public PagingChannelFeedPosts() : base(10)
        {
            cursor = string.Empty;
        }

        public PagingChannelFeedPosts(int _limit, int _comments, string _cursor) : base(10)
        {
            limit = _limit;
            comments = _comments;
            cursor = _cursor;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit);
            request.AddParameter("comments", comments);

            if (cursor.isValidString())
            {
                request.AddParameter("cursor", cursor);
            }            

            return request;
        }
    }
}
