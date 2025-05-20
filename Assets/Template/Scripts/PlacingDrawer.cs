#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Placing))]
public class PlacingDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // WarningText'i al
        SerializedProperty warningTextProperty = property.FindPropertyRelative("AvailablePlacesForMode");
        string warningText = warningTextProperty.intValue + " Ki�ilik Dizilim Engelleri";

        // Ba�l�k belirle
        label.text = !string.IsNullOrEmpty(warningText) ? warningText  : "Unnamed Placing";

        // Varsay�lan �izim
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Eleman�n y�ksekli�i
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif