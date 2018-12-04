using UnityEditor;
using UnityEngine;

namespace StageSelection
{
    [CustomEditor(typeof(WorldData))]
    public class WorldDataInspector : Editor
    {
        private WorldData _self;

        private void Awake()
        {
            _self = target as WorldData;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Draw"))
            {
                _self.GeneratePoints();
            }
        }
    }
}