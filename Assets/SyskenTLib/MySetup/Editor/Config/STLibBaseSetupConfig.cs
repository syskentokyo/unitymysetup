using System.Collections.Generic;
using UnityEngine;

namespace SyskenTLib.MySetUp.Editor
{
    
    public class STLibBaseSetupConfig : ScriptableObject
    {
        [Header("自動インストールするパッケージのID")]
        [SerializeField] private List<string> _targetPackageIDList = new List<string>();
        public List<string> GetTargetPackageIDList => _targetPackageIDList;
        
        
        [Header("作成するフォルダのパス")]
        [SerializeField] private List<string> _createDirectoryPathList = new List<string>();
        public List<string> GetCreateDirectoryPathList => _createDirectoryPathList;


        [Header("レイヤー")] [TextArea(minLines:200,maxLines:400)] [SerializeField] private string _originalLayerSetting = "";
        public string GetOriginalLayerSetting => _originalLayerSetting;
    }
}