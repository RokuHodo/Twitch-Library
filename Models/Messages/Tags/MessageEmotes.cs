namespace TwitchLibrary.Models.Messages.Tags
{
    public class MessageEmotes
    {
        public int count_total { get; internal set; }
        public int count_unique { get; internal set; }

        public MessageEmote[] emotes { get; internal set; }  
    }
}
