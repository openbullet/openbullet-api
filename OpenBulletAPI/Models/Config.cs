using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenBulletAPI.Models
{
    public class Config
    {
        [BsonId]
        public string Id { get; set; }

        [BsonField("Name")]
        public string Name { get; set; }

        [BsonField("Code")]
        public string Code { get; set; }

        [BsonField("Whitelist")]
        public string[] Whitelist { get; set; }
    }
}
