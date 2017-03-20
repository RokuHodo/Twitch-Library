using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//project namespaces
using TwitchLibrary.Extensions;

//improted .dll's
using RestSharp;

namespace TwitchLibrary.Helpers.Paging.Collections
{
    public class PagingChannelCollections : PagingLimit
    {
        public string cursor;
        public string containing_item;

        public PagingChannelCollections() : base(10)
        {

        }

        public PagingChannelCollections(int _limit, string _cursor, string _containing_item) : base(10)
        {
            limit = _limit;
            cursor = _cursor;
            containing_item = _containing_item;
        }

        /// <summary>
        /// Sets how to recieve the <see cref="RestRequest"/> when getting paged results.
        /// </summary>
        public new RestRequest Add(RestRequest request)
        {
            request.AddQueryParameter("limit", limit.ToString());

            if (cursor.isValidString())
            {
                request.AddQueryParameter("cursor", cursor);
            }

            if (containing_item.isValidString())
            {
                //really weird query parameter "video:<video_id>"
                request.AddQueryParameter("containing_item", "video:" + containing_item);
            }

            return request;
        }
    }
}
