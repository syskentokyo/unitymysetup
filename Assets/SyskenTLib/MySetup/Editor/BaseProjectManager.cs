using System;
using System.IO;
using System.Linq;
using tokyo.sysken.CommonDefine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Rendering;

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
            Debug.Log("おすすめのパッケージインポート");
            _currentPackageIndex = 0;
            StartImportDefaultPackage();
        }

        private void StartImportDefaultPackage()
        {
            string nextPackageID = _currentBaseSetupConfig.GetTargetPackageIDList[_currentPackageIndex];
            Debug.Log("次のパッケージ:" + nextPackageID);
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
                    Debug.Log("すべてインポート終了");
                    StartNextProcess(); //次の処理
                }
            }
        }


        #endregion

        #region ディレクトリ作成

        public void CreateAllDirectory()
        {

            CreateAllDirectoryProcess();

        }
        
        private void StartCreateDirectory()
        {
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
            Debug.Log("ディレクトリ作成します。 " + dirPath);
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
            Debug.Log("UnityProjectSettingの設定開始");

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
            string overwriteAppID = "";

            switch (_currentUnityProjectSetupConfig.GetOverwriteAppIDType)
            {
                case UnityProjectAppIDType.None:
                {
                    return;
                }

                case UnityProjectAppIDType.OverwriteBaseID:
                {
                    overwriteAppID = _currentUnityProjectSetupConfig.GetBaseAppID;
                    break;
                }

                case UnityProjectAppIDType.OverwriteBaseIDAndAddRandomID:
                {
                    string dateTxt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    overwriteAppID = _currentUnityProjectSetupConfig.GetBaseAppID + ".app" + dateTxt;
                    break;
                }
            }

            if (overwriteAppID == "")
            {
                return;
            }

            Debug.Log("アプリIDを上書きします" + overwriteAppID);

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
                Debug.Log("アプリ名を上書き:" + appName);
                PlayerSettings.productName = appName;
            }

        }

        private void StartOverwriteCompanyName()
        {
            string companyName = _currentUnityProjectSetupConfig.GetCompanyName;
            if (companyName != "")
            {
                Debug.Log("会社名を上書き:" + companyName);
                PlayerSettings.companyName = companyName;
            }
        }

        private void StartPlatformCommonGeneral()
        {
            string appversion = "1.0.0";
            Debug.Log("アプリバージョン  " + appversion);
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
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new GraphicsDeviceType[]{GraphicsDeviceType.Direct3D12});
            Debug.Log("Windows: グラフィック  " + PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64)[0]);


            PlayerSettings.useHDRDisplay = true;
            Debug.Log("Windows: ディスプレイHDR対応：  " + PlayerSettings.useHDRDisplay );
        }


        private void StartOverwriteIOSSetting()
        {
            
            Debug.Log("iOS:AutomaticSign上書き " + _currentUnityProjectSetupConfig.GetIOSTurnONAutomaticSign);
            PlayerSettings.iOS.appleEnableAutomaticSigning = _currentUnityProjectSetupConfig.GetIOSTurnONAutomaticSign;

            string teamID = _currentUnityProjectSetupConfig.GetIOSTeamID;
            if (teamID != "")
            {
                Debug.Log("iOS:TeamID上書き " + teamID);
                PlayerSettings.iOS.appleDeveloperTeamID = teamID;
            }

            Debug.Log("iOS:サポートOSバージョン上書き " + _currentUnityProjectSetupConfig.GetIOSSupportMinOSVersion);
            PlayerSettings.iOS.targetOSVersionString = _currentUnityProjectSetupConfig.GetIOSSupportMinOSVersion;


            string cameraUsage = _currentUnityProjectSetupConfig.GetIOSCamraUsage;
            if (cameraUsage != "")
            {
                Debug.Log("iOS:カメラ利用理由上書き " + cameraUsage);
                PlayerSettings.iOS.cameraUsageDescription = cameraUsage;
            }

            string microphoneUsage = _currentUnityProjectSetupConfig.GetIOSMicrophoneUsage;
            if (microphoneUsage != "")
            {
                Debug.Log("iOS:マイク利用理由上書き " + microphoneUsage);
                PlayerSettings.iOS.microphoneUsageDescription = microphoneUsage;
            }

            string locationUsage = _currentUnityProjectSetupConfig.GetIOSLocationUsage;
            if (locationUsage != "")
            {
                Debug.Log("iOS:現在地利用理由上書き " + locationUsage);
                PlayerSettings.iOS.locationUsageDescription = teamID;
            }

            PlayerSettings.iOS.deferSystemGesturesMode = SystemGestureDeferMode.All;
            Debug.Log("iOS:コントロールセンターを一発で開けないようにします ");



        }


        private void StartOverwriteAndroidSetting()
        {


            //
            //
            //
            Debug.Log("Android:サポートOSバージョン上書き " + _currentUnityProjectSetupConfig.GetAndroidSupportMinOSVersion);
            PlayerSettings.Android.minSdkVersion = _currentUnityProjectSetupConfig.GetAndroidSupportMinOSVersion;


            //
            //
            //
            AndroidSdkVersions _androidTargetOSVersion = _currentUnityProjectSetupConfig.GetAndroidTargetOSVersion;
            int targetOSVersionNum = (int)_androidTargetOSVersion;
            _androidTargetOSVersion = (AndroidSdkVersions)(targetOSVersionNum +
                                                           _currentUnityProjectSetupConfig
                                                               .GetAndroidTargetOSVersionAddNum);
            
            Debug.Log("Android:ターゲットOSバージョン上書き " + _androidTargetOSVersion);
            PlayerSettings.Android.targetSdkVersion = _androidTargetOSVersion;

            
            //
            //
            //
            ScriptingImplementation targetScriptingImplementation = ScriptingImplementation.IL2CPP;
            Debug.Log("Android:ScriptBackend  " + targetScriptingImplementation);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, targetScriptingImplementation);

            //
            //
            //

            AndroidArchitecture targetArchitecture = AndroidArchitecture.ARM64;
            Debug.Log("Android:アーキテクチャ  " + targetArchitecture);
            PlayerSettings.Android.targetArchitectures = targetArchitecture;
            
            //
            //
            //
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new GraphicsDeviceType[]{GraphicsDeviceType.OpenGLES3});
            Debug.Log("Android: グラフィック  " + PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)[0]);

        }

        private void StartEditorOnProjectSetting()
        {

            //
            // オブジェクト複製時の名前法則
            //
            EditorSettings.gameObjectNamingScheme = EditorSettings.NamingScheme.Underscore;
            EditorSettings.gameObjectNamingDigits = 2;
            Debug.Log("GameObject複製時の名前変更："+EditorSettings.gameObjectNamingScheme +"  桁："+EditorSettings.gameObjectNamingDigits);


            Debug.Log("インポート時の設定");
            EditorSettings.refreshImportMode = AssetDatabase.RefreshImportMode.OutOfProcessPerQueue;
            EditorUserSettings.desiredImportWorkerCount = 4;
            AssetDatabase.SaveAssets();
            // EditorUserSettings.
        }
        

        #endregion

        #region UnityLayerSetting

        public void StartAddLayer()
        {
            StartAddLayerProcess();
        }
        
        
        private void StartAddLayerSetting()
        {
            Debug.Log("UnityのLayerの設定開始");

            StartAddLayerProcess();

            StartNextProcess(); //次の処理
        }

        private void StartAddLayerProcess()
        {
            string layerSettingText = ReadLayerSettingFile();

            string targetLayerText = _currentBaseSetupConfig.GetOriginalLayerSetting;
            
            string replaceLayerText = CreateAddLayerSettingText(CountTargetText(targetLayerText,"-"));
            Debug.Log("AddLayer:"+replaceLayerText);


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
        }
        
        

        #endregion
    }
}