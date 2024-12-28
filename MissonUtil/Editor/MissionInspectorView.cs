using UnityEditor;
using UnityEngine.UIElements;

public class MissionInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<MissionInspectorView, VisualElement.UxmlTraits> { }

    Editor editor;
    public MissionInspectorView()
    {

    }
    internal void UpdateSelection(MissionNodeView node)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(editor);

        editor = Editor.CreateEditor(node.Node);

        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });

        Add(container);
    }
}
