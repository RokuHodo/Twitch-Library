// standard namespaces
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

// project namespaces
using TwitchLibrary.Extensions;

namespace TwitchLibrary.Helpers.Paging
{
    public static class Paging
    {
        #region Pages by cursor

        /// <summary>
        /// Asynchronously gets a complete list of objects using the passed request method, requested page cursor, paging, and property to iterate through.
        /// </summary>
        /// <typeparam name="Model">The object type that is returned in <see cref="List{T}"/> format.</typeparam>
        /// <typeparam name="PageResult">The object that is returned with each page request.</typeparam>
        /// <typeparam name="Paging">The data set that specifies how to request the each page.</typeparam>
        /// <param name="GetPage">The method that requests each page.</param>
        /// <param name="channel_id">Twitch channel id to request the information from.</param>
        /// <param name="paging">The instance of the Paging object that specifies how to request the each page.</param>
        /// <param name="property">The property of the requested page that stores the objects to be iterated through and retured.</param>
        /// <returns></returns>
        public static async Task<List<Model>> GetPagesByCursorAsync<Model, PageResult, Paging>(Func<string, Paging, Task<PageResult>> GetPage, string channel_id, Paging paging, string property) where Paging : new()
        {
            List<Model>     list                                = new List<Model>();

            // paging
            FieldInfo       paging_cursor_info                  = paging.GetType().GetField("cursor");

            // requested page
            PageResult      requested_page                      = await GetPage(channel_id, paging);
            PropertyInfo    requested_page_total_info           = requested_page.GetType().GetProperty("_total");
            PropertyInfo    requested_page_cursor_info          = requested_page.GetType().GetProperty("_cursor");
            PropertyInfo    requested_page_property_loop_info   = requested_page.GetType().GetProperty(property);

            if (!requested_page_total_info.isNull())
            {
                int _total = Convert.ToInt32(requested_page_total_info.GetValue(requested_page));

                if (_total == 0)
                {
                    return list;
                }
            }

            // requested page values
            string requested_page_cursor_value = (string)requested_page_cursor_info.GetValue(requested_page);
            List<Model> requested_page_property_loop_list = (List<Model>)requested_page_property_loop_info.GetValue(requested_page);

            bool requesting = true;

            do
            {
                foreach (Model follower in requested_page_property_loop_list)
                {
                    list.Add(follower);
                }

                if (requested_page_cursor_value.isValid())
                {
                    // update the paging cursor property to properly request the next page
                    paging_cursor_info.SetValue(paging, requested_page_cursor_value);

                    // request the new page and update the associated property variables
                    requested_page = await GetPage(channel_id, paging);
                    requested_page_cursor_value = (string)requested_page_cursor_info.GetValue(requested_page);
                    requested_page_property_loop_list = (List<Model>)requested_page_property_loop_info.GetValue(requested_page);
                }
                else
                {
                    requesting = false;
                }
            }
            while (requesting);

            return list;
        }

        /// <summary>
        /// Asynchronously gets a complete list of objects using the passed request method, requested page cursor, paging, and property to iterate through.
        /// </summary>
        /// <typeparam name="Model">The object type that is returned in <see cref="List{T}"/> format.</typeparam>
        /// <typeparam name="PageResult">The object that is returned with each page request.</typeparam>
        /// <typeparam name="Paging">The data set that specifies how to request the each page.</typeparam>
        /// <param name="GetPage">The method that requests each page.</param>
        /// <param name="paging">The instance of the Paging object that specifies how to request the each page.</param>
        /// <param name="property">The property of the requested page that stores the objects to be iterated through and retured.</param>
        /// <returns></returns>
        public static async Task<List<Model>> GetPagesByCursorAsync<Model, PageResult, Paging>(Func<Paging, Task<PageResult>> GetPage, Paging paging, string property) where Paging : new()
        {
            List<Model>     list                                = new List<Model>();

            // paging
            FieldInfo       paging_cursor_info                  = paging.GetType().GetField("cursor");

            // requested page
            PageResult      requested_page                      = await GetPage(paging);
            PropertyInfo    requested_page_total_info           = requested_page.GetType().GetProperty("_total");
            PropertyInfo    requested_page_cursor_info          = requested_page.GetType().GetProperty("_cursor");
            PropertyInfo    requested_page_property_loop_info   = requested_page.GetType().GetProperty(property);

            if (!requested_page_total_info.isNull())
            {
                int _total = Convert.ToInt32(requested_page_total_info.GetValue(requested_page));

                if (_total == 0)
                {
                    return list;
                }
            }

            // requested page values
            string requested_page_cursor_value = (string)requested_page_cursor_info.GetValue(requested_page);
            List<Model> requested_page_property_loop_list = (List<Model>)requested_page_property_loop_info.GetValue(requested_page);

            bool requesting = true;

            do
            {
                foreach (Model follower in requested_page_property_loop_list)
                {
                    list.Add(follower);
                }

                if (requested_page_cursor_value.isValid())
                {
                    // update the paging cursor property to properly request the next page
                    paging_cursor_info.SetValue(paging, requested_page_cursor_value);

                    // request the new page and update the associated property variables
                    requested_page = await GetPage(paging);
                    requested_page_cursor_value = (string)requested_page_cursor_info.GetValue(requested_page);
                    requested_page_property_loop_list = (List<Model>)requested_page_property_loop_info.GetValue(requested_page);
                }
                else
                {
                    requesting = false;
                }
            }
            while (requesting);

            return list;
        }

        #endregion

        #region Pages by total

        /// <summary>
        /// Asynchronously gets a complete list of objects using the passed request method, the requested page total, paging offset, and property to iterate through.
        /// </summary>
        /// <typeparam name="Model">The object type that is returned in <see cref="List{T}"/> format.</typeparam>
        /// <typeparam name="PageResult">The object that is returned with each page request.</typeparam>
        /// <typeparam name="Paging">The data set that specifies how to request the each page.</typeparam>
        /// <param name="GetPage">The method that requests each page.</param>
        /// <param name="channel_id">Twitch channel id to request the information from.</param>
        /// <param name="paging">The instance of the Paging object that specifies how to request the each page.</param>
        /// <param name="property">The property of the requested page that stores the objects to be iterated through and retured.</param>
        /// <returns></returns>
        public static async Task<List<Model>> GetPagesByTotalAsync<Model, PageResult, Paging>(Func<string, Paging, Task<PageResult>> GetPage, string channel_id, Paging paging, string property) where Paging : new()
        {
            List<Model>     list                                = new List<Model>();

            // paging
            FieldInfo       paging_offset_max_info              = paging.GetType().GetField("offset_max");
            PropertyInfo    paging_offset_info                  = paging.GetType().GetProperty("offset");
            PropertyInfo    paging_limit_info                   = paging.GetType().GetProperty("limit");            

            // requested page
            PageResult      requested_page                      = await GetPage(channel_id, paging);
            PropertyInfo    requested_page_total_info           = requested_page.GetType().GetProperty("_total");
            PropertyInfo    requested_page_property_loop_info   = requested_page.GetType().GetProperty(property);

            // page didn't actually have a "_total" property
            if (requested_page_total_info.isNull())
            {
                return list;
            }            

            // paging values
            int paging_offset_max_value = Convert.ToInt32(paging_offset_max_info.GetValue(paging));
            int paging_limit_value = Convert.ToInt32(paging_limit_info.GetValue(paging));

            // requested page values
            int requested_page_total_value = Convert.ToInt32(requested_page_total_info.GetValue(requested_page));
            List<Model> requested_page_property_loop_list = (List<Model>)requested_page_property_loop_info.GetValue(requested_page);

            // calculate the max number of pages that would be returned based on requested_page._total and paging.limit
            decimal pages_dec = requested_page_total_value / (decimal)paging_limit_value;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));
            int page = 0;

            // the total number of pages could return more results than could be offset, track offset to make sure we don't over request
            int offset = 0;

            bool requesting = true;

            do
            {
                foreach (Model follower in requested_page_property_loop_list)
                {
                    list.Add(follower);
                }

                ++page;
                offset = paging_limit_value * page;
                
                requesting = page < pages && offset <= paging_offset_max_value;

                if (requesting)
                {
                    // set the new offset to request the next page
                    paging_offset_info.SetValue(paging, offset);

                    // request the new page
                    requested_page = await GetPage(channel_id, paging);
                    requested_page_property_loop_list = (List<Model>)requested_page_property_loop_info.GetValue(requested_page);                    
                }
            }
            while (requesting);

            return list;
        }

        /// <summary>
        /// Asynchronously gets a complete list of objects using the passed request method, the requested page total, paging offset, and property to iterate through.
        /// </summary>
        /// <typeparam name="Model">The object type that is returned in <see cref="List{T}"/> format.</typeparam>
        /// <typeparam name="PageResult">The object that is returned with each page request.</typeparam>
        /// <typeparam name="Paging">The data set that specifies how to request the each page.</typeparam>
        /// <param name="GetPage">The method that requests each page.</param>
        /// <param name="paging">The instance of the Paging object that specifies how to request the each page.</param>
        /// <param name="property">The property of the requested page that stores the objects to be iterated through and retured.</param>
        /// <returns></returns>
        public static async Task<List<Model>> GetPagesByTotalAsync<Model, PageResult, Paging>(Func<Paging, Task<PageResult>> GetPage, Paging paging, string property) where Paging : new()
        {
            List<Model>     list                                = new List<Model>();

            // paging
            FieldInfo       paging_offset_max_info              = paging.GetType().GetField("offset_max");
            PropertyInfo    paging_offset_info                  = paging.GetType().GetProperty("offset");
            PropertyInfo    paging_limit_info                   = paging.GetType().GetProperty("limit");

            // requested page
            PageResult      requested_page                      = await GetPage(paging);
            PropertyInfo    requested_page_total_info           = requested_page.GetType().GetProperty("_total");
            PropertyInfo    requested_page_property_loop_info   = requested_page.GetType().GetProperty(property);

            // page didn't actually have a "_total" property
            if (requested_page_total_info.isNull())
            {
                return list;
            }

            // paging values
            int paging_offset_max_value = Convert.ToInt32(paging_offset_max_info.GetValue(paging));
            int paging_limit_value = Convert.ToInt32(paging_limit_info.GetValue(paging));

            // requested page values
            int requested_page_total_value = Convert.ToInt32(requested_page_total_info.GetValue(requested_page));
            List<Model> requested_page_property_loop_list = (List<Model>)requested_page_property_loop_info.GetValue(requested_page);

            // calculate the max number of pages that would be returned based on requested_page._total and paging.limit
            decimal pages_dec = requested_page_total_value / (decimal)paging_limit_value;
            int pages = Convert.ToInt32(Math.Ceiling(pages_dec));
            int page = 0;

            // the total number of pages could return more results than could be offset, track offset to make sure we don't over request
            int offset = 0;

            bool requesting = true;

            do
            {
                foreach (Model follower in requested_page_property_loop_list)
                {
                    list.Add(follower);
                }

                ++page;
                offset = paging_limit_value * page;

                requesting = page < pages && offset <= paging_offset_max_value;

                if (requesting)
                {
                    // set the new offset to request the next page
                    paging_offset_info.SetValue(paging, offset);

                    // request the new page
                    requested_page = await GetPage(paging);
                    requested_page_property_loop_list = (List<Model>)requested_page_property_loop_info.GetValue(requested_page);
                }
            }
            while (requesting);

            return list;
        }

        #endregion

        #region Pages by offset

        public static async Task<List<Model>> GetPagesByOffsetAsync<Model, PageResult, Paging>(Func<Paging, Task<PageResult>> GetPage, Paging paging, string property) where Paging : new()
        {
            List<Model>     list                                = new List<Model>();

            // paging
            FieldInfo       paging_offset_max_info              = paging.GetType().GetField("offset_max");
            PropertyInfo    paging_offset_info                  = paging.GetType().GetProperty("offset");
            PropertyInfo    paging_limit_info                   = paging.GetType().GetProperty("limit");

            // requested page
            PageResult      requested_page                      = await GetPage(paging);
            PropertyInfo    requested_page_property_loop_info   = requested_page.GetType().GetProperty(property);

            if (requested_page.isNull())
            {
                return list;
            }

            // paging values
            int paging_offset_max_value = Convert.ToInt32(paging_offset_max_info.GetValue(paging));
            int paging_limit_value = Convert.ToInt32(paging_limit_info.GetValue(paging));

            // requested page values
            List<Model> requested_page_property_loop_list = (List<Model>)requested_page_property_loop_info.GetValue(requested_page);

            if (!requested_page_property_loop_list.isValid())
            {
                return list;
            }

            // Twitch droppped the ball and didn't include _total or _cursor, just loop until we find nothing
            int page = 0;
            int offset = 0;

            bool requesting = true;

            do
            {
                foreach (Model follower in requested_page_property_loop_list)
                {
                    list.Add(follower);
                }

                ++page;
                offset = paging_limit_value * page;

                // initial request check
                requesting = offset <= paging_offset_max_value;

                if (requesting)
                {
                    // set the new offset to request the next page
                    paging_offset_info.SetValue(paging, offset);

                    // request the new page
                    requested_page = await GetPage(paging);
                    requested_page_property_loop_list = (List<Model>)requested_page_property_loop_info.GetValue(requested_page);

                    // the page doesn't contain any data, probably reached the end, stop requesting
                    if (requested_page.isNull() || !requested_page_property_loop_list.isValid())
                    {
                        requesting = false;
                    }
                }
            }
            while (requesting);

            return list;
        }

        #endregion
    }
}
