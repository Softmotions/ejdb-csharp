// Copyright © Anton Paar GmbH, 2012-2013

#region Copyright (c) 2011-06, Olaf Kober <amarok.projects@gmail.com>
//================================================================================
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//	copies of the Software, and to permit persons to whom the Software is
//	furnished to do so, subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in
//	all copies or substantial portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//	THE SOFTWARE.
//================================================================================
#endregion

using System;
using System.Globalization;
using System.Runtime.CompilerServices;


namespace Ejdb
{
	/// <summary>
	/// This type provides static methods for validating argument values on public methods and throwing appropriate 
	/// argument exceptions when specific conditions are not met.
	/// </summary>
	public static class Verify
	{
		#region ++ Public Interface (NotNull) ++

		/// <summary>
		/// Verifies that the supplied value is not a null reference, hence, that it refers to a valid object.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		
		public static void NotNull(Object value, String paramName, Func<String> messageFunc = null)
		{
			if (value == null)
				_ThrowArgumentNullException(paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (NotEmpty) ++

		/// <summary>
		/// Verifies that the supplied string value is neither a null reference nor an empty string.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		/// <exception cref="ArgumentException">
		/// An empty string was passed to a method that did not accept it as a valid argument.</exception>
		
		public static void NotEmpty(String value, String paramName, Func<String> messageFunc = null)
		{
			if (value == null)
				_ThrowArgumentNullException(paramName, messageFunc);
			if (value.Length == 0)
				_ThrowEmptyStringException(paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (NotNegative) ++

		/// <summary>
		/// Verifies that the supplied value is not negative; thus, that the value is equal to or greater than zero.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// positive (equal to or greater than zero).</exception>
		
		public static void NotNegative(Int32 value, String paramName, Func<String> messageFunc = null)
		{
			if (value < 0)
				_ThrowValueMustBePositiveException(value, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is not negative; thus, that the value is equal to or greater than zero.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// positive (equal to or greater than zero).</exception>
		
		public static void NotNegative(Int64 value, String paramName, Func<String> messageFunc = null)
		{
			if (value < 0L)
				_ThrowValueMustBePositiveException(value, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is not negative; thus, that the value is equal to or greater than zero.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// positive (equal to or greater than zero).</exception>
		
		public static void NotNegative(Single value, String paramName, Func<String> messageFunc = null)
		{
			if (value < 0.0)
				_ThrowValueMustBePositiveException(value, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is not negative; thus, that the value is equal to or greater than zero.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// positive (equal to or greater than zero).</exception>
		
		public static void NotNegative(Double value, String paramName, Func<String> messageFunc = null)
		{
			if (value < 0.0)
				_ThrowValueMustBePositiveException(value, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is not negative; thus, that the value is equal to or greater than zero.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// positive (equal to or greater than zero).</exception>
		
		public static void NotNegative(Decimal value, String paramName, Func<String> messageFunc = null)
		{
			if (value < 0)
				_ThrowValueMustBePositiveException(value, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is not negative; thus, that the value is equal to or greater than zero.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// positive (equal to or greater than zero).</exception>
		
		public static void NotNegative(TimeSpan value, String paramName, Func<String> messageFunc = null)
		{
			if (value < TimeSpan.Zero)
				_ThrowValueMustBePositiveException(value, paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (AtLeast) ++

		/// <summary>
		/// Verifies that the supplied value is at least (equal to or greater than) the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at least (equal to or greater than) the minimum value.</exception>
		
		public static void AtLeast(Byte value, Byte minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue)
				_ThrowValueMustBeAtLeastException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at least (equal to or greater than) the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at least (equal to or greater than) the minimum value.</exception>
		
		public static void AtLeast(Int32 value, Int32 minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue)
				_ThrowValueMustBeAtLeastException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at least (equal to or greater than) the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at least (equal to or greater than) the minimum value.</exception>
		
		public static void AtLeast(Int64 value, Int64 minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue)
				_ThrowValueMustBeAtLeastException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at least (equal to or greater than) the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at least (equal to or greater than) the minimum value.</exception>
		
		public static void AtLeast(Single value, Single minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue)
				_ThrowValueMustBeAtLeastException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at least (equal to or greater than) the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at least (equal to or greater than) the minimum value.</exception>
		public static void AtLeast(Double value, Double minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue)
				_ThrowValueMustBeAtLeastException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at least (equal to or greater than) the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at least (equal to or greater than) the minimum value.</exception>
		public static void AtLeast(Decimal value, Decimal minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue)
				_ThrowValueMustBeAtLeastException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at least (equal to or greater than) the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at least (equal to or greater than) the minimum value.</exception>
		public static void AtLeast(TimeSpan value, TimeSpan minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue)
				_ThrowValueMustBeAtLeastException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at least (equal to or greater than) the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at least (equal to or greater than) the minimum value.</exception>
		public static void AtLeast(DateTime value, DateTime minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue)
				_ThrowValueMustBeAtLeastException(value, minValue, paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (AtMost) ++

		/// <summary>
		/// Verifies that the supplied value is at most (equal to or less than) the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at most (equal to or less than) the maximum value.</exception>
		public static void AtMost(Byte value, Byte maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value > maxValue)
				_ThrowValueMustBeAtMostException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at most (equal to or less than) the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at most (equal to or less than) the maximum value.</exception>
		public static void AtMost(Int32 value, Int32 maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value > maxValue)
				_ThrowValueMustBeAtMostException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at most (equal to or less than) the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at most (equal to or less than) the maximum value.</exception>
		public static void AtMost(Int64 value, Int64 maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value > maxValue)
				_ThrowValueMustBeAtMostException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at most (equal to or less than) the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at most (equal to or less than) the maximum value.</exception>
		public static void AtMost(Single value, Single maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value > maxValue)
				_ThrowValueMustBeAtMostException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at most (equal to or less than) the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at most (equal to or less than) the maximum value.</exception>
		public static void AtMost(Double value, Double maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value > maxValue)
				_ThrowValueMustBeAtMostException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at most (equal to or less than) the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at most (equal to or less than) the maximum value.</exception>
		public static void AtMost(Decimal value, Decimal maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value > maxValue)
				_ThrowValueMustBeAtMostException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at most (equal to or less than) the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at most (equal to or less than) the maximum value.</exception>
		public static void AtMost(TimeSpan value, TimeSpan maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value > maxValue)
				_ThrowValueMustBeAtMostException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is at most (equal to or less than) the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// at most (equal to or less than) the maximum value.</exception>
		public static void AtMost(DateTime value, DateTime maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value > maxValue)
				_ThrowValueMustBeAtMostException(value, maxValue, paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (IsGreater) ++

		/// <summary>
		/// Verifies that the supplied value is greater than the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// greater than the minimum value.</exception>
		
		public static void IsGreater(Byte value, Byte minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value <= minValue)
				_ThrowValueMustBeGreaterThanException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is greater than the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// greater than the minimum value.</exception>
		
		public static void IsGreater(Int32 value, Int32 minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value <= minValue)
				_ThrowValueMustBeGreaterThanException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is greater than the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// greater than the minimum value.</exception>
		
		public static void IsGreater(Int64 value, Int64 minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value <= minValue)
				_ThrowValueMustBeGreaterThanException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is greater than the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// greater than the minimum value.</exception>
		
		public static void IsGreater(Single value, Single minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value <= minValue)
				_ThrowValueMustBeGreaterThanException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is greater than the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// greater than the minimum value.</exception>
		
		public static void IsGreater(Double value, Double minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value <= minValue)
				_ThrowValueMustBeGreaterThanException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is greater than the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// greater than the minimum value.</exception>
		
		public static void IsGreater(Decimal value, Decimal minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value <= minValue)
				_ThrowValueMustBeGreaterThanException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is greater than the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// greater than the minimum value.</exception>
		
		public static void IsGreater(TimeSpan value, TimeSpan minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value <= minValue)
				_ThrowValueMustBeGreaterThanException(value, minValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is greater than the specified minimum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// greater than the minimum value.</exception>
		
		public static void IsGreater(DateTime value, DateTime minValue, String paramName, Func<String> messageFunc = null)
		{
			if (value <= minValue)
				_ThrowValueMustBeGreaterThanException(value, minValue, paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (IsLess) ++

		/// <summary>
		/// Verifies that the supplied value is less than the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// less than the maximum value.</exception>
		
		public static void IsLess(Byte value, Byte maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value >= maxValue)
				_ThrowValueMustBeLessThanException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is less than the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// less than the maximum value.</exception>
		
		public static void IsLess(Int32 value, Int32 maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value >= maxValue)
				_ThrowValueMustBeLessThanException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is less than the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// less than the maximum value.</exception>
		
		public static void IsLess(Int64 value, Int64 maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value >= maxValue)
				_ThrowValueMustBeLessThanException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is less than the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// less than the maximum value.</exception>
		
		public static void IsLess(Single value, Single maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value >= maxValue)
				_ThrowValueMustBeLessThanException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is less than the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// less than the maximum value.</exception>
		
		public static void IsLess(Double value, Double maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value >= maxValue)
				_ThrowValueMustBeLessThanException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is less than the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// less than the maximum value.</exception>
		
		public static void IsLess(Decimal value, Decimal maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value >= maxValue)
				_ThrowValueMustBeLessThanException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is less than the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// less than the maximum value.</exception>
		
		public static void IsLess(TimeSpan value, TimeSpan maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value >= maxValue)
				_ThrowValueMustBeLessThanException(value, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is less than the specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// less than the maximum value.</exception>
		
		public static void IsLess(DateTime value, DateTime maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value >= maxValue)
				_ThrowValueMustBeLessThanException(value, maxValue, paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (InRange) ++

		/// <summary>
		/// Verifies that the supplied value is in the range of the supplied minimum and maximum values, meaning
		/// that the value is equal to or greater than the specified minimum value and equal to or less than the 
		/// specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// equal to or greater than the minimum value and equal to or less than the maximum value.</exception>
		
		public static void InRange(Byte value, Byte minValue, Byte maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue || value > maxValue)
				_ThrowValueMustBeInRangeException(value, minValue, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is in the range of the supplied minimum and maximum values, meaning
		/// that the value is equal to or greater than the specified minimum value and equal to or less than the 
		/// specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// equal to or greater than the minimum value and equal to or less than the maximum value.</exception>
		
		public static void InRange(Int32 value, Int32 minValue, Int32 maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue || value > maxValue)
				_ThrowValueMustBeInRangeException(value, minValue, maxValue, paramName, messageFunc);
		}


		/// <summary>
		/// Verifies that the supplied value is in the range of the supplied minimum and maximum values, meaning
		/// that the value is equal to or greater than the specified minimum value and equal to or less than the 
		/// specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// equal to or greater than the minimum value and equal to or less than the maximum value.</exception>
		
		public static void InRange(Int64 value, Int64 minValue, Int64 maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue || value > maxValue)
				_ThrowValueMustBeInRangeException(value, minValue, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is in the range of the supplied minimum and maximum values, meaning
		/// that the value is equal to or greater than the specified minimum value and equal to or less than the 
		/// specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// equal to or greater than the minimum value and equal to or less than the maximum value.</exception>
		
		public static void InRange(Single value, Single minValue, Single maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue || value > maxValue)
				_ThrowValueMustBeInRangeException(value, minValue, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is in the range of the supplied minimum and maximum values, meaning
		/// that the value is equal to or greater than the specified minimum value and equal to or less than the 
		/// specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// equal to or greater than the minimum value and equal to or less than the maximum value.</exception>
		
		public static void InRange(Double value, Double minValue, Double maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue || value > maxValue)
				_ThrowValueMustBeInRangeException(value, minValue, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is in the range of the supplied minimum and maximum values, meaning
		/// that the value is equal to or greater than the specified minimum value and equal to or less than the 
		/// specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// equal to or greater than the minimum value and equal to or less than the maximum value.</exception>
		
		public static void InRange(Decimal value, Decimal minValue, Decimal maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue || value > maxValue)
				_ThrowValueMustBeInRangeException(value, minValue, maxValue, paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the supplied value is in the range of the supplied minimum and maximum values, meaning
		/// that the value is equal to or greater than the specified minimum value and equal to or less than the 
		/// specified maximum value.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="minValue">
		/// The minimum value.</param>
		/// <param name="maxValue">
		/// The maximum value.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value must be 
		/// equal to or greater than the minimum value and equal to or less than the maximum value.</exception>
		
		public static void InRange(TimeSpan value, TimeSpan minValue, TimeSpan maxValue, String paramName, Func<String> messageFunc = null)
		{
			if (value < minValue || value > maxValue)
				_ThrowValueMustBeInRangeException(value, minValue, maxValue, paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (IsUniversalTime) ++

		/// <summary>
		/// Verifies that the supplied date/time value is in coordinated universal time (UTC).
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A date/time value was passed to a method that did not accept it as a valid argument, because the 
		/// value is expected to be in coordinated universal time (UTC).</exception>
		
		public static void IsUniversalTime(DateTime value, String paramName, Func<String> messageFunc = null)
		{
			if (value.Kind != DateTimeKind.Utc)
				_ThrowDateTimeMustBeInUniversalTimeException(paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (IsLocalTime) ++

		/// <summary>
		/// Verifies that the supplied date/time value is in local time.
		/// </summary>
		/// 
		/// <param name="value">
		/// The value to verify.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A date/time value was passed to a method that did not accept it as a valid argument, because the 
		/// value is expected to be in local time.</exception>
		
		public static void IsLocalTime(DateTime value, String paramName, Func<String> messageFunc = null)
		{
			if (value.Kind != DateTimeKind.Local)
				_ThrowDateTimeMustBeInLocalTimeException(paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (BufferSegment) ++

		/// <summary>
		/// Verifies that the supplied arguments specifying a buffer segment are valid.
		/// </summary>
		/// 
		/// <param name="buffer">
		/// The byte array containing the elements of the segment.</param>
		/// <param name="offset">
		/// The zero-based index of the first byte in the segment.</param>
		/// <param name="count">
		/// The number of bytes in the segment.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// A value was passed to a method that did not accept it as a valid argument, because the value 
		/// must be positive (equal to or greater than zero).</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// An offset value was passed to a method that did not accept it as a valid argument, because the 
		/// offset exceeds the length of the associated byte array.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// A count value was passed to a method that did not accept it as a valid argument, because the count 
		/// exceeds the length of the associated byte array.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Offset and count values were passed to a method that did not accept it as valid arguments, because 
		/// the sum of offset and count exceeds the length of the associated byte array.</exception>
		
		public static void BufferSegment(Byte[] buffer, Int32 offset, Int32 count, Func<String> messageFunc = null)
		{
			if (buffer == null)
				_ThrowArgumentNullException("buffer", messageFunc);
			if (offset < 0)
				_ThrowValueMustBePositiveException(offset, "offset", messageFunc);
			if (count < 0)
				_ThrowValueMustBePositiveException(count, "count", messageFunc);
			if ((buffer.Length == 0 && offset > 0) || (buffer.Length > 0 && offset >= buffer.Length))
				_ThrowOffsetExceedsBufferLengthException(offset, "offset", messageFunc);
			if (count > buffer.Length)
				_ThrowCountExceedsBufferLengthException(count, "count", messageFunc);
			if (offset + count > buffer.Length)
				_ThrowOffsetAndCountExceedBufferLengthException(offset + count, "offset + count", messageFunc);
		}

		#endregion

		#region ++ Public Interface (IsSubclassOf) ++

		/// <summary>
		/// Verifies that the supplied type is a sub-class of the specified base class.
		/// 
		/// This method cannot be used to determine whether an interface derives from another interface, 
		/// or whether a class implements an interface.
		/// </summary>
		/// 
		/// <param name="type">
		/// The type to verify.</param>
		/// <param name="baseType">
		/// The base type from which the type must derive.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		/// <exception cref="ArgumentException">
		/// A value was passed to a method that did not accept it as a valid argument, because 
		/// the value representing a type must be a sub-class of specified base type.</exception>
		
		public static void IsSubclassOf(Type type, Type baseType, String paramName, Func<String> messageFunc = null)
		{
			if (type == null || baseType == null)
				_ThrowArgumentNullException(paramName, messageFunc);
			if (type.IsSubclassOf(baseType) == false)
				_ThrowTypeMustBeSubclassOfException(baseType, paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (IsAssignableTo) ++

		/// <summary>
		/// Verifies that the supplied type can be assigned to the specified target type, hence, that the supplied
		/// type either derives from the target type, if the target type is a base class, or that the target type 
		/// realizes the target type, if the target type is an interface.
		/// </summary>
		/// 
		/// <param name="type">
		/// The type to verify.</param>
		/// <param name="targetType">
		/// The target type to which the type can be assigned.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		/// <exception cref="ArgumentException">
		/// A value was passed to a method that did not accept it as a valid argument, because 
		/// the value representing a type is not assignable to the specified target type.</exception>
		
		public static void IsAssignableTo(Type type, Type targetType, String paramName, Func<String> messageFunc = null)
		{
			if (type == null || targetType == null)
				_ThrowArgumentNullException(paramName, messageFunc);
			if (targetType.IsAssignableFrom(type) == false)
				_ThrowTypeMustBeAssignableToException(targetType, paramName, messageFunc);
		}

		#endregion

		#region ++ Public Interface (That) ++

		/// <summary>
		/// Verifies that the boolean result of a condition is true.
		/// </summary>
		/// 
		/// <param name="conditionResult">
		/// The boolean result of a condition, expected to be true.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="messageFunc">
		/// A callback used to obtain the exception message in case that verification failed.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// A value was passed to a method that did not accept it as a valid argument.</exception>
		
		public static void That(Boolean conditionResult, String paramName, Func<String> messageFunc = null)
		{
			if (conditionResult == false)
				_ThrowInvalidArgumentException(paramName, messageFunc);
		}

		/// <summary>
		/// Verifies that the boolean result of a condition is true.
		/// </summary>
		/// 
		/// <param name="conditionResult">
		/// The boolean result of a condition, expected to be true.</param>
		/// <param name="paramName">
		/// The name of the method parameter that is verified.</param>
		/// <param name="message">
		/// The exception message for the case that the condition result is false.</param>
		/// 
		/// <exception cref="ArgumentException">
		/// A value was passed to a method that did not accept it as a valid argument.</exception>
		
		public static void That(Boolean conditionResult, String paramName, String message)
		{
			if (conditionResult == false)
				throw new ArgumentException(message, paramName);
		}

		#endregion

		#region Implementation

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowArgumentNullException(String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				VerifyResources.NullArgument;

			throw new ArgumentNullException(paramName, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowEmptyStringException(String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				VerifyResources.EmptyStringArgument;

			throw new ArgumentException(message, paramName);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowValueMustBePositiveException(Object value, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				VerifyResources.ValueMustBePositive;

			throw new ArgumentOutOfRangeException(paramName, value, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowValueMustBeAtLeastException(Object value, Object minValue, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				String.Format(CultureInfo.InvariantCulture, VerifyResources.ValueMustBeAtLeast, minValue);

			throw new ArgumentOutOfRangeException(paramName, value, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowValueMustBeAtMostException(Object value, Object maxValue, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				String.Format(CultureInfo.InvariantCulture, VerifyResources.ValueMustBeAtMost, maxValue);

			throw new ArgumentOutOfRangeException(paramName, value, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowValueMustBeGreaterThanException(Object value, Object minValue, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				String.Format(CultureInfo.InvariantCulture, VerifyResources.ValueMustBeGreaterThan, minValue);

			throw new ArgumentOutOfRangeException(paramName, value, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowValueMustBeLessThanException(Object value, Object maxValue, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				String.Format(CultureInfo.InvariantCulture, VerifyResources.ValueMustBeLessThan, maxValue);

			throw new ArgumentOutOfRangeException(paramName, value, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowValueMustBeInRangeException(Object value, Object minValue, Object maxValue, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				String.Format(CultureInfo.InvariantCulture, VerifyResources.ValueMustBeInRange, minValue, maxValue);

			throw new ArgumentOutOfRangeException(paramName, value, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowDateTimeMustBeInUniversalTimeException(String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				VerifyResources.DateTimeMustBeInUniversalTime;

			throw new ArgumentException(message, paramName);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowDateTimeMustBeInLocalTimeException(String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				VerifyResources.DateTimeMustBeInLocalTime;

			throw new ArgumentException(message, paramName);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowOffsetExceedsBufferLengthException(Object value, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				VerifyResources.OffsetExceedsBufferLength;

			throw new ArgumentOutOfRangeException(paramName, value, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowCountExceedsBufferLengthException(Object value, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				VerifyResources.CountExceedsBufferLength;

			throw new ArgumentOutOfRangeException(paramName, value, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowOffsetAndCountExceedBufferLengthException(Object value, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				VerifyResources.OffsetAndCountExceedBufferLength;

			throw new ArgumentOutOfRangeException(paramName, value, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowTypeMustBeSubclassOfException(Type baseType, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				String.Format(CultureInfo.InvariantCulture, VerifyResources.TypeMustBeSubclassOf, baseType);

			throw new ArgumentException(message, paramName);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowTypeMustBeAssignableToException(Type targetType, String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				String.Format(CultureInfo.InvariantCulture, VerifyResources.TypeMustBeAssignableTo, targetType);

			throw new ArgumentException(message, paramName);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void _ThrowInvalidArgumentException(String paramName, Func<String> messageFunc)
		{
			var message = messageFunc != null ?
				messageFunc() :
				VerifyResources.InvalidArgument;

			throw new ArgumentException(message, paramName);
		}

		#endregion

	}
}
