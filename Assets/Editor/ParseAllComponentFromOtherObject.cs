using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ParseAllComponentFromOtherObject : EditorWindow
{
    private GameObject oldObject;
    private GameObject newObject;
    [MenuItem("Tuhai993/Utility/ParseAllComponentFromOTherObject")]
    static void ParseAllComponentFromOTherObject()
    {
        //EditorSceneManager.OpenScene("Assets/Start.unity");
        Init();
    }
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ParseAllComponentFromOtherObject window = (ParseAllComponentFromOtherObject)EditorWindow.GetWindow(typeof(ParseAllComponentFromOtherObject));
        window.Show();
    }
    void OnGUI()
    {
        var labelStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter,fontStyle = FontStyle.Bold};
        GUILayout.Label("COPY UNITY COMPONENTS", labelStyle);
        GUILayout.Label("This not work with component that already exist in NewObject(such as Transform), you should remove old removeable component to get a new one.", EditorStyles.helpBox);
        var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Old Object", style);
        GUILayout.Label("Drag object that you want get components here", EditorStyles.centeredGreyMiniLabel);
        oldObject = (GameObject)EditorGUILayout.ObjectField(oldObject,typeof(GameObject),true);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("New Object", style);
        GUILayout.Label("Drag object that you want receives components here", EditorStyles.centeredGreyMiniLabel);
        newObject = (GameObject)EditorGUILayout.ObjectField(newObject,typeof(GameObject),true);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("PARSE"))
        {
            var allOldTransf = oldObject.GetComponentsInChildren<Transform>();
            var allNewTransf = newObject.GetComponentsInChildren<Transform>();
            foreach (var oldTransf in allOldTransf)
            {
                foreach (var newTransf in allNewTransf)
                {
                    if(string.CompareOrdinal(oldTransf.name,newTransf.name) != 0) continue;
                    ParseComponent(oldTransf, newTransf);
                    break;
                }
            }
        }
        EditorGUI.EndDisabledGroup();
    }

    private void ParseComponent(Transform objA,Transform objB)
    {
        var allComponent = objA.GetComponents<Component>();
        foreach (var component in allComponent)
        {
            if(component.GetType() == typeof(Transform)) continue;
            Component new_component = objB.gameObject.AddComponent(component.GetType());
            if (UnityEditorInternal.ComponentUtility.CopyComponent(component))
            {
                UnityEditorInternal.ComponentUtility.PasteComponentValues(new_component);
            }
            // foreach (FieldInfo f in allComponent.GetType().GetFields())
            // {
            //     f.SetValue(new_component, f.GetValue(component));
            // }
        }
    }
}
