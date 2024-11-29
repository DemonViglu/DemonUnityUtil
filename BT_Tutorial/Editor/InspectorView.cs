using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

    Editor editor;
    public InspectorView()
    {

    }

    internal void UpdateSelection(NodeView node)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(editor);

        editor = Editor.CreateEditor(node.node);

        IMGUIContainer container = new IMGUIContainer(()=>{ editor.OnInspectorGUI(); });

        Add(container);
    }
}
