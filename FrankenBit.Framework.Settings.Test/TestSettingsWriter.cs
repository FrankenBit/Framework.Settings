using System;
using System.IO;
using NUnit.Framework;

namespace FrankenBit.Framework.Settings.Test
{
    [TestFixture]
    public class TestSettingsWriter
    {
        private const string Reference =
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>\r\n" +
            "<settings>\r\n" +
            "  <group type=\"FrankenBit.Framework.Settings.Test.TestSettingsWriter\">\r\n" +
            "    <setting name=\"Setting1\" type=\"System.String\" />\r\n" +
            "  </group>\r\n" +
            "</settings>";

        public string Setting1 { get; set; }

        [Test]
        public void TestSave()
        {
            using ( var stream = new MemoryStream() )
            {
                SettingsWriter.Write( stream, this );
                stream.Seek( 0, SeekOrigin.Begin );

                using ( var reader = new StreamReader( stream ) )
                {
                    string settings = reader.ReadToEnd();
                    Console.WriteLine( settings );

                    Assert.AreEqual( Reference, settings );
                }
            }
        }
    }
}
