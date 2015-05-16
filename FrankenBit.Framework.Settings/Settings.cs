using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using NLog;

namespace FrankenBit.Framework.Settings
{
    public abstract class Settings
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Setting[] _settings;

        protected Settings()
        {
            _settings = Scan( this ).ToArray();
        }

        protected Settings( [NotNull] string path )
            : this()
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrWhiteSpace( path ) );
            Load( path );
        }

        public string Path { get; set; }

        public bool Changed
        {
            get { return _settings.Any( s => s.Changed ); }
        }

        public void Load()
        {
            if ( !string.IsNullOrWhiteSpace( Path ) ) Load( Path );
            else Reset();
        }

        public bool Load( [NotNull] string path )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrWhiteSpace( path ) );

            Path = path;
            if ( !File.Exists( path ) )
            {
                Logger.Warn( "Unable to load '{0}' settings from '{1}': file not found.",
                    GetType().FullName, path );
                return false;
            }

            using ( Stream input = new FileStream( path, FileMode.Open, FileAccess.Read ) )
            {
                SettingsReader.Read( input, this );
            }

            foreach ( Setting setting in _settings ) setting.Reload();
            return true;
        }

        public void Reset()
        {
            foreach ( Setting setting in _settings ) setting.Reset();
        }

        public void Save()
        {
            if ( string.IsNullOrWhiteSpace( Path ) ) throw new InvalidOperationException();
            SaveAs( Path );

            foreach ( Setting setting in _settings ) setting.Reload();
        }

        public void Save( [NotNull] string path )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrWhiteSpace( path ) );
            Path = path;
            Save();
        }

        public void SaveAs( [NotNull] string path )
        {
            using ( Stream output = new FileStream( path, FileMode.Truncate ) ) SettingsWriter.Write( output, this );
        }

        [NotNull]
        internal static IEnumerable<Setting> Scan( [NotNull] object instance )
        {
            Contract.Requires( instance != null );

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            Type settingsType = typeof( Settings );

            return instance.GetType().GetProperties( flags )
                .Where( p => p.CanWrite && p.DeclaringType != settingsType )
                .Select( p => new Setting( instance, p ) );
        }
    }
}