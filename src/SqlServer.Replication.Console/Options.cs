using CmdLine;

namespace SqlServer.Replication.Console
{
    [CommandLineArguments]
    internal class Options
    {
        [CommandLineParameter("Action", Required = true)]
        public string Action { get; set; }

        [CommandLineParameter("OutputFile")]
        public string OutputFile { get; set; }

        [CommandLineParameter("ProjectFile")]
        public string ProjectFile { get; set; }

        [CommandLineParameter("SourceFile")]
        public string SourceFile { get; set; }

        [CommandLineParameter("SourceDatabaseName")]
        public string SourceDatabaseName { get; set; }

        [CommandLineParameter("SourcePassword")]
        public string SourcePassword { get; set; }

        [CommandLineParameter("SourceServerName")]
        public string SourceServerName { get; set; }

        [CommandLineParameter("SourceUser")]
        public string SourceUser { get; set; }

        [CommandLineParameter("TargetFile")]
        public string TargetFile { get; set; }
    }
}
