using UnityEngine;
using UnityEditor;

public static class SolidTextureMaker
{
    [MenuItem("Tools/Create Solid Texture From Material")]
    static void Create()
    {
        var mat = Selection.activeObject as Material;
        if (mat == null)
        {
            Debug.LogWarning("Сначала выделите материал в Project.");
            return;
        }

        Color c = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : mat.color;

        var tex = new Texture2D(4, 4);
        var pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = c;
        tex.SetPixels(pixels);
        tex.Apply();

        string path = "Assets/Textures" + mat.name + "_Solid.png";
        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.Refresh();
        Debug.Log("Создано: " + path);
    }
}
