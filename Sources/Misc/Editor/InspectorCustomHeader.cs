using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PlutoECL.Misc
{
    [InitializeOnLoad]
    public static class InspectorCustomHeader
    {
        static InspectorCustomHeader()
        {
            Editor.finishedDefaultHeaderGUI += DrawExtension;
        }

        static void DrawExtension(Editor editor)
        {
            if (!PlutoSettings.Instance || !PlutoSettings.Instance.UseCustomInspectorHeader)
                return;
            
            if (editor.target is GameObject gameObject)
            {
                var entity = gameObject.GetComponent<Entity>();

                if (entity)
                {
                    DrawEmptyFieldWarning(gameObject);
                    return;
                }

                if (GUILayout.Button("Make it Entity"))
                {
                    gameObject.AddComponent<Entity>();
                    EditorUtility.SetDirty(editor.target);
                }
            }
        }

        static void DrawEmptyFieldWarning(GameObject gameObject)
        {
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
                        var style = new GUIStyle();
                        style.normal.textColor = new Color(1f, 0.75f, 0f);
                        style.fontStyle = FontStyle.Bold;
                        
                        GUILayout.Label("Field " + field.Name + " of " + type.Name + " is Empty.", style);
                    }
                }
            }
        }
    }
}