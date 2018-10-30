#addin Cake.Curl
using System.Linq;

var ANDROID_X86 = "android-x86";
var ANDROID_ARMEABI_V7A = "android-armeabiv7a";
var LibTypes = new string[] {
    "-mock",
    ""
};

var ANDROID_ARCHITECTURES = new string[] {
    ANDROID_X86,
    ANDROID_ARMEABI_V7A
};

var IOS_ARCHITECTURES = new string[] {
    "ios"
};

var All_ARCHITECTURES = new string[][] {
    ANDROID_ARCHITECTURES,
    IOS_ARCHITECTURES
};

enum Environment
{
    Android,
    iOS
}

// --------------------------------------------------------------------------------
// Native lib directory
// --------------------------------------------------------------------------------
var TAG = "6be5558";
var nativeLibDirectory = Directory(string.Concat(System.IO.Path.GetTempPath(), "nativeauthlibs"));
var androidLibDirectory = Directory("../SafeAuthenticator.Android/");
var iosLibDirectory = Directory("../SafeAuthenticator.iOS/");

// --------------------------------------------------------------------------------
// Download Libs
// --------------------------------------------------------------------------------
Task("Download-Libs")
    .Does(() => {
    foreach (var item in Enum.GetValues(typeof(Environment)))
    {
        string[] targets = null;
        Information($"\n{item}");
        switch (item)
        {

            case Environment.Android:
                targets = ANDROID_ARCHITECTURES;
                break;
            case Environment.iOS:
                targets = IOS_ARCHITECTURES;
                break;
        }
        foreach (var type in LibTypes)
        {
            foreach (var target in targets)
            {
                var targetDirectory = $"{nativeLibDirectory.Path}/{item}/{target}";
                var zipFilename = $"safe_authenticator{type}-{TAG}-{target}.zip";
                var zipDownloadUrl = $"https://s3.eu-west-2.amazonaws.com/safe-client-libs/{zipFilename}";
                var zipSavePath = $"{nativeLibDirectory.Path}/{item}/{target}/{zipFilename}";

                Information($"Downloading : {zipFilename}");
                if (!DirectoryExists(targetDirectory))
                    CreateDirectory(targetDirectory);

                if (!FileExists(zipSavePath))
                {
                    CurlDownloadFiles(
                        new[] {
                                new Uri (zipDownloadUrl)
                        },
                        new CurlDownloadSettings
                        {
                            OutputPaths = new FilePath[] {
                                    zipSavePath
                            }
                        });
                }
                else
                {
                    Information("File already exists");
                }
            }
        }
    }
})
    .ReportError(exception => {
    Information(exception.Message);
});

Task("UnZip-Libs")
    .IsDependentOn("Download-Libs")
    .Does(() => {
    foreach (var item in Enum.GetValues(typeof(Environment)))
    {
        string[] targets = null;
        var outputDirectory = string.Empty;
        Information($"\n{item}");
        switch (item)
        {
            case Environment.Android:
                targets = ANDROID_ARCHITECTURES;
                outputDirectory = androidLibDirectory;
                break;
            case Environment.iOS:
                targets = IOS_ARCHITECTURES;
                outputDirectory = iosLibDirectory;
                break;
        }

        CleanDirectories(string.Concat(outputDirectory, "/lib"));
        
        foreach(var target in targets) 
        {
            var zipSourceDirectory = Directory(string.Format("{0}/{1}/{2}", nativeLibDirectory.Path, item, target));
            var zipFiles = GetFiles(string.Format("{0}/*.*", zipSourceDirectory));
            foreach(var zip in zipFiles) 
            {
                var filename = zip.GetFilename();
                Information(" Unzipping : " + filename);
                var platformOutputDirectory = new StringBuilder();
                platformOutputDirectory.Append(outputDirectory);
                platformOutputDirectory.Append("/lib");

                if(filename.ToString().Contains("mock")) 
                {
                    platformOutputDirectory.Append("/mock");
                }
                else 
                {
                    platformOutputDirectory.Append("/non-mock");
                }

                if(target.Equals(ANDROID_X86))
                    platformOutputDirectory.Append("/x86");
                else if(target.Equals(ANDROID_ARMEABI_V7A))
                    platformOutputDirectory.Append("/armeabi-v7a");

                Unzip(zip, platformOutputDirectory.ToString());

                if(target.Contains("android"))
                {
                    var aFile = GetFiles(string.Format("{0}/*.a", platformOutputDirectory.ToString()));
                    DeleteFile(aFile.ToArray()[0].FullPath);
                }
            }
        }
    }
})
    .ReportError(exception => {
    Information(exception.Message);
});
