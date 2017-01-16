//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Games
{   
    public class PagingTopGames : ITwitchPaging
    {
        public long limit,      //max = 100     default = 10
                    offset;   

        public PagingTopGames()
        {
            limit = 10;
            offset = 0;            
        }

        public PagingTopGames(long _limit, long _offset)
        {
            limit = _limit;
            offset = _offset;            
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 10));
            request.AddParameter("offset", offset);            

            return request;
        }
    }
}
