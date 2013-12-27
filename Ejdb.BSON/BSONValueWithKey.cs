// // Copyright © Anton Paar GmbH, 2004-2013
//  
namespace Ejdb.BSON
{
    public class BsonValueWithKey
    {
        public BsonValueWithKey(string key, BsonValue value, BsonType type)
        {
            Value = value.Value;
            BsonType = type;
            Key = key;
        }

        public object Value { get; private set; }

        public BsonType BsonType { get; set; }

        public string Key { get; private set; }
        
    }
}