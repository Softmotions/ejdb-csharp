namespace Ejdb.DB
{
    public class PartialQuery<TMember> : IPartialQuery
    {
        private TMember _comparisonValue;

        public static PartialQuery<TMember> Create(string queryOperator, TMember comparisonValue)
        {
            return new PartialQuery<TMember>(queryOperator, comparisonValue);
        }

        private PartialQuery(string queryOperator, TMember comparisonValue)
        {
            QueryOperator = queryOperator;
            _comparisonValue = comparisonValue;
        }

        public string QueryOperator { get; private set; }

        public object ComparisonValue
        {
            get { return _comparisonValue; }
        }
    }
}