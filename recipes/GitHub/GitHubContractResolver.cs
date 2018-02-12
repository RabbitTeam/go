using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Rabbit.Go.GitHub
{
    public class GitHubContractResolver : DefaultContractResolver
    {
        #region Overrides of DefaultContractResolver

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.
        /// </summary>
        /// <param name="memberSerialization">The member's parent <see cref="T:Newtonsoft.Json.MemberSerialization" />.</param>
        /// <param name="member">The member to create a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for.</param>
        /// <returns>A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);

            if (jsonProperty.HasMemberAttribute)
                return jsonProperty;

            var propertyName = jsonProperty.PropertyName;
            for (var i = 1; i < propertyName.Length; i++)
            {
                var c = propertyName[i];

                bool IsUpper()
                {
                    return c >= 'A' && c <= 'Z';
                }
                if (!IsUpper())
                    continue;

                propertyName = propertyName.Insert(i, "_");
                i += 1;
            }

            jsonProperty.PropertyName = propertyName.ToLower();

            return jsonProperty;
        }

        #endregion Overrides of DefaultContractResolver
    }
}