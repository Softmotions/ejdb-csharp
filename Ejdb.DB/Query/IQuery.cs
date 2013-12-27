// // Copyright © Anton Paar GmbH, 2004-2013
//  

using Ejdb.BSON;

namespace Ejdb.DB
{
    public interface IQuery
    {
        BsonDocument GetQueryDocument();
    }
}