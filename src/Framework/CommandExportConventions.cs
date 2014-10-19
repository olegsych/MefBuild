using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Linq;
using System.Reflection;

namespace MefBuild
{
    /// <summary>
    /// Ensures that types derived from <see cref="Command"/> are exported with correct contract type and metadata.
    /// </summary>
    /// <remarks>
    /// Implementation is based on MEF's DirectAttributeContext class.
    /// <a href="http://mef.codeplex.com/SourceControl/latest#oob/src/System.Composition.TypedParts/TypedParts/Util/DirectAttributeContext.cs"/>
    /// </remarks>
    internal class CommandExportConventions : AttributedModelProvider
    {
        public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, ParameterInfo parameter)
        {
            if (reflectedType == null)
            {
                throw new ArgumentNullException("reflectedType");
            }

            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            return parameter.GetCustomAttributes();
        }

        public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, MemberInfo member)
        {
            if (reflectedType == null)
            {
                throw new ArgumentNullException("reflectedType");
            }

            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            return GetAttributesOfTypeOrMember(reflectedType, member);
        }

        private static IEnumerable<Attribute> GetAttributesOfTypeOrMember(Type reflectedType, MemberInfo typeOrMember)
        {
            var type = typeOrMember as TypeInfo;
            if (type != null)
            {
                return GetAttributesOfType(reflectedType, type);
            }

            return GetAttributesOfMember(reflectedType, typeOrMember);
        }

        private static IEnumerable<Attribute> GetAttributesOfMember(Type reflectedType, MemberInfo member)
        {
            if (member.DeclaringType == reflectedType)
            {
                return member.GetCustomAttributes();
            }

            return Enumerable.Empty<Attribute>();
        }

        private static IEnumerable<Attribute> GetAttributesOfType(Type reflectedType, TypeInfo type)
        {
            var attributes = new List<Attribute>(type.GetCustomAttributes());

            if (attributes.OfType<ExportAttribute>().Any(a => a.ContractType == typeof(Command)))
            {
                attributes.Add(new ExportMetadataAttribute("CommandType", reflectedType));
            }

            return attributes;
        }
    }
}
