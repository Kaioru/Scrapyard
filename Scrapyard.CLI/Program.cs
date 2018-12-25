using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Scrapyard.Core.Codecs;

namespace Scrapyard.CLI
{
    internal static class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Value(0, Required = true, MetaName = "input", HelpText = "The input directory.")]
            public string InputDirectory { get; set; }

            [Value(1, Required = true, MetaName = "output", HelpText = "The output file.")]
            public string OutputFile { get; set; }
        }

        private static void Main(string[] args)
            => Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    var decoder = new DirectoryDecoder();
                    var node = decoder.Decode(o.InputDirectory);
                    var watch = Stopwatch.StartNew();

                    if (o.Verbose)
                        Console.WriteLine($"Finished parsing directories in {watch.ElapsedMilliseconds}ms");

                    var attributes = File.GetAttributes(o.OutputFile);
                    var output = o.OutputFile;

                    if (attributes.HasFlag(FileAttributes.Directory))
                        output = Path.Join(output, "Package.nx");

                    var encoder = new NXEncoder();

                    watch.Restart();

                    using (var stream = new FileStream(output,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None
                    ))
                    using (var writer = new BinaryWriter(stream))
                        encoder.Encode(writer, node);

                    if (o.Verbose)
                        Console.WriteLine($"Finished writing node in {watch.ElapsedMilliseconds}ms");
                    Console.WriteLine($"Finished packaging {o.InputDirectory} to {output}");
                });
    }
}