//standard namespaces
using System;
using System.Collections.Generic;
using System.Drawing;

//project namespaces
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Extensions;
using TwitchLibrary.Models.Messages.Tags;

namespace TwitchLibrary.Helpers.Messages
{
    public static class TagConverter
    {
        internal static type ToGeneric<type>(Dictionary<string, string> tags, string key)
        {
            type value = default(type);            

            if (!key.isValid())
            {
                return value;
            }

            tags.TryGetValue(key, out string value_string);

            if (!value_string.isValid())
            {
                return value;
            }

            if (!value_string.CanCovertTo<type>())
            {
                return value;
            }

            try
            {
                value = (type)Convert.ChangeType(value_string, typeof(type));             
            }
            catch(Exception exception)
            {
                LibraryDebug.Error(LibraryDebugMethod.CONVERT, "tag", LibraryDebugError.NORMAL_EXCEPTION, TimeStamp.TimeLong);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
                LibraryDebug.PrintLine(nameof(value_string), value_string);
            }

            return value;
        }

        public static bool ToBool(Dictionary<string, string> tags, string key)
        {
            bool value = default(bool);

            //we need to convert it to an int first before we can convert to a bool
            int value_int = ToGeneric<int>(tags, key);

            try
            {
                value = Convert.ToBoolean(value_int);
            }
            catch (Exception exception)
            {
                LibraryDebug.Error(LibraryDebugMethod.CONVERT, "tag", LibraryDebugError.NORMAL_EXCEPTION, TimeStamp.TimeLong);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
                LibraryDebug.PrintLine(nameof(value_int), value_int.ToString());
            }

            return value;
        }

        public static Color ToColor(Dictionary<string, string> tags, string key)
        {
            Color value = default(Color);

            string color_string = ToGeneric<string>(tags, key);

            try
            {
                value = ColorTranslator.FromHtml(color_string);
            }
            catch (Exception exception)
            {
                LibraryDebug.Error(LibraryDebugMethod.CONVERT, "tag", LibraryDebugError.NORMAL_EXCEPTION, TimeStamp.TimeLong);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
                LibraryDebug.PrintLine(nameof(color_string), color_string);
            }

            return value;
        }

        public static type ToEnum<type>(Dictionary<string, string> tags, string key) where type : struct, IConvertible
        {
            type value = default(type);

            if (!key.isValid())
            {
                return value;
            }

            string value_string = string.Empty;

            tags.TryGetValue(key, out value_string);

            if (!value_string.isValid())
            {
                return value;
            }

            if (!value_string.CanCovertTo<type>())
            {
                return value;
            }

            try
            {
                Enum.TryParse(value_string, out value);
            }
            catch (Exception exception)
            {
                LibraryDebug.Error(LibraryDebugMethod.CONVERT, "value", LibraryDebugError.NORMAL_EXCEPTION, TimeStamp.TimeLong);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
                LibraryDebug.PrintLine(nameof(value_string), value_string);
            }

            return value;
        }

        public static Dictionary<string, string> ToBadges(Dictionary<string, string> tags, string key)
        {
            Dictionary<string, string> badges = new Dictionary<string, string>();

            if (!key.isValid())
            {
                return badges;
            }

            //split the tag into each badge and it's version number            
            //badge/version,badge/verison...
            string[] badges_array = ToGeneric<string>(tags, key).StringToArray<string>(',');    //[0] badge/verison     [1] badge/version       [n] ...

            if (!badges_array.isValid())
            {
                return badges;
            }

            //split each badge into it's name and version number
            foreach(string element in badges_array)
            {
                string[] info = element.StringToArray<string>('/');                             //[0] badge             [1] version

                badges[info[0]] = info[1];
            }

            return badges;
        }

        public static MessageEmotes ToEmotes(Dictionary<string, string> tags, string key)
        {
            MessageEmotes emotes = new MessageEmotes();

            //get every unique emote with all ranges where the emote was used in the body of message
            //emote_id:start-end,start-end,.../emote_id:start-end,start-end,...
            string[] emotes_array = ToGeneric<string>(tags, key).StringToArray<string>('/');

            if (!emotes_array.isValid())
            {
                return emotes;
            }

            int count_total = 0;

            List<MessageEmote> emotes_list = new List<MessageEmote>();

            foreach (string emote_element in emotes_array)
            {
                List<MessageEmoteRange> ranges = new List<MessageEmoteRange>();

                string[] emote_data = emote_element.StringToArray<string>(':');     //[0] emote_id      [1] start-end,start-end,...
                string[] emote_ranges = emote_data[1].StringToArray<string>(',');   //[0] start-end     [1] start-end    [n] ....

                foreach (string range_element in emote_ranges)
                {
                    int[] emote_range = range_element.StringToArray<int>('-');

                    MessageEmoteRange range = new MessageEmoteRange                 //[0] start         [1] end
                    {
                        start = emote_range[0],
                        end = emote_range[1]
                    };

                    ranges.Add(range);

                    ++count_total;
                }

                MessageEmote emote = new MessageEmote
                {
                    count = ranges.Count,
                    emote_id = emote_data[0],
                    ranges = ranges.ToArray()
                };

                emotes_list.Add(emote);
            }

            emotes.emotes = emotes_list.ToArray();
            emotes.count_total = count_total;
            emotes.count_unique = emotes_list.Count;

            return emotes;
        }
    }
}
