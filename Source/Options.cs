using System;
using CommandLine;
using CommandLine.Text; 

namespace exefinder
{
    /// <summary>
    /// Internal class used for the command line parsing
    /// </summary>
    internal class Options 
    {
        [ParserState]
        public IParserState LastParserState { get; set; }

        [Option('i', "input", Required = true, DefaultValue = "", HelpText = "Input file")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, DefaultValue = "", HelpText = "Output directory")]
        public string Output { get; set; }

        [Option('x', "extensions", Required = false, DefaultValue = "exe", HelpText = "File extensions to search for. Defaults to exe. Comma separate each value")]
        public string Extensions { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Copyright = new CopyrightInfo("woanware", 2013),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };

            this.HandleParsingErrorsInHelp(help);

            help.AddPreOptionsLine("Usage: exefinder -i mft.txt -o exes.txt -x \"exe,dll,bat\"");
            help.AddOptions(this);

            if (this.LastParserState.Errors.Count > 0)
            {
                var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
                    help.AddPreOptionsLine(errors);
                }
            }

            return help;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="help"></param>
        private void HandleParsingErrorsInHelp(HelpText help)
        {
            //if (this.LastPostParsingState.Errors.Count > 0)
            //{
            //    var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
            //    if (!string.IsNullOrEmpty(errors))
            //    {
            //        help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
            //        help.AddPreOptionsLine(errors);
            //    }
            //}
        }
    }
}
