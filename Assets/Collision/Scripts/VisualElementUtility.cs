using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementUtility
{
    public static void AddSpace(this VisualElement element, float height = 10.0f)
    {
        var space = new VisualElement();
        space.style.height = height;
        element.Add(space);
    }

    public static void AddDivide(this VisualElement element, float thickness = 1.0f)
    {
        var line = new VisualElement();
        line.style.height = thickness;
        line.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
        line.style.marginBottom = 4;
        line.style.marginTop = 4;
        element.Add(line);
    }

    public static VisualElement CreateHorizontalBox()
    {
        VisualElement box = new VisualElement();
        box.style.flexDirection = FlexDirection.Row;
        box.style.alignItems = Align.FlexStart;
        return box;
    }
}
