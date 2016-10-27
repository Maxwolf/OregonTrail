var json = "./project.json";
var target = Argument ("target", "Default");

Task ("Default").IsDependentOn ("build");

Task ("build").IsDependentOn ("clean").Does (() => 
{
	DotNetCoreRestore();
	
	// Always use Jenkins configuration never Debug or Release they will bump version numbers.
	var buildSettings = new DotNetCoreBuildSettings {
		Configuration = "Jenkins"
	};
	
	DotNetCoreBuild(json, buildSettings);
});

Task ("clean").Does (() => 
{
	CleanDirectories ("./**/bin");
	CleanDirectories ("./**/obj");

	CleanDirectories ("./**/Components");
	//CleanDirectories ("./**/tools");

	DeleteFiles ("./**/*.apk");
});

RunTarget (target);