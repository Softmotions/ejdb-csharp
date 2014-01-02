using System;
using System.Linq;
using Ejdb.BSON;

namespace Ejdb.DB
{
	public abstract class QueryBuilderBase<TThis> : IQuery
		where TThis: IQuery
	{
		private readonly BsonDocument _document;

		protected abstract TThis This();

		public QueryBuilderBase()
		{
			_document = new BsonDocument();
		}

		public BsonDocument Document
		{
			get { return _document; }
		}

		public TThis Join(string fieldName, string foreignCollectionName)
		{
			var doDocument = _GetOrCreateElement("$do", () => new BsonDocument());

			var innerMostDoc = new BsonDocument()
				.Add("$join", foreignCollectionName);

			doDocument.Add(fieldName, innerMostDoc);
			return This();
		}

		public TThis EqualsIgnoreCase(string fieldName, string value)
		{
			return _BinaryQuery("$icase", fieldName, value);
		}

		public TThis EQ(string fieldName, object value)
		{
			_document[fieldName] = value;
			return This();
		}

		public TThis Exists(string fieldName)
		{
			return _BinaryQuery("$exists", fieldName, true);
		}

		public TThis NotExists(string fieldName)
		{
			return _BinaryQuery("$exists", fieldName, false);
		}

		public TThis GT(string fieldName, object value)
		{
			return _BinaryQuery("$gt", fieldName, value);
		}

		public TThis GTE(string fieldName, object value)
		{
			return _BinaryQuery("$gte", fieldName, value);
		}

		public TThis LT(string fieldName, object value)
		{
			return _BinaryQuery("$lt", fieldName, value);
		}

		public TThis LTE(string fieldName, object value)
		{
			return _BinaryQuery("$lte", fieldName, value);
		}

		public TThis BeginsWith(string fieldName, string value)
		{
			return _BinaryQuery("$begin", fieldName, value);
		}

		public TThis EndsWith(string fieldName, string value)
		{
			return _BinaryQuery("end", fieldName, value);
		}

		public TThis In<T>(string fieldName, params T[] comparisonValues)
		{
			return _BinaryQuery("$in", fieldName, comparisonValues);
		}

		public TThis NotIn<T>(string fieldName, params T[] comparisonValues)
		{
			return _BinaryQuery("$nin", fieldName, comparisonValues);
		}

		public TThis Between<T>(string fieldName, T comparisonValue1, T comparisonValue2)
		{
			var comparisonValues = new[] { comparisonValue1, comparisonValue2 };
			return _BinaryQuery("$bt", fieldName, comparisonValues);
		}

		public TThis NotEquals(string fieldName, object comparisonValue)
		{
			return _BinaryQuery("$not", fieldName, comparisonValue);
		}

		public TThis Not(string fieldName, IPartialQuery query)
		{
			var childValue = new BsonDocument();
			childValue.Add(query.QueryOperator, query.ComparisonValue);
			return _BinaryQuery("$not", fieldName, childValue);
		}

		private TThis _BinaryQuery(string queryOperation, string fieldName, object comparisonValue)
		{
			_document[fieldName] = new BsonDocument()
				.Add(queryOperation, comparisonValue);

			return This();
		}

		public TThis StringMatchesAllTokens(string fieldName, params string[] values)
		{
			return _BinaryQuery("$strand", fieldName, values);
		}

		public TThis StringMatchesAnyTokens(string fieldName, params string[] values)
		{
			return _BinaryQuery("$stror", fieldName, values);
		}

		public TThis Or(params IQuery[] queries)
		{
			var documents = queries.Select(x => x.Document).ToArray();
			var childValue = new BsonArray(documents);
			_document["$or"] = childValue;
			return This();
		}

		public TThis ElemMatch(string fieldName, params IQuery[] queries)
		{
			var queryDocument = new BsonDocument();

			foreach (var query in queries)
			{
				foreach (var field in query.Document)
					queryDocument[field.Key] = field.Value;
			}
			
			return _BinaryQuery("$elemMatch", fieldName, queryDocument);
		}

		private T _GetOrCreateElement<T>(string elementName, Func<T> defaultValue)
		{
			var element = _document[elementName];
			if (element == null)
			{
				element = defaultValue();
				_document.Add(elementName, element);
			}
			return (T)element;
		}
	}
}