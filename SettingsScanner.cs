using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace FrankenBit.Framework.Settings
{
    internal static class SettingsScanner
    {
        [NotNull]
        public static IEnumerable<Setting> Scan( [NotNull] object instance )
        {
            Contract.Requires<ArgumentNullException>( instance != null );

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            Type settingsType = typeof( Settings );

            return instance.GetType().GetProperties( flags )
                .Where( p => p.CanWrite && p.DeclaringType != settingsType )
                .Select( p => new Setting( instance, p ) );
        }
    }
}