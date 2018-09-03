#addin Cake.Curl

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var ANDROID_X86 = "android-x86";
var ANDROID_ARMEABI_V7A = "android-armeabiv7a";
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
enum Environment {
  Android,
  iOS
}

// --------------------------------------------------------------------------------
// Native lib directory
// --------------------------------------------------------------------------------

var TAG = "6be5558";
var androidLibDirectory = Directory("SafeAuthenticator.Android/lib/");
var iosLibDirectory = Directory("SafeAuthenticator.iOS/Native References/");
var nativeLibDirectory = Directory(string.Concat(System.IO.Path.GetTempPath(), "nativeauthlibs"));

// --------------------------------------------------------------------------------
// Download Libs
// --------------------------------------------------------------------------------

Task("Download-Libs")
  .Does(() => {
    foreach(var item in Enum.GetValues(typeof(Environment))) {
      string[] targets = null;
      Information(string.Format("\n{0} ", item));
      switch (item) 
      {
      case Environment.Android:
        targets = ANDROID_ARCHITECTURES;
        break;
      case Environment.iOS:
        targets = IOS_ARCHITECTURES;
        break;
      }

      foreach(var target in targets) {
        var targetDirectory = string.Format("{0}/{1}/{2}", nativeLibDirectory.Path, item, target);
        var zipFilename = string.Format("safe_authenticator-{0}-{1}.zip", TAG, target);
        var zipDownloadUrl = string.Format("https://s3.eu-west-2.amazonaws.com/safe-client-libs/{0}", zipFilename);
        var zipSavePath = string.Format("{0}/{1}/{2}/{3}", nativeLibDirectory.Path, item, target, zipFilename);

        Information("Downloading : {0}", zipFilename);

        if(!DirectoryExists(targetDirectory))
          CreateDirectory(targetDirectory);

        if(!FileExists(zipSavePath)) 
        {
          CurlDownloadFiles(
            new [] {
              new Uri(zipDownloadUrl)
            },
            new CurlDownloadSettings {
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
  })
  .ReportError(exception => {
    Information(exception.Message);
  });

Task("UnZip-Libs")
  .IsDependentOn("Download-Libs")
  .Does(() => {
    foreach(var item in Enum.GetValues(typeof(Environment))) {
      string[] targets = null;
      var outputDirectory = string.Empty;
      Information(string.Format("\n {0} ", item));
      switch (item)
      {
      case Environment.Android:
        targets = ANDROID_ARCHITECTURES;
        outputDirectory = androidLibDirectory.Path.FullPath.ToString();
        break;
      case Environment.iOS:
        targets = IOS_ARCHITECTURES;
        outputDirectory = iosLibDirectory.Path.FullPath.ToString();
        break;
      }

      CleanDirectories(outputDirectory);

      foreach(var target in targets) {
        var zipDirectorySource = Directory(string.Format("{0}/{1}/{2}", nativeLibDirectory.Path, item, target));
        var zipFiles = GetFiles(string.Format("{0}/*.*", zipDirectorySource));
        foreach(var zip in zipFiles) {
          var filename = zip.GetFilename();
          Information(" Unzipping : " + filename);
          var platformOutputDirectory = new StringBuilder();
          platformOutputDirectory.Append(outputDirectory);

          if(target.Equals(ANDROID_X86))
            platformOutputDirectory.Append("/x86");
          else if(target.Equals(ANDROID_ARMEABI_V7A))
            platformOutputDirectory.Append("/armeabi-v7a");

          Unzip(zip, platformOutputDirectory.ToString());
          
          if(target.Equals(ANDROID_X86) || target.Equals(ANDROID_ARMEABI_V7A))
          {
            var aFilePath = platformOutputDirectory.ToString() + "/libsafe_authenticator.a";
            DeleteFile(aFilePath);
          } 
        }
      }
    }
  })
  .ReportError(exception => {
    Information(exception.Message);
  });

Task("Default")
  .IsDependentOn("UnZip-Libs")
  .Does(() => {

  });

RunTarget(target);
