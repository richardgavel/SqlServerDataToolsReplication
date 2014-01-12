using CmdLine;
using SqlServer.Replication.Core;

namespace SqlServer.Replication.Console
{
    /// <summary>
    /// Console application used to perform publication compilation, extraction and SQL script generation
    /// </summary>
    static class Program
    {
        static void Main()
        {
            try
            {
                var options = CommandLine.Parse<Options>();

                switch (options.Action)
                {
                    case "Compile":
                        Compile(options);
                        break;

                    case "Extract":
                        Extract(options);
                        break;

                    case "Script":
                        Script(options);
                        break;
                }
            }
            catch (CommandLineException exception)
            {
                System.Console.WriteLine(exception.ArgumentHelp.Message);
                System.Console.WriteLine(exception.ArgumentHelp.GetHelpText(System.Console.BufferWidth));
            }
        }

        static void Compile(Options options)
        {
            if (string.IsNullOrEmpty(options.SourceFile))
                throw new CommandLineRequiredArgumentMissingException(typeof(Options), "SourceFile", -1);

            if (string.IsNullOrEmpty(options.ProjectFile))
                throw new CommandLineRequiredArgumentMissingException(typeof(Options), "ProjectFile", -1);

            var augmentor = new SqlPublicationDacpacAugmentor();
            var compiler = new SqlPublicationCompiler();

            augmentor.Augment(options.SourceFile, compiler.Compile(options.ProjectFile, options.SourceFile));
        }

        static void Extract(Options options)
        {
            if (string.IsNullOrEmpty(options.SourceServerName))
                throw new CommandLineRequiredArgumentMissingException(typeof(Options), "SourceServerName", -1);

            if (string.IsNullOrEmpty(options.SourceDatabaseName))
                throw new CommandLineRequiredArgumentMissingException(typeof(Options), "SourceDatabaseName", -1);

            if (string.IsNullOrEmpty(options.SourceUser))
                throw new CommandLineRequiredArgumentMissingException(typeof(Options), "SourceUser", -1);

            if (string.IsNullOrEmpty(options.SourcePassword))
                throw new CommandLineRequiredArgumentMissingException(typeof(Options), "SourcePassword", -1);

            if (string.IsNullOrEmpty(options.TargetFile))
                throw new CommandLineRequiredArgumentMissingException(typeof(Options), "TargetFile", -1);
            
            var augmentor = new SqlPublicationDacpacAugmentor();
            var extractor = new SqlPublicationExtractor();

            var connectionString = string.Format("Data Source={0};Initial Catalog={1};UID={2};Password={3};MultipleActiveResultSets=True",
                                                 options.SourceServerName, options.SourceDatabaseName, options.SourceUser,
                                                 options.SourcePassword);
            augmentor.Augment(options.TargetFile, extractor.Extract(connectionString));
        }
        
        static void Script(Options options)
        {
            if (string.IsNullOrEmpty(options.SourceFile))
                throw new CommandLineRequiredArgumentMissingException(typeof(Options), "SourceFile", -1);

            if (string.IsNullOrEmpty(options.TargetFile))
                throw new CommandLineRequiredArgumentMissingException(typeof(Options), "TargetFile", -1);

            if (string.IsNullOrEmpty(options.OutputFile))
                throw new CommandLineRequiredArgumentMissingException(typeof(Options), "OutputFile", -1);
            
            var scriptGenerator = new SqlPublicationScriptGenerator();
            scriptGenerator.Generate(options.SourceFile, options.TargetFile, options.OutputFile);
        }
    }
}
