using System.Collections.Generic;
using System.Reflection;
using JsonFx.Json.Resolvers;
using JsonFx.Serialization;

namespace EasyHttp.Codecs.JsonFXExtensions
{
    using System;

    public class RemoveAmpersandFromNameJsonResolverStrategy : JsonResolverStrategy
    {
        public override IEnumerable<DataName> GetName(MemberInfo member)
        {
            if (!member.Name.StartsWith("@", StringComparison.InvariantCulture))
                return base.GetName(member);

            string nameWithoutAmpersand = member.Name.Remove(0);
            return new List<DataName> 
            {
                new DataName(nameWithoutAmpersand) 
            };
        }
    }
}