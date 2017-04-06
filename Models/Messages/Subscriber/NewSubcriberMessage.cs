//project namespaces
using TwitchLibrary.Models.Messages.Private;

namespace TwitchLibrary.Models.Messages.Subscriber
{
    public class UserSubcribedMessage
    {
        //twitch prime
        public bool is_premium { get; protected set; }

        public string room_name { get; protected set; }
        public string body { get; protected set; }

        public NewSubcriberSender sender { get; protected set; }

        public UserSubcribedMessage(PrivateMessage private_message)
        {
            is_premium = private_message.body.Contains("Twitch Prime");

            room_name = private_message.room_name;
            body = private_message.body;

            sender = new NewSubcriberSender(private_message);
        }
    }
}
