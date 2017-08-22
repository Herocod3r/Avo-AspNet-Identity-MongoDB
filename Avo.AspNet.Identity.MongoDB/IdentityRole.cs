using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Avo.AspNet.Identity.MongoDB
{
    public class IdentityRole : IRole<string>
    {
        public IdentityRole(string name)
        {
            Name = name;
        }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
