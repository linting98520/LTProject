using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(CollisionController))]
public class CollisionControllerInspector : Editor
{
    private CollisionController controller;
    private StyleSheet panelStyleSheet;

    public override VisualElement CreateInspectorGUI()
    {
        if (controller == null)
            controller = (CollisionController)target;

        if (panelStyleSheet == null)
        {
            panelStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Collision/Scripts/StyleSheets/InspectorPanel.uss");
        }

        VisualElement root = new VisualElement();
        root.styleSheets.Add(panelStyleSheet);

        var box = new VisualElement();
        box.AddToClassList("my-box-style");
        root.Add(box);

        Label label = new Label("Collision控制器");
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.fontSize = 12;
        box.Add(label);
        box.AddDivide();

        PropertyField collisionList = new PropertyField(serializedObject.FindProperty("CollisionPoints"), "碰撞點");
        box.Add(collisionList);

        PropertyField atkCol2D = new PropertyField(serializedObject.FindProperty("AtkCollider2D"), "攻擊者");
        PropertyField atkPointList = new PropertyField(serializedObject.FindProperty("atkPoints"), "攻擊者頂點");
        box.Add(atkPointList);
        box.AddSpace();

        PropertyField defCol2D = new PropertyField(serializedObject.FindProperty("DefCollider2D"), "受擊者");
        PropertyField defPointList = new PropertyField(serializedObject.FindProperty("defPoints"), "受擊者頂點");
        box.Add(defPointList);

        box.Add(atkCol2D);
        box.Add(defCol2D);
        box.AddSpace();

        Button getColIntersectionButton = new Button(() =>
        {
            controller.StartCollisionIntersection();
        })
        { text = "交點碰撞" };
        box.Add(getColIntersectionButton);

        Button getColInnerIntersectionButton = new Button(() =>
        {
            controller.StartCollisionInnerIntersection();
        })
        { text = "內部碰撞" };
        box.Add(getColInnerIntersectionButton);

        Button getColOverlapPointsButton = new Button(() =>
        {
            controller.StartCollisionOverlapPoints();
        })
        { text = "重疊面積頂點" };
        box.Add(getColOverlapPointsButton);

        #region Gizmos
        var gizmosBox = new VisualElement();
        gizmosBox.AddToClassList("my-box-style");
        root.Add(gizmosBox);

        Label gizmosLabel = new Label("Gizmos設定");
        PropertyField gizmosSphereSize = new PropertyField(serializedObject.FindProperty("Gizmos_SphereSize"), "碰撞點大小");
        gizmosBox.Add(gizmosLabel);
        gizmosBox.AddDivide();

        gizmosBox.Add(gizmosSphereSize);
        gizmosBox.AddSpace();

        PropertyField gizmosShowCollisionPoint = new PropertyField(serializedObject.FindProperty("Gizmos_ShowCollisionPoint"), "顯示碰撞點");
        PropertyField gizmosCollisionPointColor = new PropertyField(serializedObject.FindProperty("Gizmos_CollisionPointColor"), "碰撞點顏色");

        gizmosBox.Add(gizmosShowCollisionPoint);
        gizmosBox.Add(gizmosCollisionPointColor);
        gizmosBox.AddSpace();

        PropertyField gizmosShowColliderPoint = new PropertyField(serializedObject.FindProperty("Gizmos_ShowColliderPoint"), "顯示碰撞框頂點");
        PropertyField gizmosAtkColor = new PropertyField(serializedObject.FindProperty("Gizmos_atkColliderColor"), "攻擊碰撞框顏色");
        PropertyField gizmosDefColor = new PropertyField(serializedObject.FindProperty("Gizmos_defColliderColor"), "受擊碰撞框顏色");

        gizmosBox.Add(gizmosShowColliderPoint);
        gizmosBox.Add(gizmosAtkColor);
        gizmosBox.Add(gizmosDefColor);
        #endregion

        return root;
    }
}
