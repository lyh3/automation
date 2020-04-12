using System;
using System.IO;

using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public static class MongoDBExtensions
    {
        public static string MongoSerialize(this object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var bsonWriter = BsonWriter.Create(stringWriter))
                {
                    BsonSerializer.Serialize(bsonWriter, obj.GetType(), obj);
                }
                var trim = new[] { '[', ']' };
                return stringWriter.ToString().TrimStart(trim).TrimEnd(trim);
            }
        }

        public static dynamic MongoDesrialize(string json, Type type)
        {
            return BsonSerializer.Deserialize(json, type);
        }

        public static dynamic EntityConverter(this MongoCursor cursor, Type type)
        {
            return MongoDesrialize(MongoSerialize(cursor), type);
        }
    }
}
