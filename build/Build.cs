using System;
using Nuke.Common;
using Nuke.Common.CI;

using Nuke.Common.Execution;
[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Host switch
    {
        HostType.Console => LocalBuild.Run(),
        HostType.TeamCity => throw new NotImplementedException(),
        HostType.AzurePipelines => throw new NotImplementedException(),
        HostType.Bamboo => throw new NotImplementedException(),
        HostType.Bitrise => throw new NotImplementedException(),
        HostType.AppVeyor => throw new NotImplementedException(),
        HostType.Jenkins => throw new NotImplementedException(),
        HostType.Travis => throw new NotImplementedException(),
        HostType.GitLab => throw new NotImplementedException(),
        HostType.GitHubActions => throw new NotImplementedException(),
        _ => throw new NotImplementedException(),
    };
}
