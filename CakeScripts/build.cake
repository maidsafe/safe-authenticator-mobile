#load "AndroidTest.cake"
#load "IOSTest.cake"
#load "Utility.cake"
#load "NativeLibs.cake"

var target = Argument("target", "Default");
var COMMON_SLN_PATH = "../../CommonUtils/CommonUtils.sln";
var PROJ_SLN_PATH = "../SafeAuthenticator.sln";

Task("Restore-NuGet-Packages")
  .Does(() => {
    NuGetRestore(PROJ_SLN_PATH);
});

Task("Build-Soln")
  .Does(() => {
    MSBuild(PROJ_SLN_PATH, c =>
    {
        c.Configuration = "Release";
        c.Targets.Clear();
        c.Targets.Add("Rebuild");
        c.SetVerbosity(Verbosity.Minimal);
    });
});

Task("Analyse-Result-File")
  .Does(() => {
    AnalyseResultFile(ANDROID_TEST_RESULTS_PATH);
    AnalyseResultFile(IOS_TEST_RESULTS_PATH);
    Information("All Tests Have Passed");
});

Task("Default")
  .IsDependentOn("UnZip-Libs")
  .IsDependentOn("Restore-NuGet-Packages")
  .IsDependentOn("Build-Soln")
  .IsDependentOn ("Test-Android-Emu")
  .IsDependentOn ("Test-IOS-Emu")
  .IsDependentOn ("Analyse-Result-File")

  .Does(() => { });

RunTarget(target);
