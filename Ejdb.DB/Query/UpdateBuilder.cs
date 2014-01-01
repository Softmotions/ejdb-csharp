using System;
using Ejdb.BSON;

namespace Ejdb.DB
{
	public class UpdateBuilder
	{
		private const string SET_OPERATOR = "$set";

		private readonly BsonDocument _document;

		/// <summary>
		/// Initializes a new instance of the UpdateBuilder class.
		/// </summary>
		public UpdateBuilder()
		{
			_document = new BsonDocument();
		}
		
        internal BsonDocument Document
        {
            get { return _document; }
        }

		/// <summary>
		/// Sets the value of the named element to a new value (see $set).
		/// </summary>
		/// <param name="name">The name of the element to be set.</param>
		/// <param name="value">The new value.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public UpdateBuilder Set(string name, object value)
		{
			if (name == null) 
				throw new ArgumentNullException("name");
			
			if (value == null) 
				throw new ArgumentNullException("value");

			var element = _document[SET_OPERATOR] as BsonDocument;
			if (element == null)
			{
				element = new BsonDocument();
				_document.Add(SET_OPERATOR, element);
			}
			
			element.Add(name, value);
			return this;
		} 
	}
}