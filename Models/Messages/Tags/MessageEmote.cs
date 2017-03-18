namespace TwitchLibrary.Models.Messages.Tags
{
    public class MessageEmote
    {
        public int count { get; internal set; }

        public string emote_id { get; internal set; }

        public MessageEmoteRange[] ranges { get; internal set; }  
    }
}
