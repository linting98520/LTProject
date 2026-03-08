using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using static GeometryController;
using System.Text.RegularExpressions;

[CustomEditor(typeof(GeometryController))]
public class GeometryControllerInspector : Editor
{
    GeometryController controller;

    private void OnEnable()
    {
        controller = (GeometryController)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        var spriteRenderer = new PropertyField(serializedObject.FindProperty("TemplateSpriteRenderer"), "原圖");
        root.Add(spriteRenderer);

        root.AddSpace(10);

        var divideP1 = new Vector2Field("分割線 01");
        divideP1.BindProperty(serializedObject.FindProperty(nameof(GeometryController.DivideLineA)));
        root.Add(divideP1);

        var divideP2 = new Vector2Field("分割線 02");
        divideP2.BindProperty(serializedObject.FindProperty(nameof(GeometryController.DivideLineB)));
        root.Add(divideP2);

        root.AddSpace();

        var showSphereRadius = new FloatField("點大小");
        showSphereRadius.BindProperty(serializedObject.FindProperty(nameof(GeometryController.ShowSphereRadius)));
        root.Add(showSphereRadius);

        root.AddSpace();

        #region spriteButtonArea
        var spriteButtonArea = VisualElementUtility.CreateHorizontalBox();
        spriteButtonArea.style.alignItems = Align.Center;
        root.Add(spriteButtonArea);

        ObjectField objectPicker = new ObjectField("目標物件")
        {
            objectType = typeof(SpriteRenderer),
            allowSceneObjects = true,
            style = { flexShrink = 1, flexBasis = 0, flexGrow = 1}
        };
        spriteButtonArea.Add(objectPicker);

        Button showVerticesBtn = new Button { text = "重設數值" };
        showVerticesBtn.style.flexShrink = 0;
        showVerticesBtn.style.width = 100;
        showVerticesBtn.RegisterCallback<ClickEvent>(evt =>
        {
            controller.ShowSpriteVertices(objectPicker.value as SpriteRenderer);
        });
        spriteButtonArea.Add(showVerticesBtn);
        #endregion

        root.AddSpace();

        Button divideButton = new Button(() =>
        {
            controller.ClassifyVertices();
        });
        divideButton.text = "Divide";
        root.Add(divideButton);

        root.AddSpace();

        Button createButton = new Button(() =>
        {
            controller.CreateVertices();
        });
        createButton.text = "CreateVertices";
        root.Add(createButton);

        root.AddSpace();

        Button clearButton = new Button(() =>
        {
            controller.Clear();
        });
        clearButton.text = "Clear";
        root.Add(clearButton);

        root.AddSpace(10);

        #region List Property
        var showListProperty = serializedObject.FindProperty("TriangleShowList");
        var showListField = new PropertyField(showListProperty, "顯示清單");
        root.Add(showListField);
        #endregion

        //預設編輯器
        //InspectorElement.FillDefaultInspector(element, serializedObject, this);

        return root;
    }

    private void OnSceneGUI()
    {
        controller.DivideLineA = DrawHandle("P1", controller.DivideLineA);
        controller.DivideLineB = DrawHandle("P2", controller.DivideLineB);
        Handles.color = Color.red;
        Handles.DrawLine(controller.DivideLineA, controller.DivideLineB);
    }

    private Vector3 DrawHandle(string text, Vector3 p)
    {
        Vector3 pos = Handles.PositionHandle(p, Quaternion.identity);
        Handles.Label(pos - new Vector3(0.05f, 0.05f, 0), text);
        return pos;
    }
}

[CustomPropertyDrawer(typeof(DrawTriangleData))]
public class DrawTriangleDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        #region bool Field
        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;

        var label = new Label("顯示");
        label.style.minWidth = 60;

        var showProp = property.FindPropertyRelative("Show");
        var toggle = new Toggle();
        toggle.bindingPath = showProp.propertyPath;

        row.Add(label);
        row.Add(toggle);
        #endregion

        root.Add(row);
        root.Add(new PropertyField(property.FindPropertyRelative("vertices"), "座標"));

        return root;
    }

    int GetIndex(SerializedProperty property)
    {
        var match = Regex.Match(property.propertyPath, @"\d+");
        return match.Success ? int.Parse(match.Value) : 0;
    }
}

[CustomPropertyDrawer(typeof(ComplexItem))]
public class ComplexItemDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        var foldout = new Foldout();
        foldout.text = "物件資料";

        foldout.Add(new PropertyField(property.FindPropertyRelative("isEnabled"), "啟用"));
        foldout.Add(new PropertyField(property.FindPropertyRelative("speed"), "速度"));
        foldout.Add(new PropertyField(property.FindPropertyRelative("health"), "HP"));
        foldout.Add(new PropertyField(property.FindPropertyRelative("tags"), "標籤"));
        foldout.Add(new PropertyField(property.FindPropertyRelative("values"), "數值"));
        foldout.Add(new PropertyField(property.FindPropertyRelative("position"), "位置"));
        foldout.Add(new PropertyField(property.FindPropertyRelative("name"), "名稱"));
        root.Add(foldout);
        return root;
    }
}