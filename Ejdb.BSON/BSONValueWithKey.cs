// // Copyright © Anton Paar GmbH, 2004-2013
//  
namespace Ejdb.BSON
{
    public class BSONValueWithKey
    {
        public BSONValueWithKey(string key, BSONValue value, BSONType type)
        {
            Value = value.Value;
            BSONType = type;
            Key = key;
        }

        public object Value { get; private set; }

        public BSONType BSONType { get; set; }

        public string Key { get; private set; }
        
    }
}