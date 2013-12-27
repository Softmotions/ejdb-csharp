namespace Ejdb.DB
{
    public interface IPartialQuery
    {
        string QueryOperator { get; }
        object ComparisonValue { get; }
    }
}