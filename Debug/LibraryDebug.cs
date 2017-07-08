// standard namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

// project namespaces
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Extensions;

namespace TwitchLibrary.Debug
{
    public static class LibraryDebug
    {       
        #region Success, error, and warning messages

        /// <summary>
        /// Prints a success debug message to the command line following a template with an optional time stamp.
        /// [ Success ] Successfully (method) the (obj)
        /// </summary>
        public static void Success(LibraryDebugMethod method, string obj, TimeStamp stamp = TimeStamp.None)
        {
            string text = "Successfully " + GetSuccessMethodString(method).ToLower() + " the " + obj;
            PrintLine(ConsoleColor.Green, stamp, text);
        }

        /// <summary>
        /// Gets the proper conjugation of the <see cref="LibraryDebugMethod"/> in the present tense.
        /// </summary>
        public static string GetSuccessMethodString(LibraryDebugMethod method)
        {
            string str = method.ToString();

            switch (method)
            {
                // anything that ends in a consonant 
                case LibraryDebugMethod.ADD:
                case LibraryDebugMethod.EDIT:
                case LibraryDebugMethod.LOAD:
                    {
                        str += "ed";
                    }
                    break;
                // anything that ends in a vowel
                case LibraryDebugMethod.PARSE:
                case LibraryDebugMethod.REMOVE:
                case LibraryDebugMethod.SERIALIZE:
                case LibraryDebugMethod.UPDATE:
                    {
                        str += "d";
                    }
                    break;
                // anything that ends in "y"
                case LibraryDebugMethod.MODIFY:
                case LibraryDebugMethod.APPLY:
                    {
                        str = str.TextBefore("y") + "ied";
                    }
                    break;
                // any special verbs
                case LibraryDebugMethod.GET:
                    {
                        str = "got";
                    }
                    break;
                default:
                    break;
            }

            return str;
        }

        /// <summary>
        /// Prints a custom success debug message to the command line with an optional time stamp.
        /// </summary>
        public static void Success(params string[] lines)
        {
            // string message = string.Format("[ {0,-7} ] " + text, "Success");
            PrintLine(ConsoleColor.Green, TimeStamp.None, lines);
        }

        /// <summary>
        /// Prints a custom success debug message to the command line with an optional time stamp.
        /// </summary>
        public static void Success(TimeStamp stamp, params string[] lines)
        {
            // string message = string.Format("[ {0,-7} ] " + text, "Success");
            PrintLine(ConsoleColor.Green, stamp, lines);
        }

        /// <summary>
        /// Prints a custom warning debug message to the command line with an optional time stamp.
        /// </summary>
        public static void Warning(params string[] lines)
        {
            // string message = string.Format("[ {0,-7} ] " + text, "Warning");
            PrintLine(ConsoleColor.Yellow, TimeStamp.None, lines);
        }

        /// <summary>
        /// Prints a custom warning debug message to the command line with an optional time stamp.
        /// </summary>
        public static void Warning(TimeStamp stamp, params string[] lines)
        {
            // string message = string.Format("[ {0,-7} ] " + text, "Warning");
            PrintLine(ConsoleColor.Yellow, stamp, lines);
        }

        /// <summary>
        /// Prints a error debug message to the command line following a template with an optional time stamp.
        /// [ Error ] Failed to (method) the (obj) : (error)
        /// </summary>
        public static void Error(LibraryDebugMethod method, string obj, string error, TimeStamp stamp = TimeStamp.None)
        {
            string text = "Failed to " + method.ToString().ToLower() + " the " + obj;
            if (error.isValid())
            {
                text += ": " + error;
            }
            // string message = string.Format("[ {0,-7} ] " + text, "Error");
            PrintLine(ConsoleColor.Red, stamp, text);
        }

        /// <summary>
        /// Prints a error debug message to the command line following a template with an optional time stamp.
        /// </summary>
        public static void Error(params string[] lines)
        {
            // string message = string.Format("[ {0,-7} ] " + lines.ToString(), "Error");
            PrintLine(ConsoleColor.Red, TimeStamp.None, lines);
        }

        /// <summary>
        /// Prints a error debug message to the command line following a template with an optional time stamp.
        /// </summary>
        public static void Error(TimeStamp stamp, params string[] lines)
        {
            // string message = string.Format("[ {0,-7} ] " + lines.ToString(), "Error");
            PrintLine(ConsoleColor.Red, stamp, lines);
        }

        #endregion

        #region Notifies and sub-headers

        /// <summary>
        /// Prints a notify debug message to the command line with an optional time stamp.
        /// </summary>
        public static void Notify(params string[] lines)
        {
            // string message = string.Format("[ {0,-7} ] " + text, "Notice");
            PrintLine(ConsoleColor.DarkCyan, TimeStamp.None, lines);
        }

        /// <summary>
        /// Prints a notify debug message to the command line with an optional time stamp.
        /// </summary>
        public static void Notify(TimeStamp stamp, params string[] lines)
        {
            // string message = string.Format("[ {0,-7} ] " + text, "Notice");
            PrintLine(ConsoleColor.DarkCyan, stamp, lines);
        }

        /// <summary>
        /// Prints text to the command line as cyan with an optional time stamp.
        /// </summary>        
        public static void Header(TimeStamp stamp, params string[] lines)
        {
            if (!lines.isValid())
            {
                return;
            }

            lines[0] = lines[0].Wrap("[ ", " ]");

            PrintLine(ConsoleColor.Cyan, stamp, lines);
        }

        /// <summary>
        /// Prints text to the command line as cyan with an optional time stamp.
        /// </summary>        
        public static void Header(params string[] lines)
        {
            Header(TimeStamp.None, lines);
        }

        #endregion

        #region Printing

        #region No carriage return

        /// <summary>
        /// Prints a line of text to the command line.
        /// </summary>
        public static void Print(params string[] words)
        {
            Print(ConsoleColor.Gray, TimeStamp.None, words);
        }

        /// <summary>
        /// Prints a line of text to the command line in a specified color.
        /// </summary>
        public static void Print(ConsoleColor color, params string[] words)
        {
            Print(color, TimeStamp.None, words);
        }

        /// <summary>
        /// Prints a line of text to the command line with a time stamp.
        /// </summary>
        public static void Print(TimeStamp stamp, params string[] words)
        {
            Print(ConsoleColor.Gray, stamp, words);
        }

        /// <summary>
        /// Prints a line of text to the command line in a specified color and with a time stamp.
        /// </summary>
        public static void Print(ConsoleColor color, TimeStamp stamp, params string[] words)
        {
            PrintBuilder(color, stamp, false, words);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label.
        /// </summary>
        public static void PrintFormatted(string label, string text)
        {
            string message = string.Format("{0,-20} {1,-20}", label, text);

            Print(message);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label in a specified color.
        /// </summary>
        public static void PrintFormatted(ConsoleColor color, string label, string text)
        {
            string message = string.Format("{0,-20} {1,-20}", label, text);

            Print(color, message);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label and a time stamp.
        /// </summary>
        public static void PrintFormatted(TimeStamp stamp, string label, string text)
        {
            string message = string.Format("{0,-20} {1,-20}", label, text);

            Print(stamp, message);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label in a specified color with a time stamp.
        /// </summary>
        public static void PrintFormatted(ConsoleColor color, TimeStamp stamp, string label, string text)
        {
            string message = string.Format("{0,-20} {1,-20}", label, text);

            Print(color, stamp, message);
        }

        #endregion

        #region Carriage return

        /// <summary>
        /// Prints a line of text to the command line.
        /// </summary>
        public static void PrintLine(params string[] lines)
        {
            PrintLine(ConsoleColor.Gray, TimeStamp.None, lines);
        }

        /// <summary>
        /// Prints a line of text to the command line in a specified color.
        /// </summary>
        public static void PrintLine(ConsoleColor color, params string[] lines)
        {
            PrintLine(color, TimeStamp.None, lines);
        }

        /// <summary>
        /// Prints a line of text to the command line with a time stamp.
        /// </summary>
        public static void PrintLine(TimeStamp stamp, params string[] lines)
        {
            PrintLine(ConsoleColor.Gray, stamp, lines);
        }

        /// <summary>
        /// Prints a lines of text to the command line in a specified color and with a time stamp.
        /// </summary>
        public static void PrintLine(ConsoleColor color, TimeStamp stamp, params string[] lines)
        {
            PrintBuilder(color, stamp, true, lines);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label.
        /// </summary>
        public static void PrintLineFormatted(string label, string text)
        {
            string message = string.Format("{0,-20} {1,-20}", label, text);

            PrintLine(message);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label in a specified color.
        /// </summary>
        public static void PrintLineFormatted(ConsoleColor color, string label, string text)
        {
            string message = string.Format("{0,-20} {1,-20}", label, text);

            PrintLine(color, message);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label and a time stamp.
        /// </summary>
        public static void PrintLineFormatted(TimeStamp stamp, string label, string text)
        {
            string message = string.Format("{0,-20} {1,-20}", label, text);

            PrintLine(stamp, message);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label in a specified color with a time stamp.
        /// </summary>
        public static void PrintLineFormatted(ConsoleColor color, TimeStamp stamp, string label, string text)
        {
            string message = string.Format("{0,-20} {1,-20}", label, text);

            PrintLine(color, stamp, message);
        }

        #endregion

        private static void PrintBuilder(ConsoleColor color, TimeStamp stamp, bool carriage_return, params string[] lines)
        {
            if (!lines.isValid())
            {
                return;
            }

            int primary_line_index = -1;
            string primary_line = string.Empty;

            for (int index = 0; index < lines.Length; ++index)
            {
                if (lines[index].isValid())
                {
                    primary_line = lines[index];
                    primary_line_index = index;

                    break;
                }
            }

            if (primary_line_index < 0)
            {
                return;
            }

            string time = GetTimeStampString(stamp);

            StringBuilder builder = new StringBuilder();
            builder.Append(time);
            if (carriage_return)
            {
                builder.AppendLine(primary_line);
            }
            else
            {
                builder.Append(primary_line);
            }

            if (primary_line_index < lines.Length - 1)
            {
                for (int index = ++primary_line_index; index < lines.Length; ++index)
                {
                    if (!lines[index].isValid())
                    {
                        continue;
                    }

                    builder.Append(' ', time.Length);

                    if (carriage_return)
                    {
                        builder.AppendLine("\t" +lines[index]);
                    }
                    else
                    {
                        builder.Append("\t " + lines[index]);
                    }

                }
            }

            Console.ForegroundColor = color;
            Console.Write(builder);
            Console.ResetColor();
        }

        private static string GetTimeStampString(TimeStamp stamp)
        {
            string time = "[ {0} ] ";

            switch (stamp)
            {
                case TimeStamp.None:
                    {
                        time = string.Empty;
                    }
                    break;
                case TimeStamp.DateLong:
                    {
                        time = string.Format(time, DateTime.Now.ToLongDateString());
                    }
                    break;
                case TimeStamp.DateShort:
                    {
                        time = string.Format(time, DateTime.Now.ToShortDateString());
                    }
                    break;
                case TimeStamp.TimeLong:
                    {
                        time = string.Format(time, DateTime.Now.ToLongTimeString());
                    }
                    break;
                case TimeStamp.TimeShort:
                    {
                        time = string.Format(time, DateTime.Now.ToShortTimeString());
                    }
                    break;
                case TimeStamp.Default:
                default:
                    {
                        time = string.Format(time, DateTime.Now.ToString());
                    }
                    break;
            }

            return time;
        }

        /// <summary>
        /// Prints a blank line to the command line.
        /// </summary>
        public static void BlankLine()
        {
            Console.WriteLine();
        }

        #endregion        

        #region Object dumping

        /// <summary>
        /// Prints all properties and fields of an object and all sub objects.
        /// </summary>        
        public static void PrintObject(object obj)
        {
            PrintObject(string.Empty, obj);
        }

        /// <summary>
        /// Prints all properties and fields of an object and all sub objects with a specified prefix.
        /// </summary>
        public static void PrintObject(string label, object obj)
        {
            if (obj == null || obj is ValueType || obj is DateTime || obj is string)
            {
                string value = GetObjectValueString(obj);
                PrintLineFormatted(label, value);
            }
            else if (obj is IEnumerable)
            {
                foreach (object element in (IEnumerable)obj)
                {
                    PrintObject(label, element);
                }
            }
            else
            {
                MemberInfo[] members = obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);

                foreach (MemberInfo member in members)
                {
                    FieldInfo field_info = member as FieldInfo;
                    PropertyInfo property_info = member as PropertyInfo;

                    if (field_info != null || property_info != null)
                    {
                        // get the type of the member
                        Type obj_member_type = field_info != null ? field_info.FieldType : property_info.PropertyType;

                        // get the value of the member as an object
                        object obj_member_value = field_info != null ? field_info.GetValue(obj) : property_info.GetValue(obj, null);
                        string obj_member_value_string = GetObjectValueString(obj_member_value);

                        // there is nothing to print, the object has sub properties/fields to print
                        if (obj_member_value_string == string.Empty)
                        {
                            BlankLine();
                            Header(member.Name);
                            PrintObject(string.Empty, obj_member_value);
                        }                        
                        else
                        {
                            PrintLineFormatted(member.Name, obj_member_value_string);
                        }                                              
                    }
                }
            }
        }

        private static string GetObjectValueString(object obj)
        {
            string value = string.Empty;

            // get the string version of the value to print
            if (obj is DateTime)
            {
                value = ((DateTime)obj).ToLongDateString();
            }
            else if (obj is ValueType || obj is string)
            {
                value = obj.ToString();
            }
            else if (obj == null)
            {
                value = "null";
            }

            return value;
        }

        #endregion

        #region Formatting

        public static string FormatAsColumns(string column_1, string column_2, int column_1_align = -20, int column_2_align = -20)
        {
            return string.Format("{0,"+column_1_align+"} {1,"+column_2_align+"}", column_1, column_2);
        }

        #endregion

    }
}