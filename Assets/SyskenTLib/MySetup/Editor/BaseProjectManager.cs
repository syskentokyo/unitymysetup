using System;
using System.IO;
using System.Linq;
using System.Text;
using tokyo.sysken.CommonDefine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace SyskenTLib.MySetUp.Editor
{
    enum SetupStatus:int
    {
        Init=0,
        ImportDefaultPackage=100,
        CreateDirectory=200,
        UnityProjectSetting=300,
        AddLayer=400,
        
        End=9000
    }


    public class BaseProjectManager
    {

        public Action _completedSetUpAction;

        private STLibBaseSetupConfig _currentBaseSetupConfig;
        private STLibUnityProjectSetupConfig _currentUnityProjectSetupConfig;

        private SetupStatus _currentSetupStatus = SetupStatus.Init;

        private int _currentPackageIndex = 0;
        private AddRequest _currentAddRequest = null;
        
        
        
        //現在のログテキスト
        private string _currentLogTxt = "";
        


        public void InitReadConfig()
        {
            _currentBaseSetupConfig = SearchSetUpConfig();
            _currentUnityProjectSetupConfig = SearchUnityProjectSetUpConfig();
        }

        public void StartSetup()
        {
            EditorApplication.update += OnEditorUpdate;

            InitReadConfig();
            
            _currentSetupStatus = SetupStatus.Init;
            _currentLogTxt += "環境設定"+"\n";
            _currentLogTxt += "CurrentDirectory:"+Environment.CurrentDirectory+"\n";
            _currentLogTxt += "Version:"+Environment.Version+"\n";
            _currentLogTxt += "MachineName:"+Environment.MachineName+"\n";
            _currentLogTxt += "UserName:"+Environment.UserName+"\n";
            _currentLogTxt += "OSVersion:"+Environment.OSVersion+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += "UnityVersion:"+Application.unityVersion+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            
            _currentLogTxt += "================="+"\n";
            _currentLogTxt += "================="+"\n";
            _currentLogTxt += "プロジェクトの設定開始"+"\n";
            _currentLogTxt += ""+DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += "================="+"\n";
            
            //
            StartNextProcess();
        }





        #region 共通

        private void StartNextProcess()
        {
            switch (_currentSetupStatus)
            {
                case SetupStatus.Init:
                {
                    _currentSetupStatus = SetupStatus.ImportDefaultPackage;
                    StartDefaultPackage();
                    break;
                }
                case SetupStatus.ImportDefaultPackage:
                {
                    _currentSetupStatus = SetupStatus.CreateDirectory;
                    StartCreateDirectory();
                    break;
                }
                case SetupStatus.CreateDirectory:
                {
                    _currentSetupStatus = SetupStatus.UnityProjectSetting;
                    StartUnityProjectSetting();
                    break;
                }
                case SetupStatus.UnityProjectSetting:
                {
                    _currentSetupStatus = SetupStatus.AddLayer;
                    StartAddLayerSetting();
                    break;
                }

                case SetupStatus.AddLayer:
                    _currentSetupStatus = SetupStatus.End;
                    CompleteSetup();
                    break;

            }
        }

        private void OnEditorUpdate()
        {
            switch (_currentSetupStatus)
            {
                case SetupStatus.Init:
                {

                    break;
                }
                case SetupStatus.ImportDefaultPackage:
                {
                    UpdateDefaultPackage();
                    break;
                }
                case SetupStatus.CreateDirectory:
                {

                    break;
                }
                case SetupStatus.UnityProjectSetting:
                {

                    break;
                }

                case SetupStatus.AddLayer:
                {
                    break;
                }

            }
        }


        private STLibBaseSetupConfig SearchSetUpConfig()
        {
            STLibBaseSetupConfig baseSetupConfig = null;
            string[] guids = AssetDatabase.FindAssets("t:STLibBaseSetupConfig");
            guids.ToList().ForEach(nextGUID =>
            {
                string filePath = AssetDatabase.GUIDToAssetPath(nextGUID);
                baseSetupConfig = AssetDatabase.LoadAssetAtPath<STLibBaseSetupConfig>(filePath);

            });

            return baseSetupConfig;
        }
        
        private STLibUnityProjectSetupConfig SearchUnityProjectSetUpConfig()
        {
            STLibUnityProjectSetupConfig nextSetupConfig = null;
            string[] guids = AssetDatabase.FindAssets("t:STLibUnityProjectSetupConfig");
            guids.ToList().ForEach(nextGUID =>
            {
                string filePath = AssetDatabase.GUIDToAssetPath(nextGUID);
                nextSetupConfig = AssetDatabase.LoadAssetAtPath<STLibUnityProjectSetupConfig>(filePath);

            });

            return nextSetupConfig;
        }

        private string ReadLayerSettingFile()
        {
            string rawText = "";

            string targetDirPath = Application.dataPath + "/../ProjectSettings";
            string targetFilePath = targetDirPath + "/" + "TagManager.asset";
            rawText = File.ReadAllText(targetFilePath);
            return rawText;
        }

        private void SaveLayerSettingFile(string rawText)
        {

            string targetDirPath = Application.dataPath + "/../ProjectSettings";
            string targetFilePath = targetDirPath + "/" + "TagManager.asset";

            File.WriteAllText(targetFilePath, rawText);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion

        #region デフォルトパッケージ


        private void StartDefaultPackage()
        {
            _currentLogTxt += "おすすめのパッケージインポート"+"\n";
            _currentPackageIndex = 0;
            StartImportDefaultPackage();
        }

        private void StartImportDefaultPackage()
        {
            string nextPackageID = _currentBaseSetupConfig.GetTargetPackageIDList[_currentPackageIndex];
            _currentLogTxt += "次のパッケージ:" + nextPackageID+"\n";
            _currentAddRequest = Client.Add(nextPackageID);
        }

        private void UpdateDefaultPackage()
        {
            if (_currentAddRequest == null) return;

            if (_currentAddRequest.Status == StatusCode.Success)
            {
                _currentAddRequest = null;

                _currentPackageIndex++;

                if (_currentPackageIndex < _currentBaseSetupConfig.GetTargetPackageIDList.Count)
                {
                    //次のインポート開始
                    StartImportDefaultPackage();
                }
                else
                {
                    //すべてインポート終了
                    _currentLogTxt += "すべてインポート終了"+"\n";
                    StartNextProcess(); //次の処理
                }
            }
        }


        #endregion

        #region ディレクトリ作成
        
        
        private void StartCreateDirectory()
        {
            _currentLogTxt += "================"+"\n";
            _currentLogTxt += "ディレクトリの作成を開始"+"\n";
            CreateAllDirectoryProcess();

            StartNextProcess(); //次の処理
        }

        private void CreateAllDirectoryProcess()
        {
            _currentBaseSetupConfig.GetCreateDirectoryPathList.ForEach(dirPath =>
            {
                if (dirPath != "")
                {
                    CreateDirectoryProcess(dirPath);
                }
            });
        }

        private void CreateDirectoryProcess(string dirPath)
        {
            _currentLogTxt += "ディレクトリ作成:" + dirPath+"\n";
            string saveDirPath = Path.GetDirectoryName(Application.dataPath);
            string createDirPath = saveDirPath + "/" + dirPath;
            string gitkeepPath = createDirPath + "/" + ".gitkeep";
            if (Directory.Exists(createDirPath) == false)
            {
                //サブディレクトリ作成
                Directory.CreateDirectory(createDirPath);
            }

            //GitKeep追加
            if (File.Exists(gitkeepPath) == false)
            {
                File.WriteAllText(gitkeepPath, "");
            }
        }


        #endregion

        #region UnityProjectSetting
        
        private void StartUnityProjectSetting()
        {
            _currentLogTxt += "============="+"\n";
            _currentLogTxt += "UnityProjectSettingの設定開始"+"\n";
            StartUnityProjectSettingProcess();

            StartNextProcess(); //次の処理
        }

        private void StartUnityProjectSettingProcess()
        {
            StartOverwriteAppIDProcess();

            StartOverwriteAppName();
            StartOverwriteCompanyName();

            StartPlatformCommonGeneral();

            StartOverwriteWindowsSetting();
            StartOverwriteIOSSetting();
            StartOverwriteAndroidSetting();

            StartEditorOnProjectSetting();
        }

        private void StartOverwriteAppIDProcess()
        {
            _currentLogTxt += "=========="+"\n";
            _currentLogTxt += "アプリIDの設定"+"\n";
            
            
            string overwriteAppID = "";

            switch (_currentUnityProjectSetupConfig.GetOverwriteAppIDType)
            {
                case UnityProjectAppIDType.None:
                {
                    _currentLogTxt += "アプリIDは設定しません。"+"\n";
                    return;
                }

                case UnityProjectAppIDType.OverwriteBaseID:
                {
                    overwriteAppID = _currentUnityProjectSetupConfig.GetBaseAppID;
                    _currentLogTxt += "アプリIDを変更:"+overwriteAppID+"\n";
                    break;
                }

                case UnityProjectAppIDType.OverwriteBaseIDAndAddRandomID:
                {
                    string dateTxt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    overwriteAppID = _currentUnityProjectSetupConfig.GetBaseAppID + ".app" + dateTxt;
                    _currentLogTxt += "アプリIDを変更:"+overwriteAppID+"\n";
                    break;
                }
            }

            if (overwriteAppID == "")
            {
                return;
            }

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, overwriteAppID);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, overwriteAppID);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, overwriteAppID);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.tvOS, overwriteAppID);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WebGL, overwriteAppID);
            

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        private void StartOverwriteAppName()
        {
            string appName = _currentUnityProjectSetupConfig.GetAppName;
            if (appName != "")
            {
                _currentLogTxt += "=============="+"\n";
                _currentLogTxt += "アプリ名を上書き:" + appName+"\n";
                PlayerSettings.productName = appName;
            }

        }

        private void StartOverwriteCompanyName()
        {
            string companyName = _currentUnityProjectSetupConfig.GetCompanyName;
            if (companyName != "")
            {
                _currentLogTxt += "=============="+"\n";
                _currentLogTxt += "会社名を上書き:" + companyName+"\n";
                PlayerSettings.companyName = companyName;
            }
        }

        private void StartPlatformCommonGeneral()
        {
            string appversion = "1.0.0";
            _currentLogTxt += "=============="+"\n";
            _currentLogTxt += "アプリバージョンを上書き:" + appversion+"\n";
            PlayerSettings.bundleVersion = appversion;
            
            
            //
            //画面の回転
            //
            switch (_currentUnityProjectSetupConfig.GetDeviceRotation)
            {
                case UnityDeviceRotation.Auto:
                    break;
                case UnityDeviceRotation.Portrait:
                    PlayerSettings.useAnimatedAutorotation = false;
                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
                    PlayerSettings.allowedAutorotateToPortrait = false;
                    break;
                case UnityDeviceRotation.PortraitUpsideDown:
                    PlayerSettings.useAnimatedAutorotation = false;
                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.PortraitUpsideDown;
                    PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                    break;
                case UnityDeviceRotation.LandscapeRight:
                    PlayerSettings.useAnimatedAutorotation = false;
                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeRight;
                    PlayerSettings.allowedAutorotateToLandscapeRight = false;
                    break;
                case UnityDeviceRotation.LandscapeLeft:
                    PlayerSettings.useAnimatedAutorotation = false;
                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
                    PlayerSettings.allowedAutorotateToLandscapeLeft = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void StartOverwriteWindowsSetting()
        {
            _currentLogTxt += "===================="+"\n";
            _currentLogTxt += "Windows用設定"+"\n";
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new GraphicsDeviceType[]{GraphicsDeviceType.Direct3D12});
            _currentLogTxt += "Windows: グラフィック  " + PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64)[0]+"\n";

            PlayerSettings.useHDRDisplay = true;
            _currentLogTxt += "Windows: ディスプレイHDR対応：  " + PlayerSettings.useHDRDisplay+"\n";
        }


        private void StartOverwriteIOSSetting()
        {
            _currentLogTxt += "===================="+"\n";
            _currentLogTxt += "iOS用設定"+"\n";
            
            
            _currentLogTxt += "iOS:AutomaticSign上書き " + _currentUnityProjectSetupConfig.GetIOSTurnONAutomaticSign+"\n";
            PlayerSettings.iOS.appleEnableAutomaticSigning = _currentUnityProjectSetupConfig.GetIOSTurnONAutomaticSign;

            string teamID = _currentUnityProjectSetupConfig.GetIOSTeamID;
            if (teamID != "")
            {
                _currentLogTxt += "iOS:TeamID上書き " + teamID+"\n";
                PlayerSettings.iOS.appleDeveloperTeamID = teamID;
            }

            _currentLogTxt += "iOS:サポートOSバージョン上書き " + _currentUnityProjectSetupConfig.GetIOSSupportMinOSVersion+"\n";
            PlayerSettings.iOS.targetOSVersionString = _currentUnityProjectSetupConfig.GetIOSSupportMinOSVersion;


            string cameraUsage = _currentUnityProjectSetupConfig.GetIOSCamraUsage;
            if (cameraUsage != "")
            {
                _currentLogTxt += "iOS:カメラ利用理由上書き " + cameraUsage+"\n";
                PlayerSettings.iOS.cameraUsageDescription = cameraUsage;
            }

            string microphoneUsage = _currentUnityProjectSetupConfig.GetIOSMicrophoneUsage;
            if (microphoneUsage != "")
            {
                _currentLogTxt += "iOS:マイク利用理由上書き " + microphoneUsage+"\n";
                PlayerSettings.iOS.microphoneUsageDescription = microphoneUsage;
            }

            string locationUsage = _currentUnityProjectSetupConfig.GetIOSLocationUsage;
            if (locationUsage != "")
            {
                _currentLogTxt += "iOS:現在地利用理由上書き " + locationUsage+"\n";
                PlayerSettings.iOS.locationUsageDescription = teamID;
            }

            PlayerSettings.iOS.deferSystemGesturesMode = SystemGestureDeferMode.All;
            _currentLogTxt += "iOS:コントロールセンターを一発で開けないようにします "+"\n";



        }


        private void StartOverwriteAndroidSetting()
        {
            _currentLogTxt += "===================="+"\n";
            _currentLogTxt += "Android用設定"+"\n";

            //
            //
            //
            _currentLogTxt += "Android:サポートOSバージョン上書き " + _currentUnityProjectSetupConfig.GetAndroidSupportMinOSVersion+"\n";
            PlayerSettings.Android.minSdkVersion = _currentUnityProjectSetupConfig.GetAndroidSupportMinOSVersion;


            //
            //
            //
            AndroidSdkVersions _androidTargetOSVersion = _currentUnityProjectSetupConfig.GetAndroidTargetOSVersion;
            int targetOSVersionNum = (int)_androidTargetOSVersion;
            _androidTargetOSVersion = (AndroidSdkVersions)(targetOSVersionNum +
                                                           _currentUnityProjectSetupConfig
                                                               .GetAndroidTargetOSVersionAddNum);
            
            _currentLogTxt += "Android:ターゲットOSバージョン上書き " + _androidTargetOSVersion+"\n";
            PlayerSettings.Android.targetSdkVersion = _androidTargetOSVersion;

            
            //
            //
            //
            ScriptingImplementation targetScriptingImplementation = ScriptingImplementation.IL2CPP;
            _currentLogTxt += "Android:ScriptBackend  " + targetScriptingImplementation+"\n";
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, targetScriptingImplementation);

            //
            //
            //

            AndroidArchitecture targetArchitecture = AndroidArchitecture.ARM64;
            _currentLogTxt += "Android:アーキテクチャ  " + targetArchitecture+"\n";
            PlayerSettings.Android.targetArchitectures = targetArchitecture;
            
            //
            //
            //
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new GraphicsDeviceType[]{GraphicsDeviceType.OpenGLES3});
            _currentLogTxt += "Android: グラフィック  " + PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)[0]+"\n";

        }

        private void StartEditorOnProjectSetting()
        {
            _currentLogTxt += "===================="+"\n";
            _currentLogTxt += "UnityEditor設定"+"\n";
            
            
            //
            // オブジェクト複製時の名前法則
            //
            EditorSettings.gameObjectNamingScheme = EditorSettings.NamingScheme.Underscore;
            EditorSettings.gameObjectNamingDigits = 2;
            _currentLogTxt += "GameObject複製時の名前変更："+EditorSettings.gameObjectNamingScheme +"  桁："+EditorSettings.gameObjectNamingDigits+"\n";
            
            EditorSettings.refreshImportMode = AssetDatabase.RefreshImportMode.OutOfProcessPerQueue;
            EditorUserSettings.desiredImportWorkerCount = 4;
            _currentLogTxt += "インポート時のプロセス数："+EditorUserSettings.desiredImportWorkerCount+"\n";
            AssetDatabase.SaveAssets();
            // EditorUserSettings.
        }
        

        #endregion

        #region UnityLayerSetting
        
        
        private void StartAddLayerSetting()
        {
            _currentLogTxt += "===================="+"\n";
            _currentLogTxt += "UnityのLayerの設定開始"+"\n";


            StartAddLayerProcess();

            StartNextProcess(); //次の処理
        }

        private void StartAddLayerProcess()
        {
            string layerSettingText = ReadLayerSettingFile();

            string targetLayerText = _currentBaseSetupConfig.GetOriginalLayerSetting;
            
            string replaceLayerText = CreateAddLayerSettingText(CountTargetText(targetLayerText,"-"));
            _currentLogTxt += "AddLayer:"+replaceLayerText+"\n";

            string savedText = layerSettingText.Replace(targetLayerText, replaceLayerText);
            
            SaveLayerSettingFile(savedText);
        }

        private string CreateAddLayerSettingText(int lineTotalNum)
        {
            string rawText = "";
            
            rawText += "  - " +"Default"+ "\n";
            rawText += "  - " +"TransparentFX"+ "\n";
            rawText += "  - " +"Ignore Raycast"+ "\n";
            rawText += "  - " +""+ "\n";
            rawText += "  - " +"Water"+ "\n";
            rawText += "  - " +"UI"+ "\n";
            rawText += "  - " + CommonLayer.Layer.None6.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None7.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None8.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None9.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.Player.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.PlayerItem.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None12.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None13.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None14.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.Enemy.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.EnemyItem.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None17.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None18.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None19.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None20.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None21.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.FreeItem.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.BoundArea.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None24.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None25.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None26.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.ResultRender.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None28.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None29.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None30.ToString() + "\n";
            rawText += "  - " + CommonLayer.Layer.None31.ToString() + "\n";
            

            return rawText;
        }
    
        private int CountTargetText(string targetText, string searchText) 
        {
            return targetText.Length - targetText.Replace(searchText, "").Length;
        }

    #endregion


        #region 終了系

        private void CompleteSetup()
        {
            EditorApplication.update -= OnEditorUpdate;
            
            _completedSetUpAction?.Invoke();//終了の通知
            
            //
            //ログ出力
            //
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            _currentLogTxt += ""+"\n";
            Debug.Log(_currentLogTxt);
            
            //ファイルへ出力
            string desktopDirectoryPath =
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = "unitylog" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            string saveFilePath = Path.Combine(desktopDirectoryPath, fileName);
            Debug.Log("ログファイル： "+saveFilePath);
            File.WriteAllText(saveFilePath,_currentLogTxt,Encoding.UTF8);

        }
        
        

        #endregion
    }
}