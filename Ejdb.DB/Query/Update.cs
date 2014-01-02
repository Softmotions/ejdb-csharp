using Ejdb.BSON;

namespace Ejdb.DB
{
	public class Update
	{
		/// <summary>
		/// Sets the value of the named element to a new value (see $set).
		/// </summary>
		/// <param name="name">The name of the element to be set.</param>
		/// <param name="value">The new value.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public static UpdateBuilder Set(string name, object value)
		{
			return new UpdateBuilder().Set(name, value);
		}

		/// <summary>
		/// Increments the named element by a value (see $inc).
		/// </summary>
		/// <param name="name">The name of the element to be incremented.</param>
		/// <param name="value">The value to increment by.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public static UpdateBuilder Inc(string name, object value)
		{
			return new UpdateBuilder().Inc(name, value);
		}

		/// <summary>
		/// In-place record removal operation ($dropall).
		/// </summary>
		/// <returns>The builder (so method calls can be chained).</returns>
		public static UpdateBuilder DropAll()
		{
			return new UpdateBuilder().DropAll();
		}

		/// <summary>
		/// Removes all values from the named array element that are equal to some value (see $pull).
		/// </summary>
		/// <param name="name">The name of the array element.</param>
		/// <param name="value">The value to remove.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public static UpdateBuilder Pull(string name, object value)
		{
			return new UpdateBuilder().Pull(name, value);
		}

		/// <summary>
		/// Removes all values from the named array element that are equal to any of a list of values (see $pullAll).
		/// </summary>
		/// <param name="name">The name of the array element.</param>
		/// <param name="values">The values to remove.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public static UpdateBuilder PullAll<T>(string name, params T[] values)
		{
			return new UpdateBuilder().PullAll(name, values);
		}

		// public methods
		/// <summary>
		/// Adds a value to a named array element if the value is not already in the array (see $addToSet).
		/// </summary>
		/// <param name="name">The name of the array element.</param>
		/// <param name="value">The value to add to the set.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public static UpdateBuilder AddToSet(string name, object value)
		{
			return new UpdateBuilder().AddToSet(name, value);
		}

		/// <summary>
		/// Adds a list of values to a named array element adding each value only if it not already in the array (see $addToSet and $each).
		/// </summary>
		/// <param name="name">The name of the array element.</param>
		/// <param name="values">The values to add to the set.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public static UpdateBuilder AddToSetAll<T>(string name, params T[] values)
		{
			return new UpdateBuilder().AddToSetAll(name, values);
		}
	}
}