using System.Collections.Generic;

namespace Fclp.Examples
{
    class CommandProgram
    {
        class AddArgs
        {
            public bool Verbose { get; set; }
            public bool IgnoreErrors { get; set; }
            public List<string> Files { get; set; }
            public List<string> Files2 { get; set; }
        }

        class RemoveArgs
        {
            public bool Verbose { get; set; }
            public List<string> Files { get; set; }
        }

        static void Main(string[] args)
        {
            FluentCommandLineParser fclp = new FluentCommandLineParser();
            ICommandLineCommandFluent<AddArgs> addCmd = fclp.SetupCommand<AddArgs>("add")
                             .OnSuccess(Add);

            addCmd.Setup(addArgs => addArgs.Verbose)
                  .As('v', "verbose")
                  .SetDefault(false)
                  .WithDescription("Be verbose");

            addCmd.Setup(addArgs => addArgs.IgnoreErrors)
                  .As("ignore-errors")
                  .SetDefault(false)
                  .WithDescription("If some files could not be added, do not abort");

            addCmd.Setup(addArgs => addArgs.Files)
                  .As('f', "files") 
                  .WithDescription("Files to be tracked")
                  .UseForOrphanArguments();

            ICommandLineCommandFluent<RemoveArgs> remCmd = 
                fclp.SetupCommand<RemoveArgs>("rem")
                .OnSuccess(Remove); 

            remCmd.Setup(removeArgs => removeArgs.Verbose)
                     .As('v', "verbose")
                     .SetDefault(false)
                     .WithDescription("Be verbose");

            remCmd.Setup(removeArgs => removeArgs.Files)
                     .As('f', "files")
                     .WithDescription("Files to be untracked")
                     .UseForOrphanArguments();

            fclp.Parse(args);
        }

        static void Add(AddArgs args)
        {
            
        }

        static void Remove(RemoveArgs args)
        {
            
        }
    }
}
