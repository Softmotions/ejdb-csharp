namespace Ejdb.DB
{
    public class PartialQuery : IPartialQuery
    {
        public static PartialQuery Create(string queryOperator, object comparisonValue)
        {
            return new PartialQuery(queryOperator, comparisonValue);
        }

        private PartialQuery(string queryOperator, object comparisonValue)
        {
            QueryOperator = queryOperator;
            ComparisonValue = comparisonValue;
        }

        public string QueryOperator { get; private set; }
        public object ComparisonValue { get; private set; }
    }
}