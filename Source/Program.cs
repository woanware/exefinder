using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace exefinder
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyName assemblyName = assembly.GetName();

                Console.WriteLine(Environment.NewLine + "exefinder v" + assemblyName.Version.ToString(3) + Environment.NewLine);

                Options options = new Options();
                if (CommandLine.Parser.Default.ParseArguments(args, options) == false)
                {
                    return;
                }

                string line = string.Empty;
                List<string> files = new List<string>();
                using (System.IO.StreamReader file = new System.IO.StreamReader(options.Input))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.EndsWith(".exe") == false)
                        {
                            continue;
                        }

                        if (files.Contains(line) == true)
                        {
                            continue;
                        }

                        files.Add(line);
                    }
                }

                files.Sort();
                foreach (string temp in files)
                {
                    WriteTextToFile(temp + Environment.NewLine, options.Output, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filename"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        private static string WriteTextToFile(string text,
                                            string filename,
                                            bool append)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filename, append, Encoding.GetEncoding(1252)))
                {
                    streamWriter.Write(text);
                }

                return string.Empty;
            }
            catch (IOException ex)
            {
                return ex.Message;
            }
            catch (UnauthorizedAccessException unex)
            {
                return unex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
