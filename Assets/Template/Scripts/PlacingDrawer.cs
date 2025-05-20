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
        string warningText = warningTextProperty.intValue + " Kiþilik Dizilim Engelleri";

        // Baþlýk belirle
        label.text = !string.IsNullOrEmpty(warningText) ? warningText  : "Unnamed Placing";

        // Varsayýlan çizim
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Elemanýn yüksekliði
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif