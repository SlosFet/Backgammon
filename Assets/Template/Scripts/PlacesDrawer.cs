#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Places))]
public class PlacesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Eleman�n indeksini path �zerinden bul
        int index = GetIndexFromPropertyPath(property.propertyPath);

        // Ba�l�k belirle
        label.text = $"{index + 1}. Yasakl� Dizilim";

        // Varsay�lan �izim
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Eleman�n y�ksekli�i
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    private int GetIndexFromPropertyPath(string propertyPath)
    {
        // Path'den indeks de�erini ��kar
        string[] splitPath = propertyPath.Split('[', ']');
        for (int i = splitPath.Length - 2; i >= 0; i--)
        {
            if (int.TryParse(splitPath[i], out int index))
            {
                return index;
            }
        }
        return 0; // Varsay�lan indeks
    }
}
#endif