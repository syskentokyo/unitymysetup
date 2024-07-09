using UnityEditor;
using UnityEngine;

namespace SyskenTLib.MySetUp.Editor
{
    public class SetupWindow : EditorWindow
    {
        private BaseProjectManager _baseProjectManager;
        
        public static void ShowSetUpWindow()
        {
            ShowWindow();;
        }
        
        
        [MenuItem("SyskenTLib/MySetupProject/SetUp", priority = 1)]
        private static void ShowWindow()
        {
            var window = GetWindow<SetupWindow>();
            window.titleContent = new UnityEngine.GUIContent("SetupWindow");
            window.Show();
        }

        private void OnGUI()
        {
            if (_baseProjectManager == null)
            {
                _baseProjectManager = new BaseProjectManager();
                _baseProjectManager._completedSetUpAction += OnCompleteSetUp;
            }
            
            
            EditorGUILayout.BeginVertical("Box");



            EditorGUILayout.LabelField("Init");
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("プロジェクトへ最低限の設定を行います");

            if (GUILayout.Button("Start SetUp Project"))
            {
                //適当なバーを出す
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayProgressBar("Wait", "処理中", 0.23f);
                
                
                _baseProjectManager.StartSetup();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(30);
        }

        private void OnCompleteSetUp()
        {
            Debug.Log("初期設定終わり");
            EditorUtility.ClearProgressBar();
            
            this.Close();
        }
    }
}