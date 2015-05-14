using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NLog;

namespace FrankenBit.Framework.Settings
{
    public static class SettingsReader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [NotNull]
        public static T Read<T>( [NotNull] Stream input ) where T : new()
        {
            Contract.Requires<ArgumentNullException>( input != null );

            var settings = new T();
            Read( input, settings );
            return settings;
        }

        public static bool Read( [NotNull] Stream input, [NotNull] object settings )
        {
            Contract.Requires<ArgumentNullException>( input != null );
            Contract.Requires<ArgumentNullException>( settings != null );

            try
            {
                using ( XmlReader reader = XmlReader.Create( input ) ) Read( reader, settings );
                return true;
            }
            catch ( Exception e )
            {
                Logger.WarnException( string.Format( "Unable to read '{0}' configuration from input stream: {1}",
                    settings.GetType().FullName, e ), e );
                return false;
            }
        }

        private static void Read( [NotNull] XmlReader reader, [NotNull] object settingsInstance )
        {
            Contract.Requires( reader != null );
            Contract.Requires( settingsInstance != null );

            if ( !reader.ReadToFollowing( SettingsXml.SettingsKey ) ) return;

            List<Setting> settings = SettingsScanner.Scan( settingsInstance ).ToList();

            while ( reader.ReadToFollowing( SettingsXml.GroupKey ) )
            {
                var info = new SettingInfo
                {
                    DeclaringType = reader.GetAttribute( SettingsXml.TypeKey )
                };

                while ( reader.ReadToFollowing( SettingsXml.SettingKey ) )
                {
                    info.Name = reader.GetAttribute( SettingsXml.NameKey );
                    info.TypeName = reader.GetAttribute( SettingsXml.TypeKey );

                    Setting setting = settings.Find( info.Matches );
                    if ( setting == null || !reader.Read() || info.Type == null ) continue;

                    try
                    {
                        var serializer = new XmlSerializer( info.Type );
                        setting.Value = serializer.Deserialize( reader );
                    }
                    catch ( InvalidOperationException e )
                    {
                        Logger.WarnException(
                            string.Format( "Unable to read '{0}' from settings stream: {1}", info, e ), e );
                    }
                }
            }
        }
    }
}