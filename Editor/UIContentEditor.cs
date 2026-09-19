
namespace TeaSpoons.UGuiDesignSystem.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityObject = UnityEngine.Object;

    [CustomEditor(typeof(UIContent))]
    public class UIContentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DrawPropertiesGUI(serializedObject, (UIContent)serializedObject.targetObject);
        }

        public static void DrawPropertiesGUI(SerializedObject serializedObject, UIContent content)
        {
#if USE_UNITY_LOCALIZATION
            if (content.hasNonLocalizedContent)
            {
                EditorGUILayout.HelpBox("Please use Localized strings. Regular non-localized strings are deprecated and unsafe to use when swapping languages.", MessageType.Warning);
            }
#endif

            serializedObject.Update();
            foreach (var propertyLocator in content.GetQuickEditUiElements())
            {
                DrawPropertyField(serializedObject, propertyLocator);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawPropertyField(SerializedObject parentSerializedObject, PropertyLocator propertyLocator)
        {
            if (propertyLocator.Condition == null) return;

            var isSameTarget = propertyLocator.Target == parentSerializedObject.targetObject;
            var serializedObject = isSameTarget ? parentSerializedObject : new SerializedObject(propertyLocator.Target);

            if (!isSameTarget)
                serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyLocator.PropertyPath), new GUIContent(propertyLocator.Label), true);
            if (EditorGUI.EndChangeCheck())
            {
                if (!isSameTarget)
                {
                    serializedObject.ApplyModifiedProperties();
                }
                EditorUtility.SetDirty(propertyLocator.Target);
            }
        }
    }
}
