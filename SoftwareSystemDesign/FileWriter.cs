using System;
using System.IO;
using IniParser;
using System.Text;
using Newtonsoft.Json;
using IniParser.Model;
using System.Collections.Generic;

namespace SoftwareSystemDesignApp
{
    // Class for writing sequence from console to files with .ini, .xml and .json extensions
    // and creating .csv exit file
    public static class FileWriter
    {
        // Logger instance
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // Variables for files internal data path
        private const string FILES_SECTION_NAME = "DataModel";
        private const string FILES_TAGS_NAME = "Sequence";

        /// <summary>
        /// Create/rewrite sequnce result data into .csv file
        /// </summary>
        /// <param name="membersOfSequnce">Sequence result which should be stored</param>
        public static void WriteDataToCSVFile(List<double> membersOfSequnce)
        {
            log.Debug("WriteDataToCSVFile method was called.");
            StringBuilder csv = new StringBuilder();
            string fileName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            string filePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{fileName}.csv";  // Create unique path at any iteration of running

            using (FileStream fs = File.Create(filePath))
            foreach (var member in membersOfSequnce)
            {
                csv.AppendLine(member.ToString());
            }
            File.WriteAllText(filePath, csv.ToString());
            log.Info("Sequence was successfully loaded to .csv file.");
        }

        /// <summary>
        /// Choose implementation of file write method by it extension
        /// </summary>
        /// <param name="filePath">Path to file which shoul be writed</param>
        /// <returns>Data from sequence file</returns>
        static public void WriteSequenceToFile(string filePath)
        {
            log.Debug("WriteSequnceToFile method was called.");
            string extension = Path.GetExtension(filePath);
            string sequence = Calculation.GetSequence();
            try
            {
                // Choose file reader implementation by file extension
                switch (extension)
                {
                    case ".ini":
                        WriteToINI(filePath, sequence);
                        break;
                    case ".json":
                        WriteToJSON(filePath, sequence);
                        break;
                    case ".xml":
                        WriteToXML(filePath, sequence);
                        break;
                    default:
                        Console.Clear();
                        log.Warn($"Entered file extention [${extension}] is not supported.");
                        Console.WriteLine("Unknown file extencion. Please reenter file path or enter '-sf' command again to exit from this menu.");
                        break;
                }
            }
            catch(Exception exception)
            {
                log.ErrorFormat("Unknown error has occurred.", exception);
                Console.Clear();
                Console.WriteLine("Error of creating/rewriting file with sequence");
            }
        }

        /// <summary>
        /// Method for creating .json file with sequence
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="sequence">Sequence with will be stored</param>
        private static void WriteToJSON(string filePath, string sequence)
        {
            log.Debug("Method WriteToJSON was called.");
            DataModel data = new DataModel { Sequence = sequence};
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, data);
            }
            log.Info(".json file was successfully updated.");
        }

        /// <summary>
        /// Method for creating .xml file with sequence
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="sequence">Sequence with will be stored</param>
        private static void WriteToXML(string filePath, string sequence)
        {
            log.Debug("Method WriteToXML was called.");
            DataModel overview = new DataModel { Sequence = sequence};
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(DataModel));
            FileStream file = File.Create(filePath);
            writer.Serialize(file, overview);
            file.Close();
            log.Info(".xml file was successfully updated.");
        }

        /// <summary>
        /// Method for creating .ini file with sequence
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="sequence">Sequence with will be stored</param>
        private static void WriteToINI(string filePath, string sequence)
        {
            log.Debug("Method WriteToINI was called.");
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = new IniData();
            data[FILES_SECTION_NAME][FILES_TAGS_NAME] = sequence;
            parser.WriteFile(filePath, data);
            log.Info(".ini file was successfully updated.");
        }
    }

    // Data-model-writer class
    // Class-helper for creating correct data format for parsers
    public class DataModel
    {
        public string Sequence { get; set; }
    }
}
