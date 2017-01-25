//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Enums.Helpers.Paging;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Clips
{   
    public class PagingClipsGamesFollowed : ITwitchPaging
    {
        public bool trending;

        public long limit;      //max = 100         default = 10                   
        
        public string cursor;

        public PagingClipsGamesFollowed()
        {
            trending = false;
            limit = 10;
            cursor = string.Empty;
        }

        public PagingClipsGamesFollowed(bool _trending, long _limit, string _cursor)
        {
            trending = _trending;
            limit = _limit;            
            cursor = _cursor;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("trending", trending.ToString().ToLower());
            request.AddParameter("limit", limit.Clamp(1, 100, 10));         

            if (cursor.isValidString())
            {
                request.AddParameter("cursor", cursor);
            }

            return request;
        }
    }
}
