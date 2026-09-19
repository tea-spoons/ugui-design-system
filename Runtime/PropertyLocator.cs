
namespace TeaSpoons.UGuiDesignSystem
{
    using System;
    using UnityObject = UnityEngine.Object;

    /// <summary>
    /// Represents a 
    /// </summary>
    internal readonly struct PropertyLocator
    {
        public readonly string Label;
        public readonly UnityObject Target;
        public readonly string PropertyPath;
        public readonly UnityObject Condition;

        public PropertyLocator(string label, UnityObject target, string propertyPath)
            : this(label, target, propertyPath, target)
        {
        }

        public PropertyLocator(string label, UnityObject target, string propertyPath, UnityObject condition)
        {
            Label = label;
            Target = target;
            PropertyPath = propertyPath;
            Condition = condition;
        }

#if UNITY_EDITOR
        public PropertyLocator(UnityEditor.PropertyModification modification)
        {
            Label = modification.propertyPath;
            Target = modification.target;
            PropertyPath = modification.propertyPath;
            Condition = modification.target;
        }

        public object GetValue()
        {
            if (Target == null) return null;

            var serializedObject = new UnityEditor.SerializedObject(Target);
            serializedObject.Update();
            var property = serializedObject.FindProperty(PropertyPath);
            return property.boxedValue;
        }

        public void SetValue(object value)
        {
            if (Target == null) return;

            var serializedObject = new UnityEditor.SerializedObject(Target);
            var property = serializedObject.FindProperty(PropertyPath);
            property.boxedValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif

        public override bool Equals(object obj)
        {
            return obj is PropertyLocator other &&
                GetSourceObject(Target) == GetSourceObject(other.Target) &&
                PropertyPath == other.PropertyPath;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetSourceObject(Target), PropertyPath);
        }

        private static UnityObject GetSourceObject(UnityObject obj)
        {
#if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(obj)) return obj;

            return UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(obj);
#else
            return obj;
#endif
        }
    }
}
