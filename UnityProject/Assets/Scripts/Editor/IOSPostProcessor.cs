using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class IOSPostProcessor {

    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string path) {

        if (buildTarget == BuildTarget.iOS) {

            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;

            Debug.Log(">> Automation, plist ... <<");

            #region Example
            // example of changing a value:
            // rootDict.SetString("CFBundleVersion", "6.6.6");

            // // example of adding a boolean key...
            // // < key > ITSAppUsesNonExemptEncryption </ key > < false />
            // rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            // PlistElement v = rootDict.values["URL types"];
                
            #endregion
            
            #region HTTP request fix
                
            Debug.Log(">> Automation, plist ... app security  <<");

            PlistElementDict transportSecurity = rootDict.values["NSAppTransportSecurity"].AsDict();
            PlistElementDict exceptionDomains = transportSecurity.CreateDict("NSExceptionDomains");

            var companyDomain = exceptionDomains.CreateDict("retrostylegames.com");

            companyDomain.SetString("NSTemporaryExceptionMinimumTLSVersion","TLSv1.1");
            companyDomain.SetBoolean("NSTemporaryExceptionAllowsInsecureHTTPLoads",true);
            companyDomain.SetBoolean("NSIncludesSubdomains",true);

            #endregion

            #region FIX "Ads SDK was initialized without an application ID" 
            Debug.Log(">> Automation, plist ... Ads sdk fix  <<");
            // https://stackoverflow.com/questions/58405905/ads-sdk-was-initialized-without-an-application-id issue

            rootDict.SetBoolean("GADIsAdManagerApp",true);
                
            #endregion

            // #region Branch io

            // Debug.Log(">> Automation, plist ... branch io  <<");

            // PlistElementArray urlTypes = rootDict.values["URL types"].AsArray();
            // PlistElementDict firstURL = urlTypes.values[0].AsDict();
            // firstURL.SetString("URL identifier", "io.branch.sdk");
                
            // #endregion


            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }

    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path) {

        if (target == BuildTarget.iOS) {

            PBXProject project = new PBXProject();
            string sPath = PBXProject.GetPBXProjectPath(path);
            project.ReadFromFile(sPath);

            string tn = PBXProject.GetUnityTestTargetName();
            string g = project.TargetGuidByName(tn);

            ModifyFrameworksSettings(project, g);

            // modify frameworks and settings as desired
            File.WriteAllText(sPath, project.WriteToString());
        }
    }

    static void ModifyFrameworksSettings(PBXProject project, string g) {

        // // add hella frameworks

        // Debug.Log(">> Automation, Frameworks... <<");

        // project.AddFrameworkToProject(g, "blah.framework", false);
        // project.AddFrameworkToProject(g, "libz.tbd", false);

        // // go insane with build settings

        // Debug.Log(">> Automation, Settings... <<");

        // project.AddBuildProperty(g,
        //     "LIBRARY_SEARCH_PATHS",
        //     "../blahblah/lib");

        // project.AddBuildProperty(g,
        //     "OTHER_LDFLAGS",
        //     "-lsblah -lbz2");

        // // note that, due to some Apple shoddyness, you usually need to turn this off
        // // to allow the project to ARCHIVE correctly (ie, when sending to testflight):
        // project.AddBuildProperty(g,
        //     "ENABLE_BITCODE",
        //     "false");
    }

}