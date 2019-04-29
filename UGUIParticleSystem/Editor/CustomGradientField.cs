using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

namespace BToolkit
{
    public class CustomGradientField
    {
        static MethodInfo s_miGradientField1;

        static CustomGradientField()
        {
            Type tyEditorGUILayout = typeof(EditorGUILayout);
            s_miGradientField1 = tyEditorGUILayout.GetMethod("GradientField",BindingFlags.NonPublic | BindingFlags.Static,null,new Type[] { typeof(string),typeof(Gradient),typeof(GUILayoutOption[]) },null);
        }

        public static Gradient GradientField(string label,Gradient gradient,params GUILayoutOption[] options)
        {
            try
            {
                if(gradient != null)
                {
                    gradient = (Gradient)s_miGradientField1.Invoke(null,new object[] { label,gradient,options });
                }
            }
            catch { }
            return gradient;
        }
    }
}