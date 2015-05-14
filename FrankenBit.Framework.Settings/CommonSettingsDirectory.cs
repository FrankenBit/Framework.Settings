using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using JetBrains.Annotations;

namespace FrankenBit.Framework.Settings
{
    public static class CommonSettingsDirectory
    {
        [NotNull]
        public static string GetPath( [NotNull] Type type )
        {
            Contract.Requires<ArgumentNullException>( type != null );
            Contract.Ensures( Contract.Result<string>() != null );

            string baseFolder = Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData,
                Environment.SpecialFolderOption.Create );
            Assembly assembly = type.Assembly;
            string company = GetAttribute<AssemblyCompanyAttribute>( assembly, a => a.Company );
            string product = GetAttribute<AssemblyProductAttribute>( assembly, a => a.Product );
            string version = GetAttribute<AssemblyInformationalVersionAttribute>( assembly,
                a => a.InformationalVersion );

            return System.IO.Path.Combine( baseFolder, company, product, version );
        }

        [NotNull]
        private static string GetAttribute<T>( [NotNull] Assembly assembly, [NotNull] Func<T, string> extract )
            where T : Attribute
        {
            var attribute = assembly.GetCustomAttribute<T>();
            return attribute != null ? extract( attribute ) : string.Empty;
        }
    }
}