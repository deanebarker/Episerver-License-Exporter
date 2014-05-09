using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Xml;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;
using System.Configuration;
using InsightlyApi.InsightlyObjects;
using log4net;
using log4net.Appender;

namespace InsightlyApi
{
    internal class Program
    {
        public static SqlConnection Connection;
        public static ILog Log;
        private static Dictionary<string, int> Counter = new Dictionary<string,int>();

        private static void Main(string[] args)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                Log = LogManager.GetLogger("root");
                Log.Debug("Start");
                Output("Start");

                // Get some basic config settings
                var serviceUrl = ConfigurationManager.AppSettings["serviceUrl"];
                var apiKey = ConfigurationManager.AppSettings["apiKey"];
                var url = ConfigurationManager.AppSettings["serviceUrl"];
                var toDownload = ConfigurationManager.AppSettings["toDownload"].Split(',').Select(s => s.Trim()).ToArray();
                var resetFile = "reset.sql";

                Output("Read configuration information.");

                // Open a Connection (all database interaction runs off this same Connection...)
                try
                {
                    Connection = new SqlConnection(ConfigurationManager.ConnectionStrings[1].ConnectionString);
                    Connection.Open();
                }
                catch (Exception e)
                {
                    Log.Error("Error opening database connection; " + e.Message + "; Connection string: " + ConfigurationManager.ConnectionStrings[1].ConnectionString);
                    throw;
                }

                Output("Opened database connection.");

                // Load the reset file.
                var resetSql = String.Empty;
                try
                {
                    resetSql = File.ReadAllText(resetFile);
                    Log.Debug("Read " + resetSql.Length + " byte(s) from \"" + resetFile + "\"");
                }
                catch (Exception e)
                {
                    Log.Error("Error reading reset file: " + resetFile + "; " + e.Message);
                    throw;
                }

                Output("Read reset file.");

                // Run the reset SQL
                try
                {
                    var resetDatabase = new SqlCommand(resetSql);
                    resetDatabase.Connection = Connection;
                    resetDatabase.ExecuteNonQuery();
                    Log.Debug("Executed reset SQL.");
                }
                catch (Exception e)
                {
                    Log.Error("Error executing reset SQL; " + e.Message);
                    throw;
                }

                Output("Executed reset SQL.");

                // Set up the client
                var encodedKey = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(apiKey));
                var client = new WebClient();
                
                // Loop through each object type we want to download
                foreach (var download in toDownload)
                {
                    Output("Downloading: " + download);

                    // This header resets after every call, so it has to be reset each time...
                    client.Headers.Add("content-type", "text/xml");

                    // Get the XML
                    var xmlString = String.Empty;
                    try
                    {
                        xmlString = client.DownloadString(String.Format(serviceUrl,download,apiKey));
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error downloading XML from " + String.Concat(url, download) + "; " + e.Message);
                        Output("Error. Skipping: " + download);
                        continue;
                    }

                    Output("Downloaded: " + download);

                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data"));
                    File.WriteAllText("data/" + download + ".xml", xmlString);

                    // Parse the XML
                    var xml = new XmlDocument();
                    try
                    {
                        xml.LoadXml(xmlString);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error parsing returned XML; " + e.Message);
                    }


                    // Loop through every top-level node in the XML, which each represents a single object
                    foreach (XmlElement objectNode in xml.SelectNodes("//Records/*"))
                    {
                        if (Counter.ContainsKey(objectNode.Name))
                        {
                            Counter[objectNode.Name]++;
                        }
                        else
                        {
                            Counter.Add(objectNode.Name, 1);
                        }

                        // Do we have a custom type for this object?
                        var mappingType = Assembly.GetExecutingAssembly().GetTypes().Where(
                            t => t.GetCustomAttributes(typeof(TableMappingAttribute), true).Where(
                                a => ((TableMappingAttribute)a).ObjectName.ToLower() == download.ToLower()).Any()).
                                    FirstOrDefault();

                        DataObject mappingObject;
                        if (mappingType != null)
                        {
                            // We have a custom type, instantiate it...
                            mappingObject = Activator.CreateInstance(mappingType, new object[] { objectNode }) as DataObject;
                        }
                        else
                        {
                            // Just use the default object
                            mappingObject = new DataObject(objectNode);
                        }

                        Output("Processing object ID: " + mappingObject.Id);


                        Log.Debug("Created object for " + objectNode.Name + "; ID: " + mappingObject.Id + "; Object Type: " + mappingObject.GetType().ToString());

                        // Save it
                        mappingObject.Save();

                        Output("Saved object ID: " + mappingObject.Id);
                    }
                }

                // Clean up
                Connection.Close();
                stopWatch.Stop();

                Log.Debug("End. " + stopWatch.Elapsed.TotalSeconds + "s");
                Output("End. " + stopWatch.Elapsed.TotalSeconds + "s");

                foreach (var counter in Counter)
                {
                    Program.Log.Debug(counter.Key + ": " + counter.Value);
                }
            }
            catch (Exception e)
            {
                Program.Log.Equals("Fatal Exception: " + e.Message);
                Console.WriteLine("Fatal Exception: " + e.Message);
            }
        }

        private static void Output(string text)
        {
            Console.WriteLine(text);
        }
    }
}
