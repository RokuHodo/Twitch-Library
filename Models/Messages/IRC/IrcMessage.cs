// standard namespaces
using System;
using System.Collections.Generic;

// project namespaces
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Enums.Extensions;
using TwitchLibrary.Extensions;

namespace TwitchLibrary.Models.Messages.IRC
{
    // [Obsolete("Class is depricated. ChatSharp.IrcMessage is now used")]
    public class IrcMessage
    {
        public bool contains_tags { get; private set; }
        public bool contains_prefix { get; private set; }

        public string[] middle { get; private set; }
        public string[] trailing { get; private set; }

        public string prefix { get; private set; }
        public string command { get; private set; }
        public string parameters { get; private set; }

        public Dictionary<string, string> tags { get; set; }                    

        public IrcMessage(string irc_message)
        {
            string irc_message_no_tags = string.Empty;

            tags = GetTags(irc_message, out irc_message_no_tags);
            prefix = GetPrefix(irc_message_no_tags);
            command = GetCommand(prefix, irc_message_no_tags);
            parameters = GetParameters(command, irc_message);
            middle = GetMiddle(parameters);
            trailing = GetTrailing(parameters);
        }

        #region Message parsing

        /// <summary>
        /// Searches for tags attached to the irc message and extracts any that exist and extracts them as a dictionary.
        /// </summary>
        private Dictionary<string, string> GetTags(string irc_message, out string irc_message_no_tags)
        {
            Dictionary<string, string> tags = new Dictionary<string, string>();

            // irc message only conmtains tags when it is preceeded with "@"
            if (!irc_message.StartsWith("@"))
            {
                contains_tags = false;

                irc_message_no_tags = irc_message;

                return tags;
            }

            contains_tags = true;

            // tags exist between "@" an the first space
            string tags_extracted = irc_message.TextBetween('@', ' ');

            // tags are delineated by ";"
            string[] tags_extracted_array = tags_extracted.StringToArray<string>(';'),
                     tags_array_temp;

            foreach (string tag in tags_extracted_array)
            {
                tags_array_temp = tag.StringToArray<string>('=');

                try
                {
                    // there should never be a situation where this fails, but just in case
                    tags[tags_array_temp[0]] = tags_array_temp[1];
                }
                catch (Exception exception)
                {
                    LibraryDebug.Error(LibraryDebugMethod.GET, "tag", LibraryDebugError.NORMAL_EXCEPTION);
                    LibraryDebug.PrintLineFormatted(nameof(exception), exception.Message);
                }
            }

            // cut of the tags to make handling the message later easier
            irc_message_no_tags = irc_message.TextAfter(" ");

            return tags;
        }

        /// <summary>
        /// Gets the prefix of the irc message. The irc message passed must have no tags attached.
        /// </summary>
        public string GetPrefix(string irc_message)
        {
            string _prefix = string.Empty;

            contains_prefix = irc_message.StartsWith(":");

            if (!contains_prefix)
            {
                return _prefix;
            }

            _prefix = irc_message.TextBetween(':', ' ');

            return _prefix;
        }

        /// <summary>
        /// Gets the irc message command. The irc message passed must have no tags attached.
        /// </summary>
        private string GetCommand(string _prefix, string irc_message)
        {
            // the prefix is optional, need to see if it exists to get the right command
            return contains_prefix ? irc_message.TextBetween(' ', ' ') : irc_message.TextBefore(' ');
        }

        /// <summary>
        /// Gets the parameters after the irc command and parses for the middle and trialing part of the message. The irc message passed must have no tags attached.
        /// </summary>
        private string GetParameters(string _command, string irc_message)
        {
            return irc_message.TextAfter(_command).RemovePadding(Padding.Left);            
        }

        private string[] GetMiddle(string parameters)
        {
            string[] _middle;

            if (parameters.IndexOf(":") != -1)
            {
                _middle = parameters.TextBefore(":").RemovePadding(Padding.Both).StringToArray<string>(' ');
            }
            else
            {
                _middle = parameters.StringToArray<string>(' ');
            }

            return _middle;
        }

        private string[] GetTrailing(string parameters)
        {
            string[] _trailing;

            if (parameters.IndexOf(":") != -1)
            {
                _trailing = parameters.TextAfter(":").RemovePadding(Padding.Both).StringToArray<string>(' ');
            }
            else
            {
                _trailing = new string[0];
            }

            return _trailing;
        }

        #endregion
    }
}