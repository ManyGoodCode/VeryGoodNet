using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using ILMerging;
using Microsoft.Win32;
using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Options;

namespace SharpPak
{
    public class SharpPakApp
    {
        public SharpPakApp()
        {
            OutputDirectory = "Output";
            AssembliesToLink = new List<string>();
        }

        public string OutputDirectory { get; set; }
        public string MainAssembly { get; set; }
        public List<string> AssembliesToLink { get; set; }
        public bool AutoReferences { get; set; }
        public bool NoLinker { get; set; }

        private static void UsageError(string error)
        {
            string exeName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
            Console.Write("{0}: ", exeName);
            Console.WriteLine(error);
            Console.WriteLine("Use {0} --help' for more information.", exeName);
            Environment.Exit(1);
        }

        public void ParseArguments(string[] args)
        {
            bool showHelp = false;
            OptionSet options = new OptionSet()
                              {
                                  "Copyright (c) 2010-2014 SharpDX - Alexandre Mutel",
                                  "Usage: SharpPak [options]* MainAssembly.exe/dll [ Assembly1.dll...]*",
                                  "Assembly linker for SharpDX applications",
                                  "",
                                  "options:",
                                  {"a|auto", "Embed automatically all referenced assemblies [default: false]", opt => AutoReferences = opt != null},
                                  {"n|nolinker", "Perform no linker [default: false]", opt => NoLinker = opt != null},
                                  {"o|output=", "Specify the output directory [default: Output]", opt => OutputDirectory = opt},
                                  {"h|help", "Show this message and exit", opt => showHelp = opt != null},
                                  {"<>", opt => AssembliesToLink.AddRange(opt.Split(' ', '\t')) },
                              };
            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                UsageError(e.Message);
            }

            if (showHelp)
            {
                options.WriteOptionDescriptions(Console.Out);
                Environment.Exit(0);
            }

            if (AssembliesToLink.Count == 0)
                UsageError("MainAssembly file to pack is missing");

            MainAssembly = AssembliesToLink[0];
            AssembliesToLink.RemoveAt(0);
        }

        private void AddAssemblies(AssemblyDefinition assembly,
            List<string> paths,
            string fromDirectory,
            string[] includeMergeListRegex)
        {
            HashSet<AssemblyDefinition> hashSet = new HashSet<AssemblyDefinition>();
            AddAssemblies(assembly, paths, fromDirectory, includeMergeListRegex, hashSet);
        }


        private void AddAssemblies(AssemblyDefinition assembly, List<string> paths, string fromDirectory, string[] includeMergeListRegex, HashSet<AssemblyDefinition> added)
        {
            if(added.Contains(assembly))
                return;

            added.Add(assembly);
            string directoryOfAssembly = Path.GetDirectoryName(assembly.MainModule.FullyQualifiedName);
            if (fromDirectory == directoryOfAssembly)
            {
                if(!paths.Contains(assembly.MainModule.FullyQualifiedName))
                {
                    paths.Add(assembly.MainModule.FullyQualifiedName);
                }
            }

            foreach (AssemblyNameReference assemblyRef in assembly.MainModule.AssemblyReferences)
            {
                bool isAssemblyAdded = false;

                foreach (var regexIncludeStr in includeMergeListRegex)
                {
                    var regexInclude = new Regex(regexIncludeStr);
                    if (regexInclude.Match(assemblyRef.Name).Success)
                    {
                        var assemblyDefRef = assembly.MainModule.AssemblyResolver.Resolve(assemblyRef);
                        AddAssemblies(assemblyDefRef, paths, fromDirectory, includeMergeListRegex, added);
                        isAssemblyAdded = true;
                        break;
                    }
                }

                if (!isAssemblyAdded && AutoReferences)
                {
                    var assemblyDefRef = assembly.MainModule.AssemblyResolver.Resolve(assemblyRef);
                    AddAssemblies(assemblyDefRef, paths, fromDirectory, includeMergeListRegex, added);
                }
            }
        }



        /// <summary>
        /// Performs packing.
        /// </summary>
        public void Run()
        {
            // Steps
            // 1) Mono.Cecil: Determine assembly dependencies
            // 2) ILMerge: Merge exe into a single assembly
            // 3) Mono.Linker
            var includeMergeListRegex = new string[] { @"SharpDX\..*" };

            // Step 1 : Mono.Cecil: Determine assembly dependencies
            var assembly = AssemblyDefinition.ReadAssembly(MainAssembly);
            var corlib = (AssemblyNameReference)assembly.MainModule.TypeSystem.Corlib;
            bool isNet40 = corlib.Version.Major == 4;

            var paths = new List<string>();

            var fromDirectory = Path.GetDirectoryName(assembly.MainModule.FullyQualifiedName);

            // Load SharpDX assemblies
            AddAssemblies(assembly, paths, fromDirectory, includeMergeListRegex);

            // Load assemblies to link
            foreach (var assemblyToLinkName in AssembliesToLink)
            {
                var assemblyToLink = AssemblyDefinition.ReadAssembly(assemblyToLinkName);
                paths.Add(assemblyToLink.MainModule.FullyQualifiedName);
            }


            // Step 2: ILMerge: Merge exe into a single assembly
            var merge = new ILMerge();

            String[] files = paths.ToArray();

            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);

            //Here we get the first file name (which was the .exe file) and use that
            // as the output
            String strOutputFile = System.IO.Path.GetFileName(files[0]);

            merge.OutputFile = OutputDirectory + "\\" + strOutputFile;
            merge.SetInputAssemblies(files);
            merge.DebugInfo = false;
            merge.CopyAttributes = true;
            merge.AllowMultipleAssemblyLevelAttributes = true;
            merge.XmlDocumentation = false;

            // Special case for v4 framework
            // See http://research.microsoft.com/en-us/people/mbarnett/ilmerge.aspx
            if (isNet40)
            {
                // Retrieve the install root path for the framework
                string installRoot = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NetFramework", false).GetValue("InstallRoot").ToString();
                var directorties = Directory.GetDirectories(installRoot, "v4.*");
                if (directorties.Length == 0)
                    UsageError(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Cannot found any .Net 4.0 directory from [{0}] ", installRoot));
                merge.SetTargetPlatform("v4", directorties[0]);                
            }

            merge.Merge();

            // Step 3: Mono.Linker
            if (!NoLinker)
            {
                var pipeline = GetStandardPipeline();
                var context = new LinkContext(pipeline) {CoreAction = AssemblyAction.Skip, OutputDirectory = OutputDirectory};
                context.OutputDirectory = OutputDirectory;

                var mainAssemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(Path.GetFullPath(MainAssembly)));
                context.Resolver.AddSearchDirectory(mainAssemblyDirectory.FullName);

                // Load assembly merged previously by ILMerge
                var mergedAssemblyDefinition = context.Resolve(merge.OutputFile);

                // Create Mono.Linker default pipeline
                pipeline = GetStandardPipeline();
                pipeline.PrependStep(new ResolveFromAssemblyStep(mergedAssemblyDefinition));

                // Add custom step for ComObject constructors
                pipeline.AddStepBefore(typeof (SweepStep), new ComObjectStep());

                pipeline.Process(context);
            }

            Console.WriteLine("Assembly successfully packed to [{0}]", merge.OutputFile);
        }

        static Pipeline GetStandardPipeline()
        {
            var pipeline = new Pipeline();
            pipeline.AppendStep(new LoadReferencesStep());
            pipeline.AppendStep(new BlacklistStep());
            pipeline.AppendStep(new TypeMapStep());
            pipeline.AppendStep(new MarkStep());
            pipeline.AppendStep(new SweepStep());
            pipeline.AppendStep(new CleanStep());
            pipeline.AppendStep(new RegenerateGuidStep());
            pipeline.AppendStep(new OutputStep());
            return pipeline;
        }
    }
}