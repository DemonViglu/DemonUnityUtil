using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace demonviglu.MissonSystem
{
    public class DemonMissionEditor : EditorWindow
    {

        MissionFactoryView missionFactoryView;
        MissionInspectorView inspectorView;

        [MenuItem("Tools/DemonTool/Mission_Editor")]
        public static void ShowExample()
        {
            DemonMissionEditor wnd = GetWindow<DemonMissionEditor>();
            wnd.titleContent = new GUIContent("DemonMissionEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            var virtualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\DemonUtil\\MissonUtil\\Editor\\DemonMissionEditor.uxml");
            virtualTree.CloneTree(root);

            missionFactoryView = root.Q<MissionFactoryView>();
            inspectorView = root.Q<MissionInspectorView>();

            missionFactoryView.OnNodeSelectd += OnNodeSelectedChanged;

            MakeMissionFactoryView();
        }

        private void MakeMissionFactoryView()
        {
            missionFactoryView.PopulateView();
        }

        void OnNodeSelectedChanged(MissionNodeView node)
        {
            inspectorView.UpdateSelection(node);
        }
    }
}