#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [CustomPropertyDrawer(typeof(SerializedKeyValuePair<,>))]
    public class SerializedKeyValuePairDrawer : GgPropertyDrawer
    {
        #region variables

        private SerializedProperty key;
        private SerializedProperty value;
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region GgPropertyHeight

        protected override float GgPropertyHeight(SerializedProperty property, float propertyHeight, float approxFieldWidth)
        {
            key = property.FindPropertyRelative("key");
            value = property.FindPropertyRelative("value");
            
            // get individual heights
            float keyHeight = PropertyDrawerExtensions.TryFindCustomPropertyDrawer(key, out PropertyDrawer keyDrawer)
                ? keyDrawer.GetPropertyHeight(key, GUIContent.none)
                : EditorGUI.GetPropertyHeight(key, GUIContent.none, true);
            
            float valueHeight = PropertyDrawerExtensions.TryFindCustomPropertyDrawer(value, out PropertyDrawer valueDrawer)
                ? valueDrawer.GetPropertyHeight(value, GUIContent.none)
                : EditorGUI.GetPropertyHeight(value, GUIContent.none, true);
            
            return Mathf.Max(keyHeight, valueHeight);
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region OnGgGUI

        protected override void OnGgGUI(Rect position, SerializedProperty property, GUIContent label, GgGUIDefaults defaultCache)
        {
            // get reference to SerializeFields
            key = property.FindPropertyRelative("key");
            value = property.FindPropertyRelative("value");
            
            // get rects
            float keyHeight = PropertyDrawerExtensions.TryFindCustomPropertyDrawer(key, out PropertyDrawer keyDrawer)
                ? keyDrawer.GetPropertyHeight(key, GUIContent.none)
                : EditorGUI.GetPropertyHeight(key, GUIContent.none, true);
            float valueHeight = PropertyDrawerExtensions.TryFindCustomPropertyDrawer(value, out PropertyDrawer valueDrawer)
                ? valueDrawer.GetPropertyHeight(value, GUIContent.none)
                : EditorGUI.GetPropertyHeight(value, GUIContent.none, true);
            Rect keyRect = new Rect(position.x, position.y, labelWidth, keyHeight);
            float valueWidth = position.width - (labelWidth + (standardSpacing * 2));
            Rect valueRect = new Rect(position.xMax - valueWidth, position.y, valueWidth, valueHeight);
            
            // draw property
            GgGUI.VarField(keyRect, key, GUIContent.none);
            GgGUI.VarField(valueRect, value, GUIContent.none);
        }

        #endregion

    } // class end
}
#endif
