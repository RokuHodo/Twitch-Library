// imported .dll's
using RestSharp;

namespace TwitchLibrary.Interfaces.Helpers.Paging
{
    public interface ITwitchPaging
    {
        RestRequest Add(RestRequest request);
    }
}
