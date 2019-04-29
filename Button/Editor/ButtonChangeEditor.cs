using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace BToolkit
{
    [CustomEditor(typeof(ButtonChange))]
    public class ButtonChangeEditor : Editor
    {
        ButtonChange buttonChange;

        void OnEnable()
        {
            buttonChange = (ButtonChange)target;
        }

        public override void OnInspectorGUI()
        {
            buttonChange.target = (Image)EditorGUILayout.ObjectField("Target", buttonChange.target, typeof(Image), true);
            buttonChange.scale = EditorGUILayout.Toggle("Scale", buttonChange.scale);
            if (buttonChange.scale)
            {
                buttonChange.changeScale = EditorGUILayout.FloatField("    ChangeScale", buttonChange.changeScale);
            }
            buttonChange.texture = EditorGUILayout.Toggle("Texture", buttonChange.texture);
            if (buttonChange.texture)
            {
                buttonChange.changeSprite = (Sprite)EditorGUILayout.ObjectField("    SpriteChange", buttonChange.changeSprite, typeof(Sprite), true);
            }
            buttonChange.listenKeyBack = EditorGUILayout.Toggle("ListenKeyBack", buttonChange.listenKeyBack);
            buttonChange.sound = (AudioClip)EditorGUILayout.ObjectField("Sound", buttonChange.sound, typeof(AudioClip), true);
            buttonChange.canPlayGlobalSound = EditorGUILayout.Toggle("CanPlayGlobalSound", buttonChange.canPlayGlobalSound);
        }
    }
}