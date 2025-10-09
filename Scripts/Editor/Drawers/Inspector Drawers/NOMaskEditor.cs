using UnityEditor;
using UnityEditor.UI;
using Sirenix.OdinInspector.Editor;

namespace NiqonNO.UGUI.Editor
{
    [CustomEditor(typeof(NOMask), true)]
    public class NOMaskEditor : MaskEditor
    {
        private PropertyTree propertyTree;

        protected override void OnEnable()
        {
            base.OnEnable();
            propertyTree = PropertyTree.Create(serializedObject.targetObject);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (propertyTree == null) return;

            propertyTree.BeginDraw(true);
            foreach (var inspectorProperty in propertyTree.EnumerateTree())
            {
                if (typeof(NOMask).IsAssignableFrom(inspectorProperty.Info.GetMemberInfo()?.DeclaringType))
                {
                    inspectorProperty.Draw();
                }
            }

            propertyTree.EndDraw();
        }

        protected void OnDisable()
        {
            propertyTree?.Dispose();
        }
    }
}
