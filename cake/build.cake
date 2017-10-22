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
        .WithVersion("0.1.0")
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("misc/nuspecs/Storm.BuildTasks.AndroidColors.nuspec")
            .WithPackageId("Storm.BuildTasks.AndroidColors")
            .WithReleaseNotesFile("misc/release_notes/Storm.BuildTasks.AndroidColors.md")
            .AddFileFromArtifacts("net46/Storm.BuildTasks.AndroidColors.dll", "build")
        )
    )
	.Build();

RunTarget(Argument("target", "help"));