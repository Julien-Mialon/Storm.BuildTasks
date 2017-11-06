#l "nuget:?package=Cake.Storm.Fluent"
#l "nuget:?package=Cake.Storm.Fluent.DotNetCore"
#l "nuget:?package=Cake.Storm.Fluent.NuGet"

Configure()
	.UseRootDirectory("..")
	.UseBuildDirectory("build")
	.UseArtifactsDirectory("artifacts")
	.AddConfiguration(configuration => configuration
		.WithSolution("Storm.BuildTasks.sln")
        .WithTargetFrameworks("net46")
		.WithBuildParameter("Configuration", "Release")
		.WithBuildParameter("Platform", "Any CPU")
		.UseDefaultTooling()
		.UseDotNetCoreTooling()
        .WithDotNetCoreOutputType(OutputType.Copy)
	)
	//platforms configuration
	.AddPlatform("dotnet")
	//targets configuration
	.AddTarget("pack", configuration => configuration
        .UseNugetPack(nugetConfiguration => nugetConfiguration.WithAuthor("Julien Mialon"))
	)
    .AddTarget("push", configuration => configuration
        .UseNugetPack(nugetConfiguration => nugetConfiguration.WithAuthor("Julien Mialon"))
        .UseNugetPush(nugetConfiguration => nugetConfiguration.WithApiKeyFromEnvironment())
    )
    //applications configuration
	.AddApplication("android-colors", configuration => configuration
        .WithProject("src/Storm.BuildTasks.AndroidColors/Storm.BuildTasks.AndroidColors.csproj")
        .WithVersion("0.2.2")
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("misc/nuspecs/Storm.BuildTasks.AndroidColors.nuspec")
            .WithPackageId("Storm.BuildTasks.AndroidColors")
            .WithReleaseNotesFile("misc/release_notes/Storm.BuildTasks.AndroidColors.md")
            .AddFileFromArtifacts("net46/Storm.BuildTasks.AndroidColors.dll", "colors")
            //dependencies
            .AddFile("src/Storm.BuildTasks.AndroidColors/bin/Release/net46/Storm.BuildTasks.Common.dll", "colors")
            .AddFile("src/Storm.BuildTasks.AndroidColors/bin/Release/net46/Microsoft.CodeAnalysis.CSharp.dll", "colors")
            .AddFile("src/Storm.BuildTasks.AndroidColors/bin/Release/net46/Microsoft.CodeAnalysis.dll", "colors")
            //props & target file
            .AddFile("src/Storm.BuildTasks.AndroidColors/Storm.BuildTasks.AndroidColors.props", "build/monoandroid")
            .AddFile("src/Storm.BuildTasks.AndroidColors/Storm.BuildTasks.AndroidColors.targets", "build/monoandroid")
        )
    )
	.Build();

RunTarget(Argument("target", "help"));