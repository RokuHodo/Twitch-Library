//standard namespaces
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

//project namespaces
using TwitchLibrary.Debug;
using TwitchLibrary.Enums.Debug;
using TwitchLibrary.Enums.Extensions;

//imported .dll's
using Newtonsoft.Json;

namespace TwitchLibrary.Extensions
{
    public static class UniversalExtensions
    {
        #region Validity checks

        /// <summary>
        /// Verifies that an object is null.
        /// </summary>     
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isNull(this object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// Verifies that an array is not null and has at least one element.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isValid(this Array array)
        {
            return array != null && array.Length > 0;
        }

        /// <summary>
        /// Verifies that a list is not null and has at least one element.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isValid<type>(this List<type> list)
        {
            return !list.isNull() && list.Count > 0;
        }

        /// <summary>
        /// Verifies that a dictionary is not null and has at least one <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isValid<Tkey, TValue>(this Dictionary<Tkey, TValue> dictionary)
        {
            return !dictionary.isNull() && dictionary.Keys.Count > 0;
        }

        /// <summary>
        /// Verifies that a string is not null, empty, or contains only whitespace.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isValid(this string str)
        {
            return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Checks to see if an object can be convereted to certain type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanCovertTo<type>(this object obj)
        {
            return TypeDescriptor.GetConverter(typeof(type)).IsValid(obj);
        }

        #endregion

        #region Arithmetic operations

        /// <summary>
        /// Clamps the value to a minum with no maximum (inclusive).
        /// If the number is less than the minimum, the default value is returned.
        /// </summary>
        public static int ClampMin(this int value, int minimum, int default_value)
        {
            //in case the user is an idiot
            default_value.ClampMin(minimum);

            if (value < minimum)
            {
                value = default_value.isNull() ? minimum : default_value;
            }

            return value;
        }

        /// <summary>
        /// Clamps the value to a minum with no maximum (inclusive).
        /// </summary>
        public static int ClampMin(this int value, int minimum)
        {
            if (value < minimum)
            {
                value = minimum;
            }

            return value;
        }

        /// <summary>
        /// Clamps the value to a maximum with no minimum (inclusive).
        /// If the number is greater than the maximum, the default value is returned.
        /// </summary>
        public static int ClampMax(this int value, int maximum, int default_value)
        {
            //in case the user is an idiot
            default_value.ClampMin(maximum);

            if (value > maximum)
            {
                value = default_value.isNull() ? maximum : default_value;
            }

            return value;
        }

        /// <summary>
        /// Clamps the value to a maximum with no minimum (inclusive).
        /// </summary>
        public static int ClampMax(this int value, int maximum)
        {
            if (value > maximum)
            {
                value = maximum;
            }

            return value;
        }

        /// <summary>
        /// Clamps the value between a range of numbers (inclusive).
        /// If the number is out of range, the default value is returned.
        /// </summary>
        public static int Clamp(this int value, int minimum, int maximum, int default_value)
        {
            //in case the user is an idiot
            default_value.Clamp(minimum, maximum);

            value = value.ClampMin(minimum, default_value);
            value = value.ClampMax(maximum, default_value);

            return value;
        }

        /// <summary>
        /// Clamps the value between a range of numbers (inclusive).
        /// If the number is out of range, either the minimum or maximum will be returned depending on an overflow or underflow.
        /// </summary>
        public static int Clamp(this int value, int minimum, int maximum)
        {
            value = value.ClampMin(minimum);
            value = value.ClampMax(maximum);

            return value;
        }

        #endregion

        #region String parsing

        /// <summary>
        /// Gets the text after character.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="start">The character to search from.</param>
        /// <param name="offset_index">How far into the string to start searching for the start character.</param>
        /// <returns>
        /// If the start character could not be found, an empty string is returned.
        /// </returns>
        public static string TextAfter(this string str, char start, int offset_index = 0)
        {
            return str.TextAfter(start.ToString(), offset_index);
        }

        /// <summary>
        /// Gets the text after a sub string.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="start">The sub string to search from.</param>
        /// <param name="index_offset">How far into the string to start searching for the start sub string.</param>
        /// <returns>
        /// If the start sub string could not be found, an empty string is returned.
        /// </returns>
        public static string TextAfter(this string str, string start, int index_offset = 0)
        {
            string result = string.Empty;

            if (!str.isValid())
            {
                LibraryDebug.Error("Failed to find text after " + start.Wrap("\"", "\""), TimeStamp.TimeLong);
                LibraryDebug.PrintLine("String is empty, null, or whitespace");

                return result;
            }

            try
            {
                int index_start = str.IndexOf(start, index_offset);
                if (index_start < 0)
                {
                    LibraryDebug.Error("Failed to find text after " + start.Wrap("\"", "\""), TimeStamp.TimeLong);
                    LibraryDebug.PrintLine("Starting point " + start.Wrap("\"", "\"") + " could not be found");
                    LibraryDebug.PrintLineFormatted(nameof(str), str);

                    return result;
                }

                index_start += start.Length;
                result = str.Substring(index_start);
            }
            catch(Exception exception)
            {
                LibraryDebug.Error("Failed to find text after " + start.Wrap("\"", "\""), TimeStamp.TimeLong);
                LibraryDebug.PrintLineFormatted(nameof(str), str);
                LibraryDebug.PrintLineFormatted(nameof(index_offset), index_offset.ToString());
                LibraryDebug.PrintLineFormatted(nameof(exception), exception.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets the text before character.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="end">The sub string to search up to.</param>
        /// <param name="index_offset">How far into the string to start searching for the end sub string.</param>
        /// <returns>
        /// If the end character could not be found, an empty string is returned.
        /// </returns>
        public static string TextBefore(this string str, char end, int offset_index = 0)
        {
            return str.TextBefore(end.ToString(), offset_index);
        }

        /// <summary>
        /// Gets the text before a sub string.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="end">The sub string to search up to.</param>
        /// <param name="index_offset">How far into the string to start searching for the end sub string.</param>
        /// <returns>
        /// If the end sub string could not be found, an empty string is returned.
        /// </returns>
        public static string TextBefore(this string str, string end, int index_offset = 0)
        {
            string result = string.Empty;

            if (!str.isValid())
            {
                LibraryDebug.Error("Failed to find text before " + end.Wrap("\"", "\""), TimeStamp.TimeLong);
                LibraryDebug.PrintLine("String is empty, null, or whitespace");

                return result;
            }

            try
            {
                int index_end = str.IndexOf(end, index_offset);
                if (index_end < 0)
                {
                    LibraryDebug.Error("Failed to find text after " + end.Wrap("\"", "\""), TimeStamp.TimeLong);
                    LibraryDebug.PrintLine("Ending point " + end.Wrap("\"", "\"") + " could not be found");
                    LibraryDebug.PrintLineFormatted(nameof(str), str);

                    return result;
                }

                result = str.Substring(0, index_end);
            }
            catch (Exception exception)
            {
                LibraryDebug.Error("Failed to find text after " + end.Wrap("\"", "\""), TimeStamp.TimeLong);
                LibraryDebug.PrintLineFormatted(nameof(str), str);
                LibraryDebug.PrintLineFormatted(nameof(index_offset), index_offset.ToString());
                LibraryDebug.PrintLineFormatted(nameof(exception), exception.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets the text between two characters.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="start">The first charcater to search from.</param>
        /// <param name="end">The character to search up to after the start character.</param>
        /// <param name="offset_index">How far into the string to start searching for the start character.</param>
        /// <returns>
        /// If no sub string could be found between the start and end characters, an empty string is returned.
        /// </returns>
        public static string TextBetween(this string str, char start, char end, int offset_index = 0)
        {
            return str.TextBetween(start.ToString(), end.ToString(), offset_index);
        }

        /// <summary>
        /// Gets the text between two sub strings.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="start">The sub string to search from.</param>
        /// <param name="end">The sub string to search up to after the start sub string.</param>
        /// <param name="index_offset">How far into the string to start searching for the start sub string.</param>
        /// <returns>
        /// If no sub string could be found between the start and end sub strings, an empty string is returned.
        /// </returns>
        public static string TextBetween(this string str, string start, string end, int index_offset = 0)
        {
            string result = string.Empty;

            if (!str.isValid())
            {
                LibraryDebug.Error("Failed to find text between " + start.Wrap("\"", "\"") + " and " + end.Wrap("\"", "\""), TimeStamp.TimeLong);
                LibraryDebug.PrintLine("String is empty, null, or whitespace");

                return result;
            }

            try
            {
                int index_start = str.IndexOf(start, index_offset);
                int index_end = str.IndexOf(end, index_start + start.Length);
                if (index_start < 0 || index_end < 0)
                {
                    LibraryDebug.Error("Failed to find text between " + start.Wrap("\"", "\"") + " and " + end.Wrap("\"", "\""), TimeStamp.TimeLong);

                    if (index_start < 0)
                    {
                        LibraryDebug.PrintLine("Starting point " + start.Wrap("\"", "\"") + " could not be found", TimeStamp.TimeLong);
                    }

                    if (index_end < 0)
                    {
                        LibraryDebug.PrintLine("Ending point " + end.Wrap("\"", "\"") + " could not be found", TimeStamp.TimeLong);
                    }

                    return result;
                }
            
                index_start += start.Length;
                result = str.Substring(index_start, index_end - index_start);
            }
            catch(Exception exception)
            {
                LibraryDebug.Error("Failed to find text between " + start.Wrap("\"", "\"") + " and " + end.Wrap("\"", "\""), TimeStamp.TimeLong);
                LibraryDebug.PrintLineFormatted(nameof(str), str);
                LibraryDebug.PrintLineFormatted(nameof(index_offset), index_offset.ToString());
                LibraryDebug.PrintLineFormatted(nameof(exception), exception.Message);
            }

            return result;
        }

        /// <summary>
        /// Converts a string into an array of a specified type.
        /// Whitespace lines are ignored and not added to the array.
        /// If no elements of the string could be converted, a default array of the specified type is returned. 
        /// </summary>
        public static type[] StringToArray<type>(this string str, char separator)
        {
            if (!str.isValid())
            {
                return default(type[]);
            }

            List<type> result = new List<type>();

            string[] array = str.Split(separator);
            foreach (string element in array)
            {
                if (!element.CanCovertTo<type>())
                {
                    LibraryDebug.Error("Could not convert " + element.Wrap("\"", "\"") + " from " + typeof(string).Name + " to " + typeof(string).Name, TimeStamp.TimeLong);
                    continue;
                }

                result.Add((type)Convert.ChangeType(element, typeof(type)));
            }

            return result.isValid() ? result.ToArray() : default(type[]);
        }

        #endregion

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
                            if (str[index].ToString().isValid())
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
                            if (str[index - 1].ToString().isValid())
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
                LibraryDebug.PrintLineFormatted(nameof(exception), exception.Message);
                LibraryDebug.PrintLineFormatted(nameof(str), str);
            }

            return value;
        }

        public static async Task<string> TrySerializeObjectAsync<type>(this type obj)
        {
            Task<string> task = Task.Run(() =>
            {
                try
                {
                    return JsonConvert.SerializeObject(obj);
                }
                catch (Exception exception)
                {
                    LibraryDebug.Error(LibraryDebugMethod.SERIALIZE, typeof(type).Name, LibraryDebugError.NORMAL_EXCEPTION);
                    LibraryDebug.PrintLine(nameof(exception), exception.Message);

                    return "";
                }
            });

            string result = await task;

            if (task.IsFaulted)
            {
                LibraryDebug.Error(LibraryDebugMethod.SERIALIZE, typeof(type).Name, LibraryDebugError.NORMAL_FAULTED);
                LibraryDebug.PrintLine(nameof(task.Exception), task.Exception.Message);

                result = "";
            }

            return result;
        }

        public static async Task<type> TryDeserializeObjectAsync<type>(this string str)
        {
            Task<type> task = Task.Run(() =>
            {
                try
                {
                    return JsonConvert.DeserializeObject<type>(str);
                }
                catch (Exception exception)
                {
                    LibraryDebug.Error(LibraryDebugMethod.SERIALIZE, typeof(type).Name, LibraryDebugError.NORMAL_EXCEPTION);
                    LibraryDebug.PrintLine(nameof(exception), exception.Message);

                    return default(type);
                }
            });

            type result = await task;

            if (task.IsFaulted)
            {
                LibraryDebug.Error(LibraryDebugMethod.SERIALIZE, typeof(type).Name, LibraryDebugError.NORMAL_FAULTED);
                LibraryDebug.PrintLine(nameof(task.Exception), task.Exception.Message);

                result = default(type);
            }

            return result;
        }
    }
}
