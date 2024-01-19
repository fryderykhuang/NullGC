using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using BenchmarkResultPages;
using RazorEngineCore;

namespace BenchmarkResultPageGenerator;

internal class Program
{
    private static readonly Regex FileNamePattern = new(@"^(\w+\.)+Benchmarks\.(?<class>\w+)\-report",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static async Task Main(string[] args)
    {
        var dir = args[0];
        var urlBase = args[1];
        var output = args[2];

        var lst = new List<BenchmarkResultFile>();
        foreach (var item in Directory.GetFiles(dir, "*.html"))
        {
            var fn = Path.GetFileName(item);
            Match m = FileNamePattern.Match(fn);
            if (m.Success)
                lst.Add(new BenchmarkResultFile
                {
                    Title = urlBase + m.Groups["class"].Value, Url = args[1] + UrlEncoder.Default.Encode(fn),
                    Content = await File.ReadAllTextAsync(item)
                });
        }

        using var sr = new StreamReader(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"{typeof(Program).Namespace}.BenchmarkResult.cshtml")!);

        await File.WriteAllTextAsync(output, await (await new RazorEngine().CompileAsync(await sr.ReadToEndAsync()))
            .RunAsync(new
            {
                Files = lst
            }));
    }
}