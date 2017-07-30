// standard namespaces
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

// project namespaces
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Enums.Messages;
using TwitchLibrary.Extensions;
using TwitchLibrary.Models.Messages.IRC.Tags;

namespace TwitchLibrary.Helpers.Messages
{
    public static class TagConverter
    {
        internal static type ToGeneric<type>(Dictionary<string, string> tags, string key)
        {
            type value = default(type);            

            if (!key.isValid() || !tags.ContainsKey(key))
            {
                return value;
            }

            tags.TryGetValue(key, out string tag_string);

            if (!tag_string.isValid())
            {
                return value;
            }

            if (!tag_string.CanCovertTo<type>())
            {
                return value;
            }

            try
            {
                value = (type)Convert.ChangeType(tag_string, typeof(type));             
            }
            catch(Exception exception)
            {
                Log.Error("Failed to convert tag to " + typeof(type).Name,
                                   Error.NORMAL_EXCEPTION,
                                   Log.FormatColumns(nameof(key), key),
                                   Log.FormatColumns(nameof(tag_string), tag_string),
                                   Log.FormatColumns(nameof(exception), exception.Message));
            }

            return value;
        }

        public static bool ToBool(Dictionary<string, string> tags, string key)
        {
            bool value = default(bool);

            // we need to convert it to an int first before we can convert to a bool
            int value_int = ToGeneric<int>(tags, key);

            try
            {
                value = Convert.ToBoolean(value_int);
            }
            catch (Exception exception)
            {
                Log.Error("Failed to convert tag to " + typeof(bool).Name,
                                   Error.NORMAL_EXCEPTION,
                                   Log.FormatColumns(nameof(key), key),
                                   Log.FormatColumns(nameof(value_int), value_int),
                                   Log.FormatColumns(nameof(exception), exception.Message));
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
                Log.Error("Failed to convert tag",
                                   Error.NORMAL_EXCEPTION,
                                   Log.FormatColumns(nameof(key), key),
                                   Log.FormatColumns(nameof(color_string), color_string),
                                   Log.FormatColumns(nameof(exception), exception.Message));
            }

            return value;
        }

        public static type ToEnum<type>(Dictionary<string, string> tags, string key) where type : struct, IConvertible
        {
            type value = default(type);

            if (!key.isValid() || !tags.ContainsKey(key))
            {
                return value;
            }

            string enum_value = string.Empty;

            tags.TryGetValue(key, out enum_value);

            if (!enum_value.isValid())
            {
                return value;
            }

            if (!enum_value.CanCovertTo<type>())
            {
                return value;
            }

            try
            {
                Enum.TryParse(enum_value, out value);
            }
            catch (Exception exception)
            {
                Log.Error("Failed to convert tag to " + typeof(type).Name,
                                   Error.NORMAL_EXCEPTION,
                                   Log.FormatColumns(nameof(key), key),
                                   Log.FormatColumns(nameof(enum_value), enum_value),
                                   Log.FormatColumns(nameof(exception), exception.Message));
            }

            return value;
        }

        public static UserType ToUserType(Dictionary<string, string> tags, string key, List<Badge> badges)
        {
            UserType user_type = ToEnum<UserType>(tags, key);
            foreach (Badge badge in badges)
            {
                if (badge.name == "broadcaster")
                {
                    user_type = UserType.broadcaster;
                }
            }

            return user_type;
        }

        public static List<Badge> ToBadges(Dictionary<string, string> tags, string key)
        {
            List<Badge> badges = new List<Badge>();

            if (!key.isValid() || !tags.ContainsKey(key))
            {
                return badges;
            }

            // split the tag into each badge and it's version number            
            // badge/version,badge/verison...
            string[] badges_array = ToGeneric<string>(tags, key).StringToArray<string>(',');    // [0] badge/verison     [1] badge/version       [n] ...

            if (!badges_array.isValid())
            {
                return badges;
            }

            // split each badge into it's name and version number
            foreach(string element in badges_array)
            {
                string[] info = element.StringToArray<string>('/');                             // [0] badge             [1] version

                badges.Add(new Badge
                {
                    name = info[0],
                    version = Convert.ToInt16(info[1])
                });
            }

            return badges;
        }

        public static List<Emote> ToEmotes(Dictionary<string, string> tags, string key)
        {
            List<Emote> emotes = new List<Emote>();

            // get every unique emote with all ranges where the emote was used in the body of message
            // emote_id:start-end,start-end,.../emote_id:start-end,start-end,...
            string[] emotes_array = ToGeneric<string>(tags, key).StringToArray<string>('/');

            if (!emotes_array.isValid())
            {
                return emotes;
            }

            foreach (string emote_element in emotes_array)
            {
                List<EmoteRange> ranges = new List<EmoteRange>();

                string[] emote_data = emote_element.StringToArray<string>(':');     // [0] emote_id      [1] start-end,start-end,...
                string[] emote_ranges = emote_data[1].StringToArray<string>(',');   // [0] start-end     [1] start-end    [n] ....

                foreach (string range_element in emote_ranges)
                {
                    int[] emote_range = range_element.StringToArray<int>('-');

                    ranges.Add(new EmoteRange                                       // [0] start         [1] end
                    {
                        start = emote_range[0],
                        end = emote_range[1]
                    });
                }

                emotes.Add(new Emote
                {
                    id = emote_data[0],
                    ranges = ranges
                });
            }

            return emotes;
        }

        public static List<type> ToList<type>(Dictionary<string, string> tags, string key, char separator = ',')
        {
            List<type> list = default(List<type>);

            if (!key.isValid() || !tags.ContainsKey(key))
            {
                return list;
            }

            list = tags[key].StringToArray<type>(separator).ToList();

            return list;
        }
    }
}
