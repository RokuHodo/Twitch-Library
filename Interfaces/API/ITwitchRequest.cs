//imported .dll's
using RestSharp;

namespace TwitchLibrary.Interfaces.API
{
    public interface ITwitchRequest
    {
        RestRequest Request(string endpoint, Method method);
    }
}
