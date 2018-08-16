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
            int id = 0;
            var rootpath = System.Environment.CurrentDirectory;
            var filesPath = "C:\\linted\\";
            var directories = Directory.GetDirectories(filesPath);

            // Initialize Final List
            finalList = new List<AppData>();
            dataAsStringList = new List<string>();

            // Iterate over folder
            foreach (var dir in directories)
            {
                Environment.CurrentDirectory = dir;

                var res = Directory.GetFiles(".\\", "lint-result.xml", System.IO.SearchOption.AllDirectories);

                if (res.Length == 0)
                    continue;

                string[] appSplit = new string[] { "\\" };
                string[] splitPath = res[0].Split(appSplit, System.StringSplitOptions.RemoveEmptyEntries);

                string appName;
                if (splitPath.Length > 2)
                    appName = dir.Split(appSplit, System.StringSplitOptions.RemoveEmptyEntries)[2].Replace("_src", "");
                else
                    appName = dir.Split(appSplit, System.StringSplitOptions.RemoveEmptyEntries)[2].Replace("_src", "");

                if(appName.Contains("tar.gz"))
                     appName = appName.Replace("_src.tar.gz", "");

                if(appName.Contains("-master"))
                    appName = appName.Replace("-master", "");


                XmlDocument doc;
                // Make and Open XML Doc
                try
                {
                     doc = new XmlDocument();
                     doc.Load(res[0]);
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
                    data.AppID = id;
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
                    string dataAsString = dat.AppID + ";" + dat.AppName + ";" + dat.SmellID + ";" + dat.Line + ";" + dat.Method + ";" + dat.AffectedClass + ";" + dat.Package;
                    dataAsStringList.Add(dataAsString);
                }
                // Parse each individual issue Data
                id += 1;
            }

            Environment.CurrentDirectory = rootpath;

            File.WriteAllLines(rootpath + "\\output.txt", dataAsStringList);
            // Rename .txt file to .csv file when this is finished, it can then be opened in Excel/Other Office applications with proper columns/rows.
            Console.WriteLine("Finished. Press any Key to Exit...");
            Console.ReadKey();
        }
    }
}
