using Ejdb.BSON;

namespace Ejdb.DB
{
	internal class EmptyQuery : IQuery
	{
		private EmptyQuery()
		{ }

		public static EmptyQuery Instance = new EmptyQuery();

		public BsonDocument Document
		{
			get { return new BsonDocument(); }
		}
	}
}