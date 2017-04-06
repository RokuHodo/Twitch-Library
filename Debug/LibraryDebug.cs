//standard namespaces
using System;
using System.Collections;
using System.Reflection;

//project namespaces
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
            string message = "Successfully " + GetSuccessMethodString(method).ToLower() + " the " + obj;

            PrintLine(message, ConsoleColor.Green, stamp, "Success");
        }

        /// <summary>
        /// Gets the proper conjugation of the <see cref="LibraryDebugMethod"/> in the present tense.
        /// </summary>
        public static string GetSuccessMethodString(LibraryDebugMethod method)
        {
            string str = method.ToString();

            switch (method)
            {
                //anything that ends in a consonant 
                case LibraryDebugMethod.ADD:
                case LibraryDebugMethod.EDIT:
                case LibraryDebugMethod.LOAD:
                    {
                        str += "ed";
                    }
                    break;
                //anything that ends in a vowel
                case LibraryDebugMethod.PARSE:
                case LibraryDebugMethod.REMOVE:
                case LibraryDebugMethod.SERIALIZE:
                case LibraryDebugMethod.UPDATE:
                    {
                        str += "d";
                    }
                    break;
                //anything that ends in "y"
                case LibraryDebugMethod.MODIFY:
                case LibraryDebugMethod.APPLY:
                    {
                        str = str.TextBefore("y") + "ied";
                    }
                    break;
                //any special verbs
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
        /// [ Success ] (message)
        /// </summary>
        public static void Success(string message, TimeStamp stamp = TimeStamp.None)
        {
            if (!message.isValid())
            {
                return;
            }

            PrintLine(message, ConsoleColor.Green, stamp, "Success");
        }

        /// <summary>
        /// Prints a custom warning debug message to the command line with an optional time stamp.
        /// [ Warning ] (message)
        /// </summary>
        public static void Warning(string message, TimeStamp stamp = TimeStamp.None)
        {
            if (!message.isValid())
            {
                return;
            }

            PrintLine(message, ConsoleColor.Yellow, stamp, "Warning");
        }

        /// <summary>
        /// Prints a error debug message to the command line following a template with an optional time stamp.
        /// [ Error ] Failed to (method) the (obj) : (error)
        /// </summary>
        public static void Error(LibraryDebugMethod method, string obj, string error, TimeStamp stamp = TimeStamp.None)
        {
            string message = "Failed to " + method.ToString().ToLower() + " the " + obj;

            if(error.isValid())
            {
                message += ": " + error;
            }

            PrintLine(message, ConsoleColor.Red, stamp, "Error");
        }

        /// <summary>
        /// Prints a custom error debug message to the command line with an optional time stamp.
        /// [ Error ] Failed to (method) the (obj) : (error)
        /// </summary>
        public static void Error(string message, TimeStamp stamp = TimeStamp.None)
        {
            if (!message.isValid())
            {
                return;

            }

            PrintLine(message, ConsoleColor.Red, stamp, "Error");
        }

        #endregion

        #region Notifies and sub-headers

        /// <summary>
        /// Prints a notify debug message to the command line with an optional time stamp.
        /// [ Notice ] (message)
        /// </summary>
        public static void Notify(string message, TimeStamp stamp = TimeStamp.None)
        {
            if (!message.isValid())
            {
                return;
            }

            PrintLine(message, ConsoleColor.Cyan, stamp, "Notice");
        }

        /// <summary>
        /// Prints text to the command line as dark cyan with an optional time stamp.
        /// </summary>        
        public static void Header(string header, TimeStamp stamp = TimeStamp.None)
        {
            PrintLine(header, ConsoleColor.DarkCyan, stamp);
        }

        #endregion

        #region Printing

        #region No carriage return

        /// <summary>
        /// Prints a line of text to the command line.
        /// </summary>
        public static void Print(string text, string prefix = "")
        {
            PrintFinal(prefix, text, ConsoleColor.Gray);
        }

        /// <summary>
        /// Prints a line of text to the command line in a specified color.
        /// </summary>
        public static void Print(string text, ConsoleColor color, string prefix = "")
        {
            PrintFinal(prefix, text, color);
        }

        /// <summary>
        /// Prints a line of text to the command line with a time stamp.
        /// </summary>
        public static void Print(string text, TimeStamp stamp, string prefix = "")
        {
            PrintFinal(prefix, text, ConsoleColor.Gray, stamp);
        }

        /// <summary>
        /// Prints a line of text to the command line in a specified color and with a time stamp.
        /// </summary>
        public static void Print(string text, ConsoleColor color, TimeStamp stamp, string prefix = "")
        {
            PrintFinal(prefix, text, color, stamp);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label.
        /// </summary>
        public static void PrintFormatted(string label, string text)
        {
            if (!text.isValid())
            {
                return;
            }

            string message = string.Format("{0,-20} {1,-20}", label, text);

            Print(message);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label in a specified color.
        /// </summary>
        public static void PrintFormatted(string label, string text, ConsoleColor color)
        {
            if (!text.isValid())
            {
                return;
            }

            string message = string.Format("{0,-20} {1,-20}", label, text);

            Print(message, color);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label and a time stamp.
        /// </summary>
        public static void PrintFormatted(string label, string text, TimeStamp stamp)
        {
            if (!text.isValid())
            {
                return;
            }

            string message = string.Format("{0,-20} {1,-20}", label, text);

            Print(message, stamp);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label in a specified color with a time stamp.
        /// </summary>
        public static void PrintFormatted(string label, string text, ConsoleColor color, TimeStamp stamp)
        {
            if (!text.isValid())
            {
                return;
            }

            string message = string.Format("{0,-20} {1,-20}", label, text);

            Print(message, color, stamp);
        }

        #endregion

        #region Carriage return

        /// <summary>
        /// Prints a line of text to the command line.
        /// </summary>
        public static void PrintLine(string text, string prefix = "")
        {
            PrintFinal(prefix, text, ConsoleColor.Gray, TimeStamp.None, true);
        }

        /// <summary>
        /// Prints a line of text to the command line in a specified color.
        /// </summary>
        public static void PrintLine(string text, ConsoleColor color, string prefix = "")
        {
            PrintFinal(prefix, text, color, TimeStamp.None, true);
        }

        /// <summary>
        /// Prints a line of text to the command line with a time stamp.
        /// </summary>
        public static void PrintLine(string text, TimeStamp stamp, string prefix = "")
        {
            PrintFinal(prefix, text, ConsoleColor.Gray, stamp, true);
        }

        /// <summary>
        /// Prints a line of text to the command line in a specified color and with a time stamp.
        /// </summary>
        public static void PrintLine(string text, ConsoleColor color, TimeStamp stamp, string prefix = "")
        {
            PrintFinal(prefix, text, color, stamp, true);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label.
        /// </summary>
        public static void PrintLineFormatted(string label, string text)
        {
            if (!text.isValid())
            {
                return;
            }

            string message = string.Format("{0,-20} {1,-20}", label, text);

            PrintLine(message);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label in a specified color.
        /// </summary>
        public static void PrintLineFormatted(string label, string text, ConsoleColor color)
        {
            if (!text.isValid())
            {
                return;
            }

            string message = string.Format("{0,-20} {1,-20}", label, text);

            PrintLine(message, color);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label and a time stamp.
        /// </summary>
        public static void PrintLineFormatted(string label, string text, TimeStamp stamp)
        {
            if (!text.isValid())
            {
                return;
            }

            string message = string.Format("{0,-20} {1,-20}", label, text);

            PrintLine(message, stamp);
        }

        /// <summary>
        /// Prints a line of text to the command line preceeded with a label in a specified color with a time stamp.
        /// </summary>
        public static void PrintLineFormatted(string label, string text, ConsoleColor color, TimeStamp stamp)
        {
            if (!text.isValid())
            {
                return;
            }

            string message = string.Format("{0,-20} {1,-20}", label, text);

            PrintLine(message, color, stamp);
        }

        #endregion

        /// <summary>
        /// Prints a line of text to the command line with a specified color and if it should return to a new line after printing.
        /// </summary>
        private static void PrintFinal(string prefix, string text, ConsoleColor color = ConsoleColor.Gray, TimeStamp stamp = TimeStamp.None, bool carriage_return = false)
        {
            if (!text.isValid())
            {
                return;
            }

            string time = "[ {0} ]";

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

            string message = text;

            if (prefix.isValid())
            {
                message = "[ " + prefix + " ] " + message;
            }

            if (time.isValid())
            {
                message = time + message;
            }

            Console.ForegroundColor = color;

            if (carriage_return)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message);
            }
            
            Console.ResetColor();
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
                        //get the type of the member
                        Type obj_member_type = field_info != null ? field_info.FieldType : property_info.PropertyType;

                        //get the value of the member as an object
                        object obj_member_value = field_info != null ? field_info.GetValue(obj) : property_info.GetValue(obj, null);
                        string obj_member_value_string = GetObjectValueString(obj_member_value);

                        //there is nothing to print, the object has sub properties/fields to print
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

            //get the string version of the value to print
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
    }
}