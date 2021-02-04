
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

        private static string DIR_OUTPUT = "../updates/";
        private static string PATH_EXE = Path.GetFullPath("../bin/Debug/TarkovAssistant.exe");
        private static string ZIP_NAME = "";
        private static string ZIP_OUT_PATH = "";
        private static string XML_OUT_PATH = Path.GetFullPath(DIR_OUTPUT + "update_info.xml");


        private static int Main(string[] args)
        {
            WriteLog("Beginning construction of update files");

            WriteLog("General output path: " + Path.GetFullPath(DIR_OUTPUT));

            var xml = XDocument.Load("https://s3.eu-west-2.amazonaws.com/jwscully.uk/tarkov-assistant/update_info.xml");

            // get the remote version to compare against
            var remoteVersion = xml.XPathSelectElement("/item/version").Value;

            Console.Out.WriteLine("Main: Found remote version: " + remoteVersion);

            // create output folder if we dont have it
            if (!Directory.Exists(DIR_OUTPUT))
            {
                WriteLog("No output folder found, creating");
                Directory.CreateDirectory(DIR_OUTPUT);
            }
                

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
                        return -1;
                    }
                }

                // build the zip file first, then we can reference this in our .xml 
                // to easily send to AWS S3
                // if either of these fail, we need to terminate!
                if (GenerateZipFile(PATH_EXE) != 0 || GenerateXMLFile(version.ToString()) != 0)
                    return -1;


                AddChangeEntry(version);
            }
            catch (Exception e)
            {
                WriteError("Could not find assembly file!");
                WriteError(e.ToString());
                return -1;
            }

            return 0;
        }

        // Generates and outputs update.xml file for AutoUpdater.NET to $root/updates/
        private static int GenerateXMLFile(string version)
        {

            // if we have no zip name, then we cannot append this to the url
            if (!(ZIP_NAME.Length > 0))
            {
                WriteError("No zip name has been found! Can't append to XML");
                return -1;
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

            return 0;
        }

        private static int GenerateZipFile(string executable_path)
        {

            if (!File.Exists(executable_path))
            {
                WriteError("GenerateZipFile: Could not locate executable at: " + executable_path);
            }
            else
            {
                WriteLog("GenerateZipFile: Found executable: " + executable_path, true);
            }

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
                ZIP_NAME = "update.zip";

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
                return -1;
            }
            finally
            {
                // clean up staging folder
                Directory.Delete(STAGING_DIR + "/", true);
            }

            return 0;
        }

        private static void AddChangeEntry(Version version)
        {
            string changelog_path = DIR_OUTPUT + "changes.txt";

            WriteLog("AddChangeEntry: Opening " + changelog_path);

            if (!File.Exists(changelog_path))
            {
                WriteLog("AddChangeEntry: Changelog not found, creating: " + changelog_path);
                File.Create(changelog_path);
            }

            try
            {
                StreamWriter writer = new StreamWriter(File.Open(changelog_path, FileMode.Open));
                writer.WriteLine("-------------------------------");
                writer.WriteLine("Changes for version " + version);
                writer.Write("\n\n\n"); 
            }
            catch (IOException e)
            {
                WriteError("Error writing to changes.txt: " + e.ToString());
            }
        }

        private static void WriteLog(string message, bool highlight = false)
        {
            Console.ForegroundColor = ConsoleColor.Blue;

            if (highlight)
                Console.ForegroundColor = ConsoleColor.DarkMagenta;

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