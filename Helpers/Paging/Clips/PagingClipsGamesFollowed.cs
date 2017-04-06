//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Clips
{
    public class PagingClipsGamesFollowed : PagingLimit, ITwitchPaging
    {
        public bool trending;
        
        public string cursor;

        public PagingClipsGamesFollowed() : base(10)
        {
            trending = false;
            cursor = string.Empty;
        }

        public PagingClipsGamesFollowed(bool _trending, int _limit, string _cursor) : base(10)
        {
            trending = _trending;
            limit = _limit;            
            cursor = _cursor;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddQueryParameter("trending", trending.ToString().ToLower());
            request.AddQueryParameter("limit", limit.ToString());         

            if (cursor.isValid())
            {
                request.AddQueryParameter("cursor", cursor);
            }

            return request;
        }
    }
}
