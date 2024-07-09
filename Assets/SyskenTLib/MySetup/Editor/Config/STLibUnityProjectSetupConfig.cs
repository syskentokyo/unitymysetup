using UnityEditor;
using UnityEngine;

namespace SyskenTLib.MySetUp.Editor
{

    public enum UnityProjectAppIDType:int
    {
        None=-99,
        OverwriteBaseID=100,
        OverwriteBaseIDAndAddRandomID=200,
    }

    public enum UnityDeviceRotation : int
    {
        Auto=0
        ,Portrait=1
        ,PortraitUpsideDown=2
        ,LandscapeRight=3
        ,LandscapeLeft=4
    }
    
    
    public class STLibUnityProjectSetupConfig : ScriptableObject
    {
        
        
        [Header("---------------------------------")]

        [Header("共通")]
        [Header("アプリIDを上書きするか")]
        [SerializeField] private UnityProjectAppIDType _overwriteAppIDType = UnityProjectAppIDType.None;
        public UnityProjectAppIDType GetOverwriteAppIDType => _overwriteAppIDType;
        
        [Header("上書きするアプリID")]
        [SerializeField] private string _baseAppID = "";
        public string GetBaseAppID => _baseAppID;

        [Header("画面の回転")] 
        [SerializeField] private UnityDeviceRotation _deviceRotation = UnityDeviceRotation.Portrait;
        public UnityDeviceRotation GetDeviceRotation => _deviceRotation;
        

        [Header("アプリ名")]
        [SerializeField] private string _appName = "demoapp";
        public string GetAppName => _appName;
        
        [Header("会社名")]
        [SerializeField] private string _companyName = "";
        public string GetCompanyName => _companyName;
        
        
        [Header("---------------------------------")]
        
        [Header("上書きするiOS系の設定")]
        [SerializeField] private string _IOSTeamID = "";
        public string GetIOSTeamID => _IOSTeamID;
        
        [SerializeField] private bool _IOSTurnONAutomaticSign = false;
        public bool GetIOSTurnONAutomaticSign => _IOSTurnONAutomaticSign;

        [SerializeField] private string _IOSSupportMinOSVersion = "17.5";
        public string GetIOSSupportMinOSVersion => _IOSSupportMinOSVersion;
        
        [SerializeField] private string _IOSCameraUsage = "";
        public string GetIOSCamraUsage => _IOSCameraUsage;
        
        [SerializeField] private string _IOSMicrophoneUsage = "";
        public string GetIOSMicrophoneUsage => _IOSMicrophoneUsage;
        
        [SerializeField] private string _IOSLocationUsage = "";
        public string GetIOSLocationUsage => _IOSLocationUsage;
        
        
        [Header("---------------------------------")]

        
        [Header("上書きするAndroid系の設定")]
        [SerializeField] private AndroidSdkVersions _androidSupportMinOSVersion = AndroidSdkVersions.AndroidApiLevel32;
        public AndroidSdkVersions GetAndroidSupportMinOSVersion => _androidSupportMinOSVersion;
        
        
        [SerializeField] private AndroidSdkVersions _androidTargetOSVersion = AndroidSdkVersions.AndroidApiLevel34;
        public AndroidSdkVersions GetAndroidTargetOSVersion => _androidTargetOSVersion;

        [Header("AndroidのターゲットOSバージョンに加算するバージョン数")]
        [SerializeField] private int _androidTargetOSVersionAddNum = 1;
        public int GetAndroidTargetOSVersionAddNum => _androidTargetOSVersionAddNum;

        
        // [Header("---------------------------------")]

        
    }
}