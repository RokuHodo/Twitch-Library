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
    internal static class UniversalExtensions
    {
        #region Validity checks

        /// <summary>
        /// Verifies that an object is null.
        /// </summary>  
        /// <param name="obj">The object to be checked.</param>
        /// <returns>
        /// Returns true is the object is null.
        /// Returns false otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isNull(this object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// Verifies that an <see cref="Array"/> is not null and has a length of at leats 1.
        /// </summary>
        /// <param name="array">The <see cref="Array"/> to be validated.</param>
        /// <returns>
        /// Returns true if the <see cref="Array"/> is not null and has a length of at least 1.
        /// Returns false otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isValid(this Array array)
        {
            return array != null && array.Length > 0;
        }

        /// <summary>
        /// Verifies that a <see cref="List{T}"/> is not null and has a length of at leats 1.
        /// </summary>
        /// <typeparam name="type">The type of the list.</typeparam>
        /// <param name="list">The <see cref="List{T}"/> to be validated.</param>
        /// <returns>
        /// Returns true if the <see cref="List{T}"/> is not null and has a length of at least 1.
        /// Returns false otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isValid<type>(this List<type> list)
        {
            return !list.isNull() && list.Count > 0;
        }

        /// <summary>
        /// Verifies that a <see cref="Dictionary{TKey, TValue}"/> is not null and has at least one <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="Tkey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The <see cref="Dictionary{TKey, TValue}"/> to be validated.</param>
        /// <returns>
        /// Returns true if the <see cref="Dictionary{TKey, TValue}"/> is not null and has at least one <see cref="KeyValuePair{TKey, TValue}"/>.
        /// Returns false otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isValid<Tkey, TValue>(this Dictionary<Tkey, TValue> dictionary)
        {
            return !dictionary.isNull() && dictionary.Keys.Count > 0;
        }

        /// <summary>
        /// Verifies that a string is not null, empty, or contains only whitespace.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to be validated.</param>
        /// <returns>
        /// Returns true if the string is not not, not whitespace, and not empty.
        /// Returns false otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isValid(this string str)
        {
            return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Checks to see if an object can be convereted to certain type.
        /// </summary>
        /// <typeparam name="type">The type the object the object will be converted to.</typeparam>
        /// <param name="obj">The <see cref="object"/> to be checked.</param>
        /// <returns>
        /// Retuens true if the object can be converted to the specified type.
        /// Returns false otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanCovertTo<type>(this object obj)
        {
            return TypeDescriptor.GetConverter(typeof(type)).IsValid(obj);
        }

        #endregion

        #region Arithmetic operations

        /// <summary>
        /// Clamps the value of an <see cref="int"/> to a minimum value.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to be clamped.</param>
        /// <param name="minimum">The lowest value the <see cref="int"/> can be.</param>
        /// <returns>
        /// Returns the minimum value if the value of the <see cref="int"/> is lower than the minumum.
        /// Returns the value of the <see cref="int"/> otherise.
        /// </returns>
        public static int ClampMin(this int value, int minimum)
        {
            if (value < minimum)
            {
                value = minimum;
            }

            return value;
        }

        /// <summary>
        /// Clamps the value of an <see cref="int"/> to a minimum value.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to be clamped.</param>
        /// <param name="minimum">The lowest value the <see cref="int"/> can be.</param>
        /// <param name="default_value">
        /// The value that will be returned if the the <see cref="int"/> is less than the minimum value.
        /// The default value is clamped against the minimum to verify that the default value is not less than the minimum.
        /// </param>
        /// <returns>
        /// Returns the default value if the value of the <see cref="int"/> is lower than the minumum.
        /// Returns the value of the <see cref="int"/> otherise.
        /// </returns>
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
        /// Clamps the value of an <see cref="int"/> to a maximum value.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to be clamped.</param>
        /// <param name="maximum">The highest value the <see cref="int"/> can be.</param>
        /// <returns>
        /// Returns the maximum value if the value of the <see cref="int"/> is greater than the maximum.
        /// Returns the value of the <see cref="int"/> otherise.
        /// </returns>
        public static int ClampMax(this int value, int maximum)
        {
            if (value > maximum)
            {
                value = maximum;
            }

            return value;
        }

        /// <summary>
        /// Clamps the value of an <see cref="int"/> to a maximum value.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to be clamped.</param>
        /// <param name="maximum">The highest value the <see cref="int"/> can be.</param>
        /// <param name="default_value">
        /// The value that will be returned if the the <see cref="int"/> is greater than the maximum value.
        /// The default value is clamped against the maximum to verify that the default value is not greater than the maximum.
        /// </param>
        /// <returns>
        /// Returns the default value if the value of the <see cref="int"/> is greater than the maximum.
        /// Returns the value of the <see cref="int"/> otherise.
        /// </returns>
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
        /// Clamps the value of an <see cref="int"/> to a minimum and maximum value.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to be clamped.</param>
        /// <param name="minimum">The lowest value the <see cref="int"/> can be.</param>
        /// <param name="maximum">The highest value the <see cref="int"/> can be.</param>
        /// <returns>
        /// Returns the minimum value if the value of the <see cref="int"/> is less than the minimum.
        /// Returns the maximum value if the value of the <see cref="int"/> is greater than the maximum.
        /// Returns the value of the <see cref="int"/> otherise.
        /// </returns>
        public static int Clamp(this int value, int minimum, int maximum)
        {
            value = value.ClampMin(minimum);
            value = value.ClampMax(maximum);

            return value;
        }

        /// <summary>
        /// Clamps the value of an <see cref="int"/> to a minimum and maximum value.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to be clamped.</param>
        /// <param name="minimum">The lowest value the <see cref="int"/> can be.</param>
        /// <param name="maximum">The highest value the <see cref="int"/> can be.</param>
        /// <param name="default_value">
        /// The value that will be returned if the the <see cref="int"/> is out of range.
        /// The default value is clamped against the minimum and maximum to verify that the default value within the valid range.
        /// </param>
        /// <returns>
        /// Returns the default value if the value of the <see cref="int"/> is out of range.
        /// Returns the value of the <see cref="int"/> otherise.
        /// </returns>
        public static int Clamp(this int value, int minimum, int maximum, int default_value)
        {
            //in case the user is an idiot
            default_value.Clamp(minimum, maximum);

            value = value.ClampMin(minimum, default_value);
            value = value.ClampMax(maximum, default_value);

            return value;
        }

        #endregion

        #region String parsing

        /// <summary>
        /// Gets the text after a <see cref="char"/> in a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to be parsed.</param>
        /// <param name="start">The <see cref="char"/> to search from.</param>
        /// <param name="offset_index">How far into the <see cref="string"/> to start searching for the start <see cref="char"/>.</param>
        /// <returns>
        /// Returns an empty string if the start <see cref="char"/> could not be found or if the offset index was out of bounds.
        /// Returns the text after the start <see cref="char"/> otherwise.
        /// </returns>
        public static string TextAfter(this string str, char start, int offset_index = 0)
        {
            return str.TextAfter(start.ToString(), offset_index);
        }

        /// <summary>
        /// Gets the text after a sub string in a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to be parsed.</param>
        /// <param name="start">The sub <see cref="string"/> to search from.</param>
        /// <param name="offset_index">How far into the <see cref="string"/> to start searching for the start sub <see cref="string"/>.</param>
        /// <returns>
        /// Returns an empty string if the start sub <see cref="string"/> could not be found or if the offset index was out of bounds.
        /// Returns the text after the start sub <see cref="char"/> otherwise.
        /// </returns>
        public static string TextAfter(this string str, string start, int offset_index = 0)
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
                int index_start = str.IndexOf(start, offset_index);
                if (index_start < 0)
                {
                    LibraryDebug.Error("Failed to find text after " + start.Wrap("\"", "\""), TimeStamp.TimeLong);
                    LibraryDebug.PrintLine("Starting point " + start.Wrap("\"", "\"") + " could not be found");
                    LibraryDebug.PrintLine(nameof(str), str);

                    return result;
                }

                index_start += start.Length;
                result = str.Substring(index_start);
            }
            catch(Exception exception)
            {
                LibraryDebug.Error("Failed to find text after " + start.Wrap("\"", "\""), TimeStamp.TimeLong);
                LibraryDebug.PrintLine(nameof(str), str);
                LibraryDebug.PrintLine(nameof(offset_index), offset_index.ToString());
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets the text before character.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to be parsed.</param>
        /// <param name="end">The <see cref="char"/> to search up to.</param>
        /// <param name="offset_index">How far into the <see cref="string"/> to start searching for the end <see cref="char"/>.</param>
        /// <returns>
        /// Returns an empty string if the end <see cref="char"/> could not be found or if the offset index was out of bounds.
        /// Returns the text before the end <see cref="char"/> otherwise.
        /// </returns>
        public static string TextBefore(this string str, char end, int offset_index = 0)
        {
            return str.TextBefore(end.ToString(), offset_index);
        }

        /// <summary>
        /// Gets the text before a <see cref="char"/> in a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to be parsed.</param>
        /// <param name="end">The sub <see cref="string"/> to search up to.</param>
        /// <param name="offset_index">How far into the <see cref="string"/> to start searching for the end sub <see cref="string"/>.</param>
        /// <returns>
        /// Returns an empty string if the end sub <see cref="string"/> could not be found or if the offset index was out of bounds.
        /// Returns the text before the end sub <see cref="string"/> otherwise.
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
                    LibraryDebug.PrintLine(nameof(str), str);

                    return result;
                }

                result = str.Substring(0, index_end);
            }
            catch (Exception exception)
            {
                LibraryDebug.Error("Failed to find text after " + end.Wrap("\"", "\""), TimeStamp.TimeLong);
                LibraryDebug.PrintLine(nameof(str), str);
                LibraryDebug.PrintLine(nameof(index_offset), index_offset.ToString());
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets the text between two <see cref="char"/>s in a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to be parsed.</param>
        /// <param name="start">The <see cref="char"/> to search from.</param>
        /// <param name="end">The <see cref="char"/> to search up to.</param>
        /// <param name="offset_index">How far into the <see cref="string"/> to start searching for the start <see cref="char"/>.</param>
        /// <returns>
        /// Returns an empty string if the start or end <see cref="char"/>s could not be found or if the offset index was out of bounds.
        /// Returns the text between the start and end <see cref="char"/>s otherwise.
        /// </returns>
        public static string TextBetween(this string str, char start, char end, int offset_index = 0)
        {
            return str.TextBetween(start.ToString(), end.ToString(), offset_index);
        }

        /// <summary>
        /// Gets the text between two sub <see cref="string"/>s in a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to be parsed.</param>
        /// <param name="start">The sub <see cref="string"/> to search from.</param>
        /// <param name="end">The sub <see cref="string"/> to search up to.</param>
        /// <param name="offset_index">How far into the <see cref="string"/> to start searching for the start sub <see cref="string"/>.</param>
        /// <returns>
        /// Returns an empty string if the start or end sub <see cref="string"/>s could not be found or if the offset index was out of bounds.
        /// Returns the text between the start and end <see cref="char"/>s otherwise.
        /// </returns>
        public static string TextBetween(this string str, string start, string end, int offset_index = 0)
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
                int index_start = str.IndexOf(start, offset_index);
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
                LibraryDebug.PrintLine(nameof(str), str);
                LibraryDebug.PrintLine(nameof(offset_index), offset_index.ToString());
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
            }

            return result;
        }

        /// <summary>
        /// Converts a <see cref="string"/> into an <see cref="Array"/> of a specified type.
        /// </summary>
        /// <typeparam name="type">The type of the returned <see cref="Array"/>.</typeparam>
        /// <param name="str">The <see cref="string"/> to be parsed.</param>
        /// <param name="separator">An <see cref="char"/> that represents a point to separate the string into elemnts.</param>
        /// <returns>
        /// Returns a default <see cref="Array"/> of the specified type is no <see cref="string"/> elements could be converted.
        /// Returns an <see cref="Array"/> of a specified type with the successfully converted <see cref="string"/> elements otherwise.
        /// </returns>
        public static type[] StringToArray<type>(this string str, char separator, StringSplitOptions options = StringSplitOptions.None)
        {
            return str.StringToArray<type>(new char[] { separator }, options);
        }

        /// <summary>
        /// Converts a <see cref="string"/> into an <see cref="Array"/> of a specified type.
        /// </summary>
        /// <typeparam name="type">The type of the returned <see cref="Array"/>.</typeparam>
        /// <param name="str">The <see cref="string"/> to be parsed.</param>
        /// <param name="separator">An <see cref="Array"/> of <see cref="char"/>s that represent points to separate the string into elemnts.</param>
        /// <returns>
        /// Returns a default <see cref="Array"/> of the specified type is no <see cref="string"/> elements could be converted.
        /// Returns an <see cref="Array"/> of a specified type with the successfully converted <see cref="string"/> elements otherwise.
        /// </returns>
        public static type[] StringToArray<type>(this string str, char[] separators, StringSplitOptions options = StringSplitOptions.None)
        {
            if (!str.isValid())
            {
                return default(type[]);
            }

            List<type> result = new List<type>();

            string[] array = str.Split(separators, options);
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

        #region String helpers

        /// <summary>
        /// Wraps a <see cref="string"/> with the specified <see cref="string"/>s.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to be wrapped.</param>
        /// <param name="prefix">The <see cref="string"/> to be placed at the begining of the current <see cref="string"/>.</param>
        /// <param name="suffix">The <see cref="string"/> to be placed at the end of the current <see cref="string"/>.</param>
        /// <returns>
        /// Returns the current string wrapped in the prefix <see cref="string"/> and the suffix <see cref="string"/>.
        /// </returns>
        public static string Wrap(this string str, string prefix, string suffix)
        {
            return prefix + str + suffix;
        }

        /// <summary>
        /// Wraps a <see cref="string"/> with the specified <see cref="char"/>s.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to be wrapped.</param>
        /// <param name="prefix">The <see cref="char"/> to be placed at the begining of the current <see cref="string"/>.</param>
        /// <param name="suffix">The <see cref="char"/> to be placed at the end of the current <see cref="string"/>.</param>
        /// <returns>
        /// Returns the current string wrapped in the prefix <see cref="char"/> and the suffix <see cref="char"/>.
        /// </returns>
        public static string Wrap(this string str, char prefix, char suffix)
        {
            return prefix + str + suffix;
        }

        /// <summary>
        /// Removes padding from the left, right, both sides of a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to be trimmed.</param>
        /// <param name="side">Which side of the <see cref="string"/> to be trimmed.</param>
        /// <returns>
        /// Returns a <see cref="string"/> that has with all padding removed from the specified side of the string.
        /// </returns>
        public static string RemovePadding(this string str, Padding side = Padding.Both)
        {
            string result = string.Empty;

            if (!str.isValid())
            {
                return str;
            }

            switch (side)
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

        #endregion

        #region Enum helpers

        /// <summary>
        /// Converts a <see cref="string"/> to a specified <see cref="enum"/> equivalent value.
        /// </summary>
        /// <typeparam name="type">The <see cref="enum"/> for the <see cref="string"/> to be converted to.</typeparam>
        /// <param name="str">The <see cref="string"/> to be converted.</param>
        /// <param name="ignore_case">Determins whether or not to ignore the case of the <see cref="string"/> when converting it to the <see cref="enum"/> value.</param>
        /// <returns>
        /// Returns the default value of the specified <see cref="enum"/> if the <see cref="string"/> could not be converted.
        /// Returns the equivalent <see cref="enum"/> value otherwise.
        /// </returns>
        public static type ConvertToEnum<type>(this string str, bool ignore_case = true) where type : struct
        {
            type value = default(type);

            if (!str.CanCovertTo<type>())
            {
                return value;                
            }            

            try
            {
                Enum.TryParse(str, ignore_case, out value);
            }
            catch(Exception exception)
            {
                LibraryDebug.Error(LibraryDebugMethod.CONVERT, "str", LibraryDebugError.NORMAL_EXCEPTION);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
                LibraryDebug.PrintLine(nameof(str), str);
            }

            return value;
        }

        #endregion

        #region Serialization helpers

        /// <summary>
        /// Asynchronously attempts to serialize an <see cref="object"/> using JSON.net into a <see cref="string"/>.
        /// </summary>
        /// <typeparam name="type">The implied type of the <see cref="object"/>.</typeparam>
        /// <param name="obj">The <see cref="object"/> to be serialized into a <see cref="string"/>.</param>
        /// <returns>
        /// Returns an empty <see cref="string"/> if the object could not be serialized or if the <see cref="Task.Run(Action)"/> failed.
        /// Returns the serialized <see cref="string"/> of the <see cref="object"/> otherwise.
        /// </returns>
        public static async Task<string> TrySerializeObjectAsync<type>(this type obj)
        {
            string result = string.Empty;

            try
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

                        return string.Empty;
                    }
                });

                result = await task;

                if (task.IsFaulted)
                {
                    LibraryDebug.Error(LibraryDebugMethod.SERIALIZE, typeof(type).Name, LibraryDebugError.NORMAL_FAULTED);
                    LibraryDebug.PrintLine(nameof(task.Id), task.Id.ToString());
                    LibraryDebug.PrintLine(nameof(task.Exception), task.Exception.Message);
                }
            }
            catch (Exception exception)
            {
                LibraryDebug.Error(LibraryDebugMethod.SERIALIZE, typeof(type).Name, LibraryDebugError.NORMAL_EXCEPTION);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
            }

            return result;
        }

        /// <summary>
        /// Asynchronously attempts to deserialize a <see cref="string"/> using JSON.net into an <see cref="object"/>.
        /// </summary>
        /// <typeparam name="type">The specified type of the <see cref="object"/> to deserialize the <see cref="string"/> as.</typeparam>
        /// <param name="str">The <see cref="string"/> to be deserialized.</param>
        /// <returns>
        /// Returns a default <see cref="object"/> of the specified type if the <see cref="string"/> could not be deserialized or if the <see cref="Task.Run(Action)"/> failed.
        /// Returns the deserialized <see cref="object"/> of the <see cref="string"/> otherwise.
        /// </returns>
        public static async Task<type> TryDeserializeObjectAsync<type>(this string str)
        {
            type result = default(type);

            try
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

                result = await task;

                if (task.IsFaulted)
                {
                    LibraryDebug.Error(LibraryDebugMethod.SERIALIZE, typeof(type).Name, LibraryDebugError.NORMAL_FAULTED);
                    LibraryDebug.PrintLine(nameof(task.Id), task.Id.ToString());
                    LibraryDebug.PrintLine(nameof(task.Exception), task.Exception.Message);
                }
            }
            catch(Exception exception)
            {
                LibraryDebug.Error(LibraryDebugMethod.DESERIALIZE, typeof(type).Name, LibraryDebugError.NORMAL_EXCEPTION);
                LibraryDebug.PrintLine(nameof(exception), exception.Message);
            }

            return result;
        }

        #endregion
    }
}
