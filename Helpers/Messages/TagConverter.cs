using System;
using System.Collections.Generic;
using System.Drawing;

using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Extensions;
using TwitchLibrary.Models.Messages.Emotes;

namespace TwitchLibrary.Helpers.Messages
{
    public static class TagConverter
    {
        public static type ToGeneric<type>(Dictionary<string, string> tags, string key)
        {
            type value = default(type);            

            if (!key.isValidString())
            {
                return value;
            }

            string value_string;

            tags.TryGetValue(key, out value_string);

            if (!value_string.isValidString())
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

            if (!key.isValidString())
            {
                return value;
            }

            string value_string = string.Empty;

            tags.TryGetValue(key, out value_string);

            if (!value_string.isValidString())
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

        public static string[] ToBadges(Dictionary<string, string> tags, string key)
        {
            List<string> badges_list = new List<string>();

            if (!key.isValidString())
            {
                return badges_list.ToArray();
            }

            string[] badges_array = ToGeneric<string>(tags, key).StringToArray<string>(',');

            if (!badges_array.isValidArray())
            {
                return badges_list.ToArray();
            }

            foreach(string element in badges_array)
            {
                string badge = element.TextBefore("/");

                if (badge.isValidString())
                {
                    badges_list.Add(badge);
                }
            }

            return badges_list.ToArray();
        }

        public static MessageEmotes ToEmotes(Dictionary<string, string> tags, string key)
        {
            int count_total = 0;
            MessageEmotes message_emotes_master;

            List<MessageEmote> message_emotes_unique = new List<MessageEmote>();
            string[] emotes_array_unique = ToGeneric<string>(tags, "emotes").StringToArray<string>('/');    //get every unique emote with attached ranges

            if (!emotes_array_unique.isValidArray())
            {
                return new MessageEmotes();
            }

            foreach (string emote_element in emotes_array_unique)
            {
                List<MessageEmoteRange> ranges = new List<MessageEmoteRange>();

                string[] emote_data = emote_element.StringToArray<string>(':'),                             //split the emote into the ID and the ranges
                         emote_ranges = emote_data[1].StringToArray<string>(',');                           //get the ranges in the message where the emote was used

                foreach (string range_element in emote_ranges)
                {
                    int[] range_array = range_element.StringToArray<int>('-');

                    MessageEmoteRange range = new MessageEmoteRange
                    {
                        start = range_array[0],
                        end = range_array[1]
                    };

                    ranges.Add(range);

                    ++count_total;
                }

                MessageEmote emote = new MessageEmote
                {
                    id = Convert.ToInt32(emote_data[0]),
                    count = emote_ranges.Length,
                    ranges = ranges.ToArray()
                };

                message_emotes_unique.Add(emote);

            }

            message_emotes_master = new MessageEmotes
            {
                count = count_total,
                emotes = message_emotes_unique.ToArray()
            };

            return message_emotes_master;
        }
    }
}
