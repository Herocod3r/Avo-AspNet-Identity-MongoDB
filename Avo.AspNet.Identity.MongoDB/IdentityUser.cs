using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Avo.AspNet.Identity.MongoDB
{
    public class IdentityUser : IUser<string>
    {
        public IdentityUser(string username)
        {
            UserName = username;
        }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string UserName { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string SecurityStamp { get; set; }
        public virtual bool TwoFactorEnabled { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        public virtual int AccessFailedCount { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual List<IdentityUserClaim> Claims { get; set; } = new List<IdentityUserClaim>();
        public virtual List<string> Roles { get; set; } = new List<string>();
        public virtual List<UserLoginInfo> Logins { get; set; } = new List<UserLoginInfo>();
    }
}
