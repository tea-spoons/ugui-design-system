
namespace TeaSpoons.UGuiDesignSystem.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    [CustomEditor(typeof(UIController), editorForChildClasses: true)]
    public class UIControllerEditor : Editor
    {
        private SerializedObject contentSerializedObject;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();

            var controller = (UIController)target;
            var content = controller.FindContent();

            GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(3));
            GUILayout.Label("Content", EditorStyles.boldLabel);

            var preferredContentProperty = serializedObject.FindProperty(UIController.PropertyNames.PreferredContent);
            EditorGUILayout.PropertyField(preferredContentProperty);

            serializedObject.ApplyModifiedProperties();

            if (!preferredContentProperty.objectReferenceValue && controller.HasMultipleUIContents())
            {
                EditorGUILayout.HelpBox("This controller has multiple child UIContents.\n" +
                    "Please assign one of them in the \"Preferred Content\" field.",
                    MessageType.Error);
                return;
            }

            DrawContentSwitchButton(controller, content, preferredContentProperty);

            serializedObject.ApplyModifiedProperties();

            if (content)
            {
                if (contentSerializedObject == null ||
                    contentSerializedObject.targetObject != content)
                {
                    contentSerializedObject = new SerializedObject(content);
                }
                UIContentEditor.DrawPropertiesGUI(contentSerializedObject, content);
            }
        }

        private void OnDestroy()
        {
            contentSerializedObject?.Dispose();
        }

        private void DrawContentSwitchButton(UIController controller, UIContent content, SerializedProperty preferredContentProperty)
        {
            var prefab = content != null ?
                PrefabUtility.GetCorrespondingObjectFromSource(content) :
                null;

            if (!Application.isPlaying)
            {
                var label = GetSwitchButtonLabel(content, prefab);
                if (GUILayout.Button(label))
                {
                    OpenContentSwitchMenu(prefab, newPrefab => ReplaceUIContent(controller, content, newPrefab, preferredContentProperty));
                }
            }
            else
            {
                GUILayout.Box("Cannot switch content prefab during play mode.", GUILayout.ExpandWidth(true));
            }
        }

        private static string GetSwitchButtonLabel(UIContent content, UIContent prefab)
        {
            if (content == null)
            {
                return "None";
            }
            if (prefab == null)
            {
                return "Unknown Content Type";
            }

            return prefab.name;
        }

        private void OpenContentSwitchMenu(UIContent currentPrefab, Action<UIContent> onSelect)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("None"), currentPrefab == null, () => onSelect(null));
            menu.AddSeparator(string.Empty);

            var contentPrefabs = AssetDatabase.FindAssets("t:prefab")
                .Select(guid => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject)))
                .Cast<GameObject>()
                .Select(go => go.GetComponent<UIContent>())
                .Where(x => x != null);
            foreach (var prefab in contentPrefabs)
            {
                menu.AddItem(new GUIContent(prefab.name), prefab == currentPrefab, () => onSelect(prefab));
            }

            menu.ShowAsContext();
        }

        private void ReplaceUIContent(UIController controller, UIContent content, UIContent newPrefab, SerializedProperty preferredContentProperty)
        {
            if (content != null &&
                HasNonContentPrefabOverrides(content) &&
                !EditorUtility.DisplayDialog("Replace UIContent", "The UIContent has overrides beyond the UIContent properties.\nDo you really want to replace it?", "Yes", "No"))
            {
                return;
            }

            Undo.SetCurrentGroupName("Replace UI Content");
            var parent = controller.transform;
            var siblingIndex = parent.childCount;
            var wasPreferred = content && preferredContentProperty.objectReferenceValue == content;
            List<object> quickEditValues = null;
            if (content != null)
            {
                parent = content.transform.parent;
                siblingIndex = content.transform.GetSiblingIndex();
                quickEditValues = GetQuickEditValues(content);
                Undo.DestroyObjectImmediate(content.gameObject);
            }
            if (newPrefab != null)
            {
                var gameObject = (UIContent)PrefabUtility.InstantiatePrefab(newPrefab, parent);

                content = gameObject.GetComponent<UIContent>();
                content.transform.SetSiblingIndex(siblingIndex);
                SetQuickEditValues(content, quickEditValues);
                Undo.RegisterCreatedObjectUndo(content.gameObject, "Create new UI Content");

                if (wasPreferred)
                {
                    preferredContentProperty.objectReferenceValue = content;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        /// <summary>
        /// Returns whether <paramref name="content"/> has prefab overrides that are not part of the UIContent.
        /// </summary>
        private static bool HasNonContentPrefabOverrides(UIContent content)
        {
            if (PrefabUtility.GetAddedGameObjects(content.gameObject).Count > 0)
            {
                return true;
            }

            if (PrefabUtility.GetAddedComponents(content.gameObject).Count > 0)
            {
                return true;
            }

            return false;
        }

        private static List<object> GetQuickEditValues(UIContent content)
        {
            var result = new List<object>();
            foreach (var id in content.GetQuickEditUiElements())
            {
                result.Add(id.GetValue());
            }
            return result;
        }

        private static void SetQuickEditValues(UIContent content, List<object> values)
        {
            if (values == null) return;

            var index = 0;
            foreach (var id in content.GetQuickEditUiElements())
            {
                id.SetValue(values[index]);
                index++;
            }
        }
    }
}
