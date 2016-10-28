var json = "./project.json";
var target = Argument ("target", "Default");

Task ("Default").IsDependentOn ("build");

Task ("build").Does (() => 
{
	DotNetCoreRestore();
	
	// Always use Jenkins configuration never Debug or Release they will bump version numbers.
	var buildSettings = new DotNetCoreBuildSettings {
		Configuration = "Jenkins"
	};
	
	DotNetCoreBuild(json, buildSettings);
	
	DotNetCorePublish(json);
});

RunTarget (target);
