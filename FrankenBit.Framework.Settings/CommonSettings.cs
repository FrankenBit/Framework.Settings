using System;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace FrankenBit.Framework.Settings
{
    public class CommonSettings : Settings
    {
        protected CommonSettings()
        {
        }

        protected CommonSettings( [NotNull] string path )
        {
            Contract.Requires<ArgumentNullException>( path != null );
            Load( System.IO.Path.Combine( CommonSettingsDirectory.GetPath( GetType() ), path ) );
        }
    }
}