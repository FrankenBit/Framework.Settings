using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NLog;

namespace FrankenBit.Framework.Settings
{
    public static class SettingsWriter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Write( [NotNull] Stream output, [NotNull] object settings )
        {
            Contract.Requires<ArgumentNullException>( output != null );
            Contract.Requires<ArgumentNullException>( settings != null );

            var xmlSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "  "
            };

            using ( XmlWriter writer = XmlWriter.Create( output, xmlSettings ) )
            {
                writer.WriteStartDocument( true );
                writer.WriteStartElement( SettingsXml.SettingsKey );

                IEnumerable<IGrouping<string, Setting>> groups =
                    Setting.Scan( settings ).GroupBy( s => s.DeclaringType );

                foreach ( IGrouping<string, Setting> group in groups )
                {
                    writer.WriteStartElement( SettingsXml.GroupKey );
                    writer.WriteAttributeString( SettingsXml.TypeKey, group.Key );

                    foreach ( Setting setting in group )
                    {
                        writer.WriteStartElement( SettingsXml.SettingKey );
                        writer.WriteAttributeString( SettingsXml.NameKey, setting.Name );
                        writer.WriteAttributeString( SettingsXml.TypeKey, setting.Type.FullName );
                        if ( setting.Value != null )
                        {
                            try
                            {
                                var serializer = new XmlSerializer( setting.Type );
                                serializer.Serialize( writer, setting.Value );
                            }
                            catch ( InvalidOperationException e )
                            {
                                Logger.WarnException(
                                    string.Format( "Unable to write {0} to settings: {1}", setting, e ), e );
                            }
                        }

                        // writer.WriteValue( setting.Value );
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}