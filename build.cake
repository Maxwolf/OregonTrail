var json = "./project.json";
var target = Argument ("target", "Default");

Task ("Default").IsDependentOn ("build");

Task ("build").Does (() => 
{
	DotNetCoreRestore();
	
	// Always use Jenkins configuration never Debug or Release they will bump version numbers.
	var buildSettings = new DotNetCoreBuildSettings {
		Verbose = true,
		Configuration = "Jenkins"
	};
	
	DotNetCoreBuild(json, buildSettings);
	
	var buildSettings = new DotNetCorePublishSettings {
		Verbose = true,
		Configuration = "Jenkins",
		NoBuild = true,
		
	};
	
	DotNetCorePublish(json);
});

RunTarget (target);
