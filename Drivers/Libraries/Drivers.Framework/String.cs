﻿#region LICENSE

// ---------------------------------- LICENSE ---------------------------------- //
//
//    Fling OS - The educational operating system
//    Copyright (C) 2015 Edward Nutting
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//  Project owner: 
//		Email: edwardnutting@outlook.com
//		For paper mail address, please contact via email for details.
//
// ------------------------------------------------------------------------------ //

#endregion

using Drivers.Compiler.Attributes;
using Drivers.Framework.Collections;
using Drivers.Framework.Exceptions;
using Drivers.Utilities;

namespace Drivers.Framework
{
    /// <summary>
    ///     Replacement class for methods, properties and fields usually found on standard System.String type.
    ///     Also contains utility methods for low-level string manipulation.
    /// </summary>
    [StringClass]
    public sealed class String : Object
    {
        /* If you add more fields here, remember to update the compiler and all the ASM files that depend on the string
           class structure ( i.e. do all the hard work! ;) )
         */

        /// <summary>
        ///     The size of the fields in an string object that come before the actual string data.
        /// </summary>
        public const uint FieldsBytesSize = 8;

        /// <summary>
        ///     The length of the string.
        /// </summary>
        public int length;

        /// <summary>
        ///     Gets the character at the specified index.
        /// </summary>
        /// <param name="index">The index of the character to get.</param>
        /// <returns>The character at the specified index.</returns>
        public unsafe char this[int index]
        {
            [NoDebug]
            get
            {
                byte* thisPtr = (byte*)ObjectUtilities.GetHandle(this);
                thisPtr += 8; /*For fields inc. inherited*/
                return ((char*)thisPtr)[index];
            }
            [NoDebug]
            set
            {
                byte* thisPtr = (byte*)ObjectUtilities.GetHandle(this);
                thisPtr += 8; /*For fields inc. inherited*/
                ((char*)thisPtr)[index] = value;
            }
        }

        /// <summary>
        ///     Gets the character at the specified index.
        /// </summary>
        /// <param name="index">The index of the character to get.</param>
        /// <returns>The character at the specified index.</returns>
        public unsafe char this[uint index]
        {
            [NoDebug]
            get
            {
                byte* thisPtr = (byte*)ObjectUtilities.GetHandle(this);
                thisPtr += 8; /*For fields inc. inherited*/
                return ((char*)thisPtr)[index];
            }
            [NoDebug]
            set
            {
                byte* thisPtr = (byte*)ObjectUtilities.GetHandle(this);
                thisPtr += 8; /*For fields inc. inherited*/
                ((char*)thisPtr)[index] = value;
            }
        }

        /*   ----------- DO NOT CREATE A CONSTRUCTOR FOR THIS CLASS - IT WILL NEVER BE CALLED IF YOU DO ----------- */

        /// <summary>
        ///     Creates a new, blank Framework.String of specified length.
        ///     IMPORTANT NOTE: You MUST assign the return value of this to a variable / local / arg /
        ///     field etc. You may not use IL or C# that results in an IL Pop op of the return value
        ///     of this method as it will screw up the GC RefCount handling.
        /// </summary>
        /// <param name="length">The length of the string to create.</param>
        /// <returns>The new string.</returns>
        [NoGC]
        [NoDebug]
        public static unsafe String New(int length)
        {
            if (length < 0)
            {
                ExceptionMethods.Throw(
                    new ArgumentException(
                        "Parameter \"length\" cannot be less than 0 in Framework.String.New(int length)."));
            }
            String result = (String)ObjectUtilities.GetObject(GC.NewString(length));
            if (result == null)
            {
                ExceptionMethods.Throw(new NullReferenceException());
            }
            return result;
        }

        /// <summary>
        ///     Concatenates two strings into one new string.
        /// </summary>
        /// <param name="str1">The first part of the new string.</param>
        /// <param name="str2">The second part of the new string.</param>
        /// <returns>The new string.</returns>
        [NoDebug]
        public static String Concat(String str1, String str2)
        {
            String newStr = New(str1.length + str2.length);
            for (int i = 0; i < str1.length; i++)
            {
                newStr[i] = str1[i];
            }
            for (int i = 0; i < str2.length; i++)
            {
                newStr[i + str1.length] = str2[i];
            }
            return newStr;
        }

        /// <summary>
        ///     Gets a pointer to the first character in the string.
        /// </summary>
        /// <returns>A pointer to the first char (that represents a character) of the specified string.</returns>
        [NoDebug]
        [NoGC]
        public unsafe char* GetCharPointer()
        {
            return (char*)((byte*)ObjectUtilities.GetHandle(this) + FieldsBytesSize);
        }

        /// <summary>
        ///     Creates a new string and pads the left side of the string with the specified character until the
        ///     whole string is of the specified length or returns the original string if it is longer.
        /// </summary>
        /// <param name="totalLength">The final length of the whole string.</param>
        /// <param name="padChar">The character to pad with.</param>
        /// <returns>The new, padded string.</returns>
        [NoDebug]
        public String PadLeft(int totalLength, char padChar)
        {
            String result = New(totalLength);

            if (length >= totalLength)
            {
                for (int i = 0; i < result.length; i++)
                {
                    result[i] = this[i];
                }
                return result;
            }

            int offset = totalLength - length;
            for (int i = 0; i < length; i++)
            {
                result[i + offset] = this[i];
            }
            for (int i = 0; i < offset; i++)
            {
                result[i] = padChar;
            }
            return result;
        }

        /// <summary>
        ///     Creates a new string and pads the right side of the string with the specified character until the
        ///     whole string is of the specified length or returns the original string if it is longer.
        /// </summary>
        /// <param name="totalLength">The final length of the whole string.</param>
        /// <param name="padChar">The character to pad with.</param>
        /// <returns>The new, padded string.</returns>
        [NoDebug]
        public String PadRight(int totalLength, char padChar)
        {
            String result = New(totalLength);
            for (int i = 0; i < length && i < totalLength; i++)
            {
                result[i] = this[i];
            }
            for (int i = length; i < totalLength; i++)
            {
                result[i] = padChar;
            }
            return result;
        }

        /// <summary>
        ///     Creates a new string and trims all spaces from the beginning and end of the string.
        /// </summary>
        /// <returns>The new, trimmed string.</returns>
        [NoDebug]
        public String Trim()
        {
            // All characters in the Zs, Zp and Zl Unicode categories, plus U+0009 CHARACTER TABULATION, U+000A LINE FEED, U+000B LINE TABULATION, U+000C FORM FEED, U+000D CARRIAGE RETURN and U+0085 NEXT LINE
            String TrimChars =
                "\u0009\u000A\u000B\u000C\u000D\u0020\u0085\u00A0\u1680\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200A\u2028\u2029\u202F\u205F\u3000";

            int removeStart = 0;
            int removeEnd = 0;
            for (int i = 0; i < length; removeStart++, i++)
            {
                bool ShouldBreak = true;
                for (int j = 0; j < TrimChars.length; j++)
                {
                    if (this[i] == TrimChars[j])
                    {
                        ShouldBreak = false;
                    }
                }
                if (ShouldBreak)
                {
                    break;
                }
            }
            for (int i = length - 1; i > removeStart; removeEnd++, i--)
            {
                bool ShouldBreak = true;
                for (int j = 0; j < TrimChars.length; j++)
                {
                    if (this[i] == TrimChars[j])
                    {
                        ShouldBreak = false;
                    }
                }
                if (ShouldBreak)
                {
                    break;
                }
            }

            String result = New(length - removeStart - removeEnd);
            for (int i = removeStart; i < length - removeEnd; i++)
            {
                result[i - removeStart] = this[i];
            }
            return result;
        }

        /// <summary>
        ///     Creates a new string and trims all spaces from the end of the string.
        /// </summary>
        /// <returns>The new, trimmed string.</returns>
        [NoDebug]
        public String TrimEnd()
        {
            // All characters in the Zs, Zp and Zl Unicode categories, plus U+0009 CHARACTER TABULATION, U+000A LINE FEED, U+000B LINE TABULATION, U+000C FORM FEED, U+000D CARRIAGE RETURN and U+0085 NEXT LINE
            String TrimChars =
                "\u0009\u000A\u000B\u000C\u000D\u0020\u0085\u00A0\u1680\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200A\u2028\u2029\u202F\u205F\u3000";

            int removeEnd = 0;
            for (int i = length - 1; i > -1; removeEnd++, i--)
            {
                bool ShouldBreak = true;
                for (int j = 0; j < TrimChars.length; j++)
                {
                    if (this[i] == TrimChars[j])
                    {
                        ShouldBreak = false;
                    }
                }
                if (ShouldBreak)
                {
                    break;
                }
            }

            String result = New(length - removeEnd);
            for (int i = 0; i < length - removeEnd; i++)
            {
                result[i] = this[i];
            }
            return result;
        }

        /// <summary>
        ///     Creates a new string that is a copy of the current string starting at the specified index for specified length.
        /// </summary>
        /// <param name="startIndex">The index to start copying at.</param>
        /// <param name="aLength">The number of characters to copy.</param>
        /// <returns>The new string.</returns>
        [NoDebug]
        public String Substring(int startIndex, int aLength)
        {
            if (startIndex >= length)
            {
                if (aLength == 0)
                {
                    return New(0);
                }
                ExceptionMethods.Throw(new IndexOutOfRangeException(startIndex, length));
            }
            else if (aLength > length - startIndex)
            {
                aLength = length - startIndex;
            }

            String result = New(aLength);
            for (int i = startIndex; i < aLength + startIndex; i++)
            {
                result[i - startIndex] = this[i];
            }
            return result;
        }

        /// <summary>
        ///     Determines whether the string starts with the specified string.
        /// </summary>
        /// <param name="prefix">The string to test for.</param>
        /// <returns>Whether the string starts with the prefix.</returns>
        [NoDebug]
        public bool StartsWith(String prefix)
        {
            if (length < prefix.length)
            {
                return false;
            }
            for (int i = 0; i < prefix.length; i++)
            {
                if (this[i] != prefix[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Determines whether the string ends with the specified string.
        /// </summary>
        /// <param name="postfix">The string to test for.</param>
        /// <returns>Whether the string ends with the postfix.</returns>
        [NoDebug]
        public bool EndsWith(String postfix)
        {
            if (length < postfix.length)
            {
                return false;
            }
            int offset = length - postfix.length;
            for (int i = length - 1; i >= offset; i--)
            {
                if (this[i] != postfix[i - offset])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Splits the string at every index where splitChar occurs and adds the splits parts (excluding splitChar)
        ///     to a list of strings.
        /// </summary>
        /// <param name="splitChar">The char to split with.</param>
        /// <returns>The list of split parts.</returns>
        [NoDebug]
        public List Split(char splitChar)
        {
            List result = new List(1);

            int lastSplitIndex = 0;
            for (int i = 0; i < length; i++)
            {
                if (this[i] == splitChar)
                {
                    result.Add(Substring(lastSplitIndex, i - lastSplitIndex));
                    lastSplitIndex = i + 1;
                }
            }
            if (length - lastSplitIndex > 0)
            {
                result.Add(Substring(lastSplitIndex, length - lastSplitIndex));
            }

            return result;
        }

        /// <summary>
        ///     Copies the current string then converts all the alpha-characters to upper-case.
        /// </summary>
        /// <returns>The new, upper-case string.</returns>
        [NoDebug]
        public String ToUpper()
        {
            if (length == 0)
                return "";

            String result = New(length);

            for (int i = 0; i < result.length; i++)
            {
                char cChar = this[i];
                if (cChar >= 'a' && cChar <= 'z')
                {
                    cChar = (char)('A' + (cChar - 'a'));
                }
                result[i] = cChar;
            }

            return result;
        }

        /// <summary>
        ///     Copies the current string then converts all the alpha-characters to lower-case.
        /// </summary>
        /// <returns>The new, lower-case string.</returns>
        [NoDebug]
        public String ToLower()
        {
            String result = New(length);

            for (int i = 0; i < result.length; i++)
            {
                char cChar = this[i];
                if (cChar >= 'A' && cChar <= 'Z')
                {
                    cChar = (char)('a' + (cChar - 'A'));
                }
                result[i] = cChar;
            }

            return result;
        }

        /// <summary>
        ///     Finds the first index of the specified character in the string.
        /// </summary>
        /// <param name="c">The character to find.</param>
        /// <returns>The first instance of the character or -1 if not found.</returns>
        [NoDebug]
        [NoGC]
        public int IndexOf(char c)
        {
            int result = -1;
            for (int i = 0; i < length; i++)
            {
                if (this[i] == c)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        ///     Finds the last index of the specified character in the string.
        /// </summary>
        /// <param name="c">The character to find.</param>
        /// <returns>The last instance of the character or -1 if not found.</returns>
        [NoDebug]
        [NoGC]
        public int LastIndexOf(char c)
        {
            int result = -1;
            for (int i = length - 1; i > -1; i--)
            {
                if (this[i] == c)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        ///     Concatenates two strings using "+" operator.
        /// </summary>
        /// <param name="x">The first string.</param>
        /// <param name="y">The second string.</param>
        /// <returns>The new contenated string.</returns>
        [NoDebug]
        public static String operator +(String x, String y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return null;
                }
                return y;
            }
            if (y == null)
            {
                return x;
            }

            return Concat(x, y);
        }

        /// <summary>
        ///     Tests whether all the characters of two strings are equal.
        /// </summary>
        /// <param name="x">The first string.</param>
        /// <param name="y">The second string.</param>
        /// <returns>Whether the two strings are identical or not.</returns>
        [NoDebug]
        [NoGC]
        public static unsafe bool operator ==(String x, String y)
        {
            bool equal = true;

            //Prevent recursive calls to this "==" implicit method!
            if (ObjectUtilities.GetHandle(x) == null ||
                ObjectUtilities.GetHandle(y) == null)
            {
                if (ObjectUtilities.GetHandle(x) == null &&
                    ObjectUtilities.GetHandle(y) == null)
                {
                    return true;
                }
                return false;
            }

            if (x.length != y.length)
            {
                equal = false;
            }
            else
            {
                for (int i = 0; i < x.length; i++)
                {
                    if (x[i] != y[i])
                    {
                        equal = false;
                        break;
                    }
                }
            }

            return equal;
        }

        /// <summary>
        ///     Tests whether any of the characters of two strings are not equal.
        /// </summary>
        /// <param name="x">The first string.</param>
        /// <param name="y">The second string.</param>
        /// <returns>Whether the two strings mismatch in any place.</returns>
        [NoDebug]
        [NoGC]
        public static bool operator !=(String x, String y)
        {
            return !(x == y);
        }

        /// <summary>
        ///     Implicitly converts the specified value to an Framework.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The Framework.String value.</returns>
        [NoDebug]
        [NoGC]
        public static implicit operator String(string x)
        {
            return (String)(object)x;
        }

        /// <summary>
        ///     Implicitly converts the specified value to an Framework.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The Framework.String value.</returns>
        [NoDebug]
        public static implicit operator String(bool x)
        {
            return x ? "True" : "False";
        }

        /// <summary>
        ///     Implicitly converts the specified Framework.String to a System.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The System.String.</returns>
        [NoDebug]
        public static explicit operator string(String x)
        {
            return (string)(object)x;
        }

        /// <summary>
        ///     Implicitly converts the specified value to a hex Framework.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The Framework.String value.</returns>
        [NoDebug]
        public static implicit operator String(byte x)
        {
            String result = "";
            uint y = x;
            while (y > 0)
            {
                uint rem = y%16u;
                switch (rem)
                {
                    case 0:
                        result = "0" + result;
                        break;
                    case 1:
                        result = "1" + result;
                        break;
                    case 2:
                        result = "2" + result;
                        break;
                    case 3:
                        result = "3" + result;
                        break;
                    case 4:
                        result = "4" + result;
                        break;
                    case 5:
                        result = "5" + result;
                        break;
                    case 6:
                        result = "6" + result;
                        break;
                    case 7:
                        result = "7" + result;
                        break;
                    case 8:
                        result = "8" + result;
                        break;
                    case 9:
                        result = "9" + result;
                        break;
                    case 10:
                        result = "A" + result;
                        break;
                    case 11:
                        result = "B" + result;
                        break;
                    case 12:
                        result = "C" + result;
                        break;
                    case 13:
                        result = "D" + result;
                        break;
                    case 14:
                        result = "E" + result;
                        break;
                    case 15:
                        result = "F" + result;
                        break;
                }
                y = y/16u;
            }
            result = "0x" + result.PadLeft(2, '0');
            return result;
        }

        /// <summary>
        ///     Implicitly converts the specified value to a hex Framework.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The Framework.String value.</returns>
        [NoDebug]
        public static implicit operator String(ushort x)
        {
            String result = "";
            uint y = x;
            while (y > 0)
            {
                uint rem = y & 0xFu;
                switch (rem)
                {
                    case 0:
                        result = "0" + result;
                        break;
                    case 1:
                        result = "1" + result;
                        break;
                    case 2:
                        result = "2" + result;
                        break;
                    case 3:
                        result = "3" + result;
                        break;
                    case 4:
                        result = "4" + result;
                        break;
                    case 5:
                        result = "5" + result;
                        break;
                    case 6:
                        result = "6" + result;
                        break;
                    case 7:
                        result = "7" + result;
                        break;
                    case 8:
                        result = "8" + result;
                        break;
                    case 9:
                        result = "9" + result;
                        break;
                    case 10:
                        result = "A" + result;
                        break;
                    case 11:
                        result = "B" + result;
                        break;
                    case 12:
                        result = "C" + result;
                        break;
                    case 13:
                        result = "D" + result;
                        break;
                    case 14:
                        result = "E" + result;
                        break;
                    case 15:
                        result = "F" + result;
                        break;
                }
                y >>= 4;
            }
            return "0x" + result.PadLeft(4, '0');
        }

        /// <summary>
        ///     Implicitly converts the specified value to an Framework.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The Framework.String value.</returns>
        [NoDebug]
        public static implicit operator String(char x)
        {
            String result = New(1);
            result[0] = x;
            return result;
        }

        /// <summary>
        ///     Implicitly converts the specified value to a hex Framework.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The Framework.String value.</returns>
        [NoDebug]
        public static implicit operator String(short x)
        {
            return (ushort)x;
        }

        /// <summary>
        ///     Implicitly converts the specified value to a hex Framework.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The Framework.String value.</returns>
        [NoDebug]
        public static implicit operator String(uint x)
        {
            String result = "";
            uint y = x;
            while (y > 0)
            {
                uint rem = y & 0xFu;
                switch (rem)
                {
                    case 0:
                        result = "0" + result;
                        break;
                    case 1:
                        result = "1" + result;
                        break;
                    case 2:
                        result = "2" + result;
                        break;
                    case 3:
                        result = "3" + result;
                        break;
                    case 4:
                        result = "4" + result;
                        break;
                    case 5:
                        result = "5" + result;
                        break;
                    case 6:
                        result = "6" + result;
                        break;
                    case 7:
                        result = "7" + result;
                        break;
                    case 8:
                        result = "8" + result;
                        break;
                    case 9:
                        result = "9" + result;
                        break;
                    case 10:
                        result = "A" + result;
                        break;
                    case 11:
                        result = "B" + result;
                        break;
                    case 12:
                        result = "C" + result;
                        break;
                    case 13:
                        result = "D" + result;
                        break;
                    case 14:
                        result = "E" + result;
                        break;
                    case 15:
                        result = "F" + result;
                        break;
                }
                y >>= 4;
            }
            return "0x" + result.PadLeft(8, '0');
        }

        /// <summary>
        ///     Implicitly converts the specified value to a hex Framework.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The Framework.String value.</returns>
        [NoDebug]
        public static implicit operator String(int x)
        {
            return (uint)x;
        }

        /// <summary>
        ///     Implicitly converts the specified value to a hex Framework.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The Framework.String value.</returns>
        [NoDebug]
        public static implicit operator String(ulong x)
        {
            uint part1 = (uint)x;
            uint part2 = (uint)(x >> 16 >> 16);
            return (String)part2 + " " + part1;
        }

        /// <summary>
        ///     Implicitly converts the specified value to a hex Framework.String.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>The Framework.String value.</returns>
        [NoDebug]
        public static implicit operator String(long x)
        {
            return (ulong)x;
        }
    }
}