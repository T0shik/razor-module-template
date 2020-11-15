using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

public class LocalBuild : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    public static int Run()
    {
        return Execute<LocalBuild>(x => x.Compile);
    }

    IProcess TestAppProcess;
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestDirectory => RootDirectory / "tests";
    AbsolutePath ModuleProjectDirectory => SourceDirectory / "RazorModule";
    AbsolutePath CodeGenerationProjectDirectory => SourceDirectory / "RazorModule.Generator";
    AbsolutePath TestApiProjectDirectory => TestDirectory / "RazorModule.TestApi";
    AbsolutePath TestsProjectDirectory => TestDirectory / "RazorModule.Tests";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(RootDirectory)
            );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(RootDirectory)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
            );
        });

    Target GenerateJavascriptClient => _ => _
        .DependsOn(Compile)
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            var clientFilePath = TestApiProjectDirectory / "wwwroot" / "client.js";
            DotNetRun(c => c
                .SetProjectFile(CodeGenerationProjectDirectory / "RazorModule.Generator.csproj")
                .SetApplicationArguments($"client-output={clientFilePath}")
                .EnableNoRestore()
                .EnableNoBuild());
        });

    Target RunTests => _ => _
        .TriggeredBy(GenerateJavascriptClient)
        .DependsOn(StartTestApi)
        .Triggers(StopTestApi)
        .Executes(() =>
        {
            DotNetTest(c => c
                .SetProjectFile(TestsProjectDirectory / "RazorModule.Tests.csproj")
                .EnableNoBuild()
                .EnableNoRestore());
        });

    Target StartTestApi => _ => _
        .Executes(() =>
        {
            TestAppProcess = ProcessTasks.StartProcess(
                "dotnet",
                "run",
                TestApiProjectDirectory
            );
        });

    Target StopTestApi => _ => _
        .Executes(() =>
        {
            TestAppProcess.Kill();
        });
}