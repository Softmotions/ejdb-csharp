using System;
using Ejdb.BSON;

namespace Ejdb.DB
{
	public class UpdateBuilder
	{
		private const string SET_OPERATOR = "$set";
		private const string INC_OPERATOR = "$inc";
		private const string ADD_TO_SET_OPERATOR = "$addToSet";
		private const string ADD_TO_SET_ALL_OPERATOR = "$addToSetAll";
		private const string PULL_OPERATOR = "$pull";
		private const string PULL_ALL_OPERATOR = "$pullAll";
		private const string DROP_ALL_OPERATOR = "$dropall";

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
			return _AddOperation(SET_OPERATOR, name, value);
		}

		/// <summary>
		/// Increments the named element by a value (see $inc).
		/// </summary>
		/// <param name="name">The name of the element to be incremented.</param>
		/// <param name="value">The value to increment by.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public UpdateBuilder Inc(string name, object value)
		{
			return _AddOperation(INC_OPERATOR, name, value);
		}

		/// <summary>
		/// Removes all values from the named array element that are equal to some value (see $pull).
		/// </summary>
		/// <param name="name">The name of the array element.</param>
		/// <param name="value">The value to remove.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public UpdateBuilder Pull(string name, object value)
		{
			return _AddOperation(PULL_OPERATOR, name, value);
		}

		/// <summary>
		/// Removes all values from the named array element that are equal to any of a list of values (see $pullAll).
		/// </summary>
		/// <param name="name">The name of the array element.</param>
		/// <param name="values">The values to remove.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public UpdateBuilder PullAll<T>(string name, params T[] values)
		{
			return _AddOperation(PULL_ALL_OPERATOR, name, values);
		}

		// public methods
        /// <summary>
        /// Adds a value to a named array element if the value is not already in the array (see $addToSet).
        /// </summary>
        /// <param name="name">The name of the array element.</param>
        /// <param name="value">The value to add to the set.</param>
        /// <returns>The builder (so method calls can be chained).</returns>
        public UpdateBuilder AddToSet(string name, object value)
        {
	        return _AddOperation(ADD_TO_SET_OPERATOR, name, value);
        }

		/// <summary>
		/// Adds a list of values to a named array element adding each value only if it not already in the array (see $addToSet and $each).
		/// </summary>
		/// <param name="name">The name of the array element.</param>
		/// <param name="values">The values to add to the set.</param>
		/// <returns>The builder (so method calls can be chained).</returns>
		public UpdateBuilder AddToSetAll<T>(string name, params T[] values)
		{
			return _AddOperation(ADD_TO_SET_ALL_OPERATOR, name, values);
		}

		/// <summary>
		/// In-place record removal operation ($dropall).
		/// </summary>
		/// <returns>The builder (so method calls can be chained).</returns>
		public UpdateBuilder DropAll()
		{
			var element = _document[DROP_ALL_OPERATOR] as BsonDocument;
			if (element == null)
				_document.Add(DROP_ALL_OPERATOR, BsonValue.Create(true));

			return this;
		}

		private UpdateBuilder _AddOperation(string operationName, string name, object value)
		{
			Verify.NotNull(name, "name");
			Verify.NotNull(value, "value");

			var element = _GetOrCreateElement(operationName);
			element.Add(name, value);

			return this;
		}

		private BsonDocument _GetOrCreateElement(string operationName)
		{
			var element = _document[operationName] as BsonDocument;
			if (element == null)
			{
				element = new BsonDocument();
				_document.Add(operationName, element);
			}
			return element;
		}
	}
}