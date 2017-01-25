﻿//project namespaces
using TwitchLibrary.Extensions;
using TwitchLibrary.Interfaces.Helpers.Paging;

//imported .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Streams
{   
    public class PagingFeaturedStreams : ITwitchPaging
    {
        public int limit,       //max = 100         default = 25
                    offset;     //max = 1000        default = 0   

        public PagingFeaturedStreams()
        {
            limit = 25;
            offset = 0;            
        }

        public PagingFeaturedStreams(int _limit, int _offset)
        {
            limit = _limit;
            offset = _offset;            
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public RestRequest Add(RestRequest request)
        {
            request.AddParameter("limit", limit.Clamp(1, 100, 25));
            request.AddParameter("offset", offset.Clamp(0, 1000, 0));            

            return request;
        }
    }
}