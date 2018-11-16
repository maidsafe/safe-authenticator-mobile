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
var androidLibDirectory = Directory("../SafeAuthenticator.Android/lib/");
var iosLibDirectory = Directory("../SafeAuthenticator.iOS/Native References/");
var androidTestLibDirectory = Directory("../Tests/SafeAuth.Tests.Android/lib/");
var iosTestLibDirectory = Directory("../Tests/SafeAuth.Tests.IOS/Native References/");

var AndroidDir = new ConvertableDirectoryPath[] {
    androidTestLibDirectory,
    androidLibDirectory
};

var IOSDir = new ConvertableDirectoryPath[] {
    iosTestLibDirectory,
    iosLibDirectory
};

var DirList = new ConvertableDirectoryPath[][] {
    AndroidDir,
    IOSDir
};

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
                    DownloadFile(zipDownloadUrl, File(zipSavePath));
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

        ConvertableDirectoryPath[] platformDir = null;
        string[] targets = null;
        var outputDirectory = string.Empty;
        Information($"\n{item}");
        switch (item)
        {
            case Environment.Android:
                targets = ANDROID_ARCHITECTURES;
                platformDir = DirList.Single(x => x.Equals(AndroidDir));
                break;
            case Environment.iOS:
                targets = IOS_ARCHITECTURES;
                platformDir = DirList.Single(x => x.Equals(IOSDir));
                break;
        }
        foreach (var type in platformDir.Zip(LibTypes, Tuple.Create))
        {
            outputDirectory = type.Item1;
            CleanDirectories(outputDirectory);
            foreach (var target in targets)
            {

                var zipFilename = $"safe_authenticator{type.Item2}-{TAG}-{target}.zip";
                var zipSavePath = $"{nativeLibDirectory.Path}/{item}/{target}/{zipFilename}";
                var zipFiles = GetFiles(string.Format(zipSavePath));

                foreach (var zip in zipFiles)
                {
                    var filename = zip.GetFilename();
                    Information(" Unzipping : " + filename);
                    var platformOutputDirectory = new StringBuilder();
                    platformOutputDirectory.Append(outputDirectory);

                    if (target.Equals(ANDROID_X86))
                        platformOutputDirectory.Append("/x86");
                    else if (target.Equals(ANDROID_ARMEABI_V7A))
                        platformOutputDirectory.Append("/armeabi-v7a");

                    Unzip(zip, platformOutputDirectory.ToString());
                    if (target.Equals(ANDROID_X86) || target.Equals(ANDROID_ARMEABI_V7A))
                    {
                        var aFilePath = platformOutputDirectory.ToString() + "/libsafe_authenticator.a";
                        DeleteFile(aFilePath);
                    }
                }
            }
        }
    }
})
    .ReportError(exception => {
    Information(exception.Message);
});
