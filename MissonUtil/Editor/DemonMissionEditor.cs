using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace demonviglu.MissonSystem
{
    public class DemonMissionEditor : EditorWindow
    {

        MissionFactoryView missionFactoryView;
        MissionInspectorView inspectorView;

        TimeTick t;

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

            Toggle toggle = root.Q<Toggle>("RefreshToggle");

            toggle.RegisterValueChangedCallback(OnToggleDown);

            t = new(0.5f);
        }

        private void OnInspectorUpdate()
        {
            if (Refresh && t.Tick())
            {
                MakeMissionFactoryView();
            }
        }

        bool Refresh = false;

        private void OnToggleDown(ChangeEvent<bool> evt)
        {
            if(evt.newValue ==  false)
            {
                Refresh = false;
            }
            else
            {
                t.Init();
                Refresh = true;
            }
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

    class TimeTick
    {
        float TimeGap;
        float TimeSet;

        public TimeTick(float time)
        {
            TimeGap = time;
            this.TimeSet = time + Time.time;
        }
        public bool Tick()
        {
            if(Time.time >= TimeSet)
            {
                TimeSet += TimeGap;
                return true;
            }
            return false;
        }

        public void Init()
        {
            TimeSet = Time.time;
        }
    }

}