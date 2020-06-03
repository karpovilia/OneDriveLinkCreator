using CommandLine;

namespace OneDriveLinkCreator
{
    public class Options
    {
        [Option("api", Required = true, HelpText = "OneDrive API key")]
        public string api_key { get; set; }

        [Option("token", Required = true, HelpText = "OneDrive Token")]
        public string token { get; set; }

        [Option("path", Required = true, HelpText = "OneDrive folder path ex: '/folder/subfolder'")]
        public string path { get; set; }
    }
}
