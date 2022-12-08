using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DefaultScene : EditorWindow
{
    void OnGUI()
    {
        EditorSceneManager.playModeStartScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"), EditorSceneManager.playModeStartScene, typeof(SceneAsset), false);
    }

    [MenuItem("Tools/Default Scene")]
    static void Open()
    {
        GetWindow<DefaultScene>();
    }
}