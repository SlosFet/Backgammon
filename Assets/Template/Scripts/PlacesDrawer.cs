#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Places))]
public class PlacesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Elemanın indeksini path üzerinden bul
        int index = GetIndexFromPropertyPath(property.propertyPath);

        // Başlık belirle
        label.text = $"{index + 1}. Yasaklı Dizilim";

        // Varsayılan çizim
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Elemanın yüksekliği
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    private int GetIndexFromPropertyPath(string propertyPath)
    {
        // Path'den indeks değerini çıkar
        string[] splitPath = propertyPath.Split('[', ']');
        for (int i = splitPath.Length - 2; i >= 0; i--)
        {
            if (int.TryParse(splitPath[i], out int index))
            {
                return index;
            }
        }
        return 0; // Varsayılan indeks
    }
}
#endif