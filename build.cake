var json = "./project.json";
var target = Argument ("target", "Default");

Task ("Default").IsDependentOn ("build");

Task ("build").Does (() => 
{
	DotNetCoreRestore();
	
	// Always use Jenkins configuration never Debug or Release they will bump version numbers.
	var buildSettings = new DotNetCoreBuildSettings {
		Verbose = true,
		Configuration = "Jenkins",
		Runtime = "win7-x64"
	};
	
	DotNetCoreBuild(json, buildSettings);
	
	var publishSettings = new DotNetCorePublishSettings {
		Verbose = true,
		Configuration = "Jenkins",
		NoBuild = true,
		Runtime = "win7-x64"
	};
	
	DotNetCorePublish(json, publishSettings);
});

RunTarget (target);
