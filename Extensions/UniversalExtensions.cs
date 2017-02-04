using System;
using System.Collections.Generic;
using System.ComponentModel;

//project namespaces
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Enums.Extensions;

namespace TwitchLibrary.Extensions
{
    public static class UniversalExtensions
    {
        /// <summary>
        /// Checks to see if an object is null.
        /// </summary>        
        public static bool isNull(this object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// Checks to see if an array is initialized and has at least one element.
        /// </summary>
        public static bool isValidArray(this Array array)
        {
            return array != null && array.Length > 0;
        }

        /// <summary>
        /// Checks to see if a list is initialized and has at least one element.
        /// </summary>
        public static bool isValidList<type>(this List<type> list)
        {
            return !list.isNull() && list.Count > 0;
        }

        /// <summary>
        /// Checks to see if a dictionary is initialized and has at least one element.
        /// </summary>
        public static bool isValidDictionary<Tkey, TValue>(this Dictionary<Tkey, TValue> dictionary)
        {
            return !dictionary.isNull() && dictionary.Keys.Count > 0;
        }

        /// <summary>
        /// Checks to see if a string is null, empty, or contains only whitespace.
        /// </summary>
        public static bool isValidString(this string str)
        {
            return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
        }
        /// <summary>
        /// Checks to see if an object can be convereted to certain type.
        /// </summary>
        public static bool CanCovertTo<type>(this object obj)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(type));

            return converter.IsValid(obj);
        }

        /// <summary>
        /// Clamps the value between a range of numbers.
        /// If the number is out of range, the default value is returned.
        /// </summary>
        public static int Clamp(this int value, int minimum, int maximum, int default_value)
        {
            //in case the user is an idiot
            default_value.Clamp(minimum, maximum);

            if(value < minimum)
            {
                value = default_value.isNull() ? minimum : default_value;
            }
            else if (value > maximum)
            {
                value = default_value.isNull() ? maximum : default_value;
            }

            return value;
        }

        /// <summary>
        /// Clamps the value between a range of numbers.
        /// If the number is out of range, either the minimum or maximum will be returned depending on an overflow or underflow.
        /// </summary>
        public static int Clamp(this int value, int minimum, int maximum)
        {
            if (value < minimum)
            {
                value = minimum;
            }
            else if (value > maximum)
            {
                value = maximum;
            }

            return value;
        }

        /// <summary>
        /// Gets the text after a certain part of a string.
        /// Returns <see cref="string.Empty"/> if the string index cannot be found.
        /// </summary>
        public static string TextAfter(this string str, string find)
        {
            string result = string.Empty;

            int index = str.IndexOf(find);

            if (index != -1)
            {
                index += find.Length;

                result = str.Substring(index);
            }

            return result;
        }

        /// <summary>
        /// Gets the text before a certain part of a string.
        /// Returns <see cref="string.Empty"/> if the string index cannot be found.
        /// </summary>
        public static string TextBefore(this string str, string find)
        {
            string result = string.Empty;

            int index = str.IndexOf(find);

            if (index != -1)
            {
                result = str.Substring(0, index);
            }

            return result;
        }

        /// <summary>
        /// Gets the text between two characters at the first occurance.
        /// The starting index can be specified.
        /// The offset specifies how far into the sub string to return.
        /// </summary>
        public static string TextBetween(this string str, char start, char end, int starting_index = 0, int offset = 0)
        {
            string result = "";

            int parse_start, parse_end;

            parse_start = str.IndexOf(start, starting_index) + 1;
            parse_end = str.IndexOf(end, parse_start);

            try
            {
                result = str.Substring(parse_start + offset, parse_end - parse_start - offset);
            }
            catch (Exception exception)
            {

                LibraryDebug.Error("Failed to find text between " + start.Wrap("\"", "\"") + " and " + end.Wrap("\"", "\""));

                LibraryDebug.PrintLine(nameof(str), str);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
            }

            return result;
        }

        /// <summary>
        /// Wraps a string with the specified strings.
        /// </summary>
        public static string Wrap(this string str, string prefix, string suffix)
        {
            if (!str.StartsWith(prefix))
            {
                str = prefix + str;
            }

            if (!str.EndsWith(suffix))
            {
                str += suffix;
            }

            return str;
        }

        /// <summary>
        /// Wraps a character with the specified strings.
        /// </summary>
        public static string Wrap(this string str, char prefix, char suffix)
        {
            return prefix + str + suffix;
        }

        /// <summary>
        /// Wraps a character with the specified strings.
        /// </summary>
        public static string Wrap(this char character, string start, string end)
        {
            return start + character + end;
        }

        /// <summary>
        /// Converts a string into an array of a specified type.
        /// Whitespace lines are ignored and not added to the array.
        /// If no elements of the string could be converted, a default array of the specified type is returned. 
        /// </summary>
        public static type[] StringToArray<type>(this string str, char parse_point)
        {
            if (!str.isValidString())
            {
                return default(type[]);
            }

            string[] array = str.Split(parse_point);

            List<type> result = new List<type>();

            int index = 0;
            foreach (string element in array)
            {
                if (!element.CanCovertTo<type>())
                {
                    continue;
                }

                try
                {
                    result.Add((type)Convert.ChangeType(element, typeof(type)));
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Could not convert array element " + element.Wrap("\"", "\"") + " from " + typeof(string).Name.Wrap("\"", "\"") + " to " + typeof(type).Name.Wrap("\"", "\"") + " at index \"" + index + "\"");
                    Console.ResetColor();

                    Console.WriteLine("{0,-20} {1,-20}", nameof(exception), exception.Message);
                }

                ++index;
            }

            return result.Count == 0 ? default(type[]) : result.ToArray();
        }

        /// <summary>
        /// Removes padding from the left, right, both sides of a string.
        /// </summary>
        public static string RemovePadding(this string str, Padding string_side = Padding.Both)
        {
            string result = string.Empty;

            switch (string_side)
            {
                case Padding.Left:
                    {
                        for (int index = 0; index < str.Length; index++)
                        {
                            if (str[index].ToString().isValidString())
                            {
                                result = str.Substring(index);

                                break;
                            }
                        }
                    }
                    break;
                case Padding.Right:
                    {
                        for (int index = str.Length; index > 0; index--)
                        {
                            if (str[index - 1].ToString().isValidString())
                            {
                                result = str.Substring(0, index);

                                break;
                            }
                        }
                    }
                    break;
                case Padding.Both:
                    {
                        result = str.RemovePadding(Padding.Left);
                        result = result.RemovePadding(Padding.Right);
                    }
                    break;
                default:
                    {
                        result = str.RemovePadding(Padding.Both);
                    }
                    break;
            }

            return result;
        }

        public static type ConvertToEnum<type>(this string str) where type : struct
        {
            type value = default(type);

            if (!str.CanCovertTo<type>())
            {
                return value;                
            }            

            try
            {
                Enum.TryParse(str, out value);
            }
            catch(Exception exception)
            {
                LibraryDebug.Error(LibraryDebugMethod.CONVERT, "str", LibraryDebugError.NORMAL_EXCEPTION);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
                LibraryDebug.PrintLine(nameof(str), str);
            }

            return value;
        }
    }
}
