using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PlutoECL.Misc
{
    [InitializeOnLoad]
    public class CustomHierarchy : MonoBehaviour
    {
        static CustomHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }
        
        static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (!PlutoSettings.Instance || !PlutoSettings.Instance.UseCustomHierarchy)
                return;
            
            var gameObject = EditorUtility.InstanceIDToObject (instanceID) as GameObject;

            if (!gameObject)
                return;
            
            var parts = gameObject.GetComponents<Part>();
                    
            for (var i = 0; i < parts.Length; i++)
            {
                var type = parts[i].GetType();
                
                if (type.IsAssignableFrom(typeof(ExtendedBehaviour))) // we ignoring logics
                    continue;
               
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (Attribute.IsDefined(field, typeof(HideInInspector)))
                        continue;
                    
                    if (Attribute.IsDefined(field, typeof(RuntimeOnlyAttribute)) || Attribute.IsDefined(field, typeof(ReadOnlyAttribute)))
                        continue;

                    var value = field.GetValue(parts[i]);
                    
                    if (value == null || value.Equals(null))
                    {
                        EditorGUI.DrawRect(selectionRect, new Color(1f, 0.5f, 0f, 0.25f));
                        return;
                    }
                }
            }
        }
    }
}