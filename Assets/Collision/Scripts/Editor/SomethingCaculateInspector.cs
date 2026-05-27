using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SomethingCaculate))]
public class SomethingCaculateInspector : Editor
{
    private SomethingCaculate something;

    private StyleSheet panelStyleSheet;

    public override VisualElement CreateInspectorGUI()
    {
        if (something == null)
            something = (SomethingCaculate)target;

        if (panelStyleSheet == null)
        {
            panelStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Collision/Scripts/StyleSheets/InspectorPanel.uss");
        }

        VisualElement root = new VisualElement();
        root.styleSheets.Add(panelStyleSheet);

        var box = new VisualElement();
        box.AddToClassList("my-box-style");
        root.Add(box);

        Vector2Field pointField = new Vector2Field("起點");
        pointField.value = Vector2.zero;

        Vector2Field point1Field = new Vector2Field("點1");
        point1Field.value = Vector2.zero;
        
        Vector2Field point2Field = new Vector2Field("點2");
        point2Field.value = Vector2.zero;

        Button button = new Button(() =>
        {
            something.VectorCross(pointField.value, point1Field.value, point2Field.value);
        })
        { text = "叉積" };

        box.Add(pointField);
        box.Add(point1Field);
        box.Add(point2Field);
        box.AddSpace();
        box.Add(button);

        return root;
    }
}
