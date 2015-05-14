using System;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace FrankenBit.Framework.Settings
{
    internal class SettingInfo
    {
        [CanBeNull]
        public string DeclaringType { get; set; }

        [CanBeNull]
        public string Name { get; set; }

        [CanBeNull]
        public Type Type
        {
            get { return TypeName != null ? Type.GetType( TypeName ) : null; }
        }

        [CanBeNull]
        public string TypeName { get; set; }

        public bool Matches( [NotNull] Setting setting )
        {
            Contract.Requires<ArgumentNullException>( setting != null );

            return setting.DeclaringType == DeclaringType &&
                   setting.Name == Name &&
                   setting.Type.IsAssignableFrom( Type );
        }

        public override string ToString()
        {
            return string.Format( "{0} {1}.{2}", TypeName, DeclaringType, Name );
        }
    }
}