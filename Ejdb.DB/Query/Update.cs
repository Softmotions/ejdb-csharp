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
	}
}