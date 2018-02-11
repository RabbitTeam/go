$configuration = "Release"
$base_dir = Resolve-Path "..\"
$artifacts_dir = "$base_dir\artifacts"
$build_dir = "$base_dir\build"
$source_dir = "$base_dir\src"
$working_dir = "$build_dir\working"
$recipe_dir = "$base_dir\recipes"
$nuget_source = "https://api.nuget.org/v3/index.json"
$nuget_apiKey = "xxxxxxxxxx"
$projects = @(
	@{ Name = "Rabbit.Go.Abstractions"; SourceDir = "$source_dir\Rabbit.Go.Abstractions"; ExternalNuGetDependencies = $null; UseMSBuild = $False; },
	@{ Name = "Rabbit.Go.Core"; SourceDir = "$source_dir\Rabbit.Go.Core"; ExternalNuGetDependencies = $null; UseMSBuild = $False; },
	@{ Name = "Rabbit.Go.DingTalk"; SourceDir = "$recipe_dir\DingTalk"; ExternalNuGetDependencies = $null; UseMSBuild = $False; }
)