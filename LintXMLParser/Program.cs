using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LintXMLParser
{
    class Program
    {
        private static List<AppData> finalList; // Final List of all Data
        private static List<string> dataAsStringList;


        static void Main(string[] args)
        {
            var rootpath = System.Environment.CurrentDirectory;
            var directories = Directory.GetDirectories(rootpath);

            // Initialize Final List
            finalList = new List<AppData>();
            dataAsStringList = new List<string>();

            // Iterate over folder
            foreach (var dir in directories)
            {
                Environment.CurrentDirectory = dir;

                string[] appSplit = new string[] { "\\" };
                string[] splitPath = dir.Split(appSplit, System.StringSplitOptions.RemoveEmptyEntries);
                string appName = splitPath[splitPath.Length - 1];

                if(appName.Contains("tar.gz"))
                     appName = appName.Replace("_src.tar.gz", "");

                if(appName.Contains("-master"))
                    appName = appName.Replace("-master", "");


                XmlDocument doc;
                // Make and Open XML Doc
                try
                {
                     doc = new XmlDocument();
                     doc.Load(".\\lint-result.xml");
                }
                catch (Exception e)
                {
                    continue;
                }
                

                // Extract List of all issues
                XmlNodeList allIssues = doc.SelectNodes("/issues/issue");

                // List of all Issue Data
                List<AppData> appDataList = new List<AppData>();

                // Parse each issue node
                foreach (XmlNode node in allIssues)
                {
                    if (node.Attributes["category"].InnerText != "Security")
                        continue;

                    if (!node.Attributes["summary"].InnerText.Contains("SM"))
                        continue;

                    AppData data = new AppData();
                    data.AppName = appName;
                    string smellID = node.Attributes["summary"].InnerText;
                    string[] smellAray = smellID.Split(':');
                    data.SmellID = smellAray[0];

                    if (node["location"].Attributes["file"].InnerText.Contains("AndroidManifest")){
                        data.AffectedClass = "AndroidManifest.xml";
                        data.Line = node["location"].Attributes["line"].InnerText;
                        data.Package = "N/A";
                        data.Method = "N/A";
                        // Add to List.
                        appDataList.Add(data);
                        continue;
                    }

                    

                    string message = node.Attributes["message"].InnerText;
                    string[] split = new string[] { "--" };
                    string[] splitMessage = message.Split(split, System.StringSplitOptions.RemoveEmptyEntries);

                    string[] components;

                    try
                    {
                        components = splitMessage[1].Split(',');
                    }
                    catch (Exception e)
                    {
                        continue;
                    }



                    data.Line = node["location"].Attributes["line"].InnerText;
                    data.Package = components[1].Split(':')[1].Trim();
                    data.Method = components[2].Split(':')[1].Trim();
                    data.AffectedClass = components[3].Split(':')[1].Trim();


                    // Add to List.
                    appDataList.Add(data);
                }

                foreach (AppData dat in appDataList)
                {
                    string dataAsString = dat.AppName + ";" + dat.SmellID + ";" + dat.Line + ";" + dat.Method + ";" + dat.AffectedClass + ";" + dat.Package;
                    dataAsStringList.Add(dataAsString);
                }
                // Parse each individual issue Data
                
            }

            Environment.CurrentDirectory = rootpath;

            File.WriteAllLines(rootpath + "output.txt", dataAsStringList);
            // Rename .txt file to .csv file when this is finished, it can then be opened in Excel/Other Office applications with proper columns/rows.
            Console.WriteLine("Finished. Press any Key to Exit...");
            Console.ReadKey();
        }
    }
}