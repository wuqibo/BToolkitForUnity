using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using UnityEngine;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace BToolkit
{
    public class XCodeProjectConfig
    {

#if UNITY_IOS
	[PostProcessBuild]
	static void OnPostprocessBuild(BuildTarget buildTarget, string path){
		if (buildTarget == BuildTarget.iOS) {
			Debug.Log (path);

			string projPath = PBXProject.GetPBXProjectPath(path);
			PBXProject proj = new PBXProject();
			proj.ReadFromString(File.ReadAllText(projPath));
			// 获取当前项目名字  
			string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

			//对所有的编译配置设置选项  
			OtherSetting(proj,target,projPath);
			//添加依赖库  
			AddFrameworks(proj,target,projPath);
			//修改plist  
			SetPlist(path);
		}  
	}

	//对所有的编译配置设置选项
	static void OtherSetting(PBXProject proj, string target, string projPath){
		proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
		//微信
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-Objc");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-all_load");
		proj.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/BToolkit/WeiXinAPI/Plugins/iOS");
		proj.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries/BToolkit/WeiXinAPI/Plugins/iOS");

		// 保存工程  
		proj.WriteToFile (projPath);
	}

	// 添加依赖库  
	static void AddFrameworks(PBXProject proj, string target, string projPath){
		//微信qq共用
		proj.AddFrameworkToProject (target, "Security.framework", false);
		proj.AddFrameworkToProject (target, "SystemConfiguration.framework", false);//支付宝也需要
		proj.AddFrameworkToProject (target, "CoreTelephony.framework", false);
		proj.AddFrameworkToProject (target, "OpenGLES.framework", false);
		proj.AddFrameworkToProject (target, "LocalAuthentication.framework", false);
		proj.AddFrameworkToProject (target, "StoreKit.framework", false);
		proj.AddFrameworkToProject (target, "libz.dylib", false);//支付宝也需要
		//微信用
		proj.AddFrameworkToProject (target, "libsqlite3.0.dylib", false);
		proj.AddFrameworkToProject (target, "libc++.dylib", false);//支付宝也需要
		proj.AddFrameworkToProject (target, "CFNetwork.framework", false);
		//QQ用
		proj.AddFrameworkToProject (target, "libiconv.dylib", false);
		proj.AddFrameworkToProject (target, "CoreGraphics.Framework", false);
		proj.AddFrameworkToProject (target, "libsqlite3.dylib", false);
		proj.AddFrameworkToProject (target, "libstdc++.dylib", false);
		// 保存工程  
		proj.WriteToFile (projPath);
	}

	// 修改plist  
	static void SetPlist(string path){
		string plistPath = path + "/Info.plist";  
		PlistDocument plist = new PlistDocument();  
		plist.ReadFromString(File.ReadAllText(plistPath));  
		PlistElementDict rootDict = plist.root;  

		//使用相机的权限
		rootDict.SetString("NSCameraUsageDescription", "程序需要使用摄影机");
		//读取相册
		rootDict.SetString("NSPhotoLibraryUsageDescription", "需要从相册读取照片");
		//写入相
		rootDict.SetString("NSPhotoLibraryAddUsageDescription", "需要保存图片到相册");
		//添加白名单
		var array0 = rootDict.CreateArray("LSApplicationQueriesSchemes");
		//微信白名单
		array0.AddString("weixin");
		//QQ白名单
		array0.AddString("mqq");
		array0.AddString("wtloginmqq2");
		array0.AddString("mqqopensdkapiV3");
		array0.AddString("mqqwpa");
		array0.AddString("mqqopensdkapiV2");
		array0.AddString("mqqOpensdkSSoLogin");
		array0.AddString("mqqopensdkapiV2");
		//插入URL Scheme 微信和QQ 支付宝Url(只需确保唯一性，可直接用AppId)
		//支付宝需和支付接口里的NSString *appScheme = @"alipay"一致
		AddURLScheme(plist, new string[][] {new string[]{"weixin","wxe89a11667c882904"}, new string[]{"qq","tencent101425566"} , new string[]{"alipay","alipay2017102509522622"}});//URLScheme必须是字母开头

		// 保存plist  
		plist.WriteToFile (plistPath);  
	}
	static void AddURLScheme(PlistDocument plist,string[][] urlSchemes){
		var	array = plist.root.CreateArray("CFBundleURLTypes");
        for (int i = 0; i < urlSchemes.Length; i++) {
			var urlDict = array.AddDict();
			urlDict.SetString("CFBundleTypeRole", "Editor");
			urlDict.SetString("CFBundleURLName", urlSchemes[i][0]);
			var urlInnerArray = urlDict.CreateArray("CFBundleURLSchemes");
			urlInnerArray.AddString(urlSchemes[i][1]);
            //不需要填ID
        }
	}
#endif
    }
}