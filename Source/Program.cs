using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace exefinder
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        private static string[] _paths = new string[] 
        {
            @"\\Program Files\\[a-zA-Z\-0-9#_!&\*\(\)]*?\.", 
            @"\\Program Files \(x86\)\\[a-zA-Z\-0-9#_!&\*\(\)]*?\.",
            @"\\ProgramData\\[a-zA-Z\-0-9#_!&\*\(\)]*?\.",
            @"\\Users\\.?\\[a-zA-Z\-0-9#_!&\*\(\)]*?\.",
            @"\\Users\\.?\\AppData\\[a-zA-Z\-0-9#_!&\*\(\)]*?\.",
            @"\\Users\\.?\\AppData\\Local\\[a-zA-Z\-0-9#_!&\*\(\)]*?\.", 
            @"\\Users\\.?\\AppData\\Roaming\\[a-zA-Z\-0-9#_!&\*\(\)]*?\.", 
            @"\\Users\\.?\\AppData\\LocalLow\\[a-zA-Z\-0-9#_!&\*\(\)]*?\.",
            @"/Program Files/[a-zA-Z\-0-9#_!&\*\(\)]*?\.", 
            @"/Program Files \(x86\)/[a-zA-Z\-0-9#_!&\*\(\)]*?\.",
            @"/ProgramData/[a-zA-Z\-0-9#_!&\*\(\)]*?\.",
            @"/Users/.?/[a-zA-Z\-0-9#_!&\*\(\)]*?\.",
            @"/Users/.?/AppData/[a-zA-Z\-0-9#_!&\*\(\)]*?\.",
            @"/Users/.?/AppData/Local/[a-zA-Z\-0-9#_!&\*\(\)]*?\.", 
            @"/Users/.?/AppData/Roaming/[a-zA-Z\-0-9#_!&\*\(\)]*?\.", 
            @"/Users/.?/AppData/LocalLow/[a-zA-Z\-0-9#_!&\*\(\)]*?\."
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
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

                string[] extensions = options.Extensions.Split(',');
                for(int index = 0; index < extensions.Length; index++)
                {
                    extensions[index] = extensions[index].Trim();
                }

                List<Regex> regexes = new List<Regex>();
                for (int index = 0; index < extensions.Length; index++)
                {
                    extensions[index] = extensions[index].Trim();

                    foreach (string path in _paths)
                    {
                        regexes.Add(new Regex(path + extensions[index], RegexOptions.IgnoreCase));
                    }
                }

                List<string> suspiciousFiles = new List<string>();

                string line = string.Empty;
                List<string> files = new List<string>();
                using (System.IO.StreamReader file = new System.IO.StreamReader(options.Input))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        line = line.Trim();

                        bool found = false;

                        string fileName = string.Empty;
                        try
                        {
                            fileName = Path.GetFileName(line);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Invalid line: " + line);
                            continue;
                        }

                        foreach (string extension in extensions)
                        {
                            if (line.EndsWith("." + extension, StringComparison.InvariantCultureIgnoreCase) == true)
                            {
                                // a.exe, c.bat etc
                                if (fileName.Length == 5)
                                {
                                    if (suspiciousFiles.Contains(line) == false)
                                    {
                                        suspiciousFiles.Add(line);
                                    }
                                }

                                foreach (Regex regex in regexes)
                                {
                                    if (regex.Match(line).Success == false)
                                    {
                                        continue;
                                    }

                                    if (suspiciousFiles.Contains(line) == false)
                                    {
                                        suspiciousFiles.Add(line);
                                    }
                                }

                                found = true;
                                break;
                            }
                        }

                        if (found == false)
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

                string outputPath = Path.Combine(options.Output, "exefinder.txt");

                files.Sort();
                foreach (string temp in files)
                {
                    string ret = WriteTextToFile(temp + Environment.NewLine, outputPath, true);
                }

                outputPath = Path.Combine(options.Output, "exefinder.suspicious.txt");

                suspiciousFiles.Sort();
                foreach (string temp in suspiciousFiles)
                {
                    WriteTextToFile(temp + Environment.NewLine, outputPath, true);
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
