namespace TwitchLibrary.Models.Messages.Emotes
{
    public class MessageEmote
    {
        public int id { get; set; }
        public int count { get; set; }

        public MessageEmoteRange[] ranges { get; set; }  
    }
}
