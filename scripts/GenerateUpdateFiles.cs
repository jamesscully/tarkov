
using System;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO.Compression;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;


namespace TarkovAssistantWPF.scripts
{
    /// <summary>
    /// This class will generate the files requires for AutoUpdater.NET and output them to $root/updates
    /// These files can be copied straight to the server
    /// </summary>
    public class GenerateUpdateFiles
    {

        private static string DIR_OUTPUT = @"../updates/";
        private static string PATH_EXE = Path.GetFullPath("../bin/Debug/TarkovAssistant.exe");
        private static string ZIP_NAME = "";
        private static string ZIP_OUT_PATH = "";
        private static string XML_OUT_PATH = Path.GetFullPath(DIR_OUTPUT + "update_info.xml");


        private static void Main(string[] args)
        {
            WriteLog("Beginning construction of update files");

            WriteLog("General output path: " + Path.GetFullPath(DIR_OUTPUT));

            var xml = XDocument.Load("https://s3.eu-west-2.amazonaws.com/jwscully.uk/tarkov-assistant/update_info.xml");

            // get the remote version to compare against
            var remoteVersion = xml.XPathSelectElement("/item/version").Value;

            Console.Out.WriteLine("Main: Found remote version: " + remoteVersion);

            // create output folder if we dont have it
            if (!Directory.Exists(DIR_OUTPUT))
                Directory.CreateDirectory(DIR_OUTPUT);

            try
            {
                WriteLog("Main: Cleaning up past updates");
                var enumerator = Directory.EnumerateFiles(DIR_OUTPUT).GetEnumerator();

                do
                {
                    if(enumerator.Current != null)
                        File.Delete(enumerator.Current);
                } while (enumerator.MoveNext());
            }
            catch (Exception e)
            {
                WriteError(e.ToString());
            }

            try
            {
                var assembly = Assembly.LoadFile(PATH_EXE);
                var version = assembly.GetName().Version;

                Console.Out.WriteLine("Main: Found version: " + version);


                // if we can't parse the remoteVersion, something spooky has happened.
                if (!Version.TryParse(remoteVersion, out Version result))
                {
                    throw new Exception("Could not parse remoteVersion: " + remoteVersion);
                }
                else
                {
                    // if the remote version is greater than ours, we can't send this XML file.
                    if (result > version)
                    {
                        WriteError("Remote version is greater than our version; change minor version?");
                        return;
                    }
                }

                // build the zip file first, then we can reference this in our .xml 
                // to easily send to AWS S3
                GenerateZipFile(PATH_EXE);
                GenerateXMLFile(version.ToString());

                // UploadToAmazon().Wait();
            }
            catch (Exception e)
            {
                WriteError("Could not find assembly file!");
                WriteError(e.ToString());
            }

        }

        // Generates and outputs update.xml file for AutoUpdater.NET to $root/updates/
        private static void GenerateXMLFile(string version)
        {

            // if we have no zip name, then we cannot append this to the url
            if (!(ZIP_NAME.Length > 0))
            {
                WriteError("No zip name has been found! Can't append to XML");
            }

            string DOWNLOAD_URL = "https://s3.eu-west-2.amazonaws.com/jwscully.uk/tarkov-assistant/" + ZIP_NAME;

            WriteLog("GenerateXMLFile: Using download URL: " + DOWNLOAD_URL);

            using (XmlWriter writer = XmlWriter.Create(XML_OUT_PATH))
            {
                writer.WriteStartElement("item");
                writer.WriteElementString("version", version);
                writer.WriteElementString("url", DOWNLOAD_URL);
                writer.WriteElementString("changelog", "");
                writer.WriteElementString("mandatory", "true");
                writer.WriteEndElement();
                writer.Flush();
            }
        }

        private static void GenerateZipFile(string executable_path)
        {

            WriteLog("GenerateZipFile: Executable path: " + executable_path);

            // we will put our exe here to be zipped up; only the .exe needs to be distributed to the user
            // note: ZipFile does not seem to allow just one file to be zipped, so we'll use this folder
            var STAGING_DIR = Path.GetFullPath(DIR_OUTPUT) + "staging";

            try
            {

                WriteLog("GenerateZipFile: EXE staging dir: " + STAGING_DIR);

                // create our staging directory if we don't have one
                if (!Directory.Exists(STAGING_DIR))
                    Directory.CreateDirectory(STAGING_DIR);


                // append timestamp to basic name
                ZIP_NAME = "update-" + DateTime.Now.ToString("yyyy-MM-ddHHmmss") + ".zip";

                // the end destination of our update zip 
                ZIP_OUT_PATH = Path.GetFullPath("../updates/" + ZIP_NAME);

                // set global var to let XML doc know the updates name

                // copy our executable to staging area
                WriteLog("GenerateZipFile: Copying exe to: " + STAGING_DIR);
                File.Copy(executable_path, STAGING_DIR + "/" + "TarkovAssistant.exe", true);

                // zip the staging dir
                WriteLog("GenerateZipFile: Writing archive to: " + ZIP_OUT_PATH);
                ZipFile.CreateFromDirectory(STAGING_DIR, ZIP_OUT_PATH);
            }
            catch (Exception e)
            {
                WriteError(e.ToString());
                return;
            }
            finally
            {
                // clean up staging folder
                Directory.Delete(STAGING_DIR + "/", true);
            }
            
        }

        private static void WriteLog(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Out.WriteLine(message);
            Console.ResetColor();
        }
        private static void WriteError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Out.WriteLine(errorMessage);
            Console.ResetColor();
        }
    }
}