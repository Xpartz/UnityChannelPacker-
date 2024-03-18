using UnityEngine;
using UnityEditor;

public class TexturePacker : EditorWindow
{
    private Texture2D textureR;
    private Texture2D textureG;
    private Texture2D textureB;
    private Texture2D textureA;
    private Texture2D mergedTexture; // Для загрузки смердженной текстуры

    [MenuItem("Window/Texture Packer")]
    public static void ShowWindow()
    {
        var window = GetWindow<TexturePacker>("Texture Packer");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Source Textures", EditorStyles.boldLabel);
        textureR = (Texture2D)EditorGUILayout.ObjectField("Texture R:", textureR, typeof(Texture2D), false);
        textureG = (Texture2D)EditorGUILayout.ObjectField("Texture G:", textureG, typeof(Texture2D), false);
        textureB = (Texture2D)EditorGUILayout.ObjectField("Texture B:", textureB, typeof(Texture2D), false);
        textureA = (Texture2D)EditorGUILayout.ObjectField("Texture A:", textureA, typeof(Texture2D), false);

        if (GUILayout.Button("Merge"))
        {
            MergeTextures();
        }

        // Добавляем интерфейс для разделения текстуры
        GUILayout.Label("Merged Texture", EditorStyles.boldLabel);
        mergedTexture = (Texture2D)EditorGUILayout.ObjectField("Merged Texture:", mergedTexture, typeof(Texture2D), false);

        if (GUILayout.Button("Split"))
        {
            if (mergedTexture != null)
            {
                SplitTexture(mergedTexture);
            }
            else
            {
                Debug.LogWarning("No merged texture selected!");
            }
        }
    }

    void MergeTextures()
    {
        // Реализация слияния текстур...
    }

    void SplitTexture(Texture2D sourceTexture)
    {
        int width = sourceTexture.width;
        int height = sourceTexture.height;

        Texture2D textureR = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Texture2D textureG = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Texture2D textureB = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Texture2D textureA = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixelColor = sourceTexture.GetPixel(x, y);
                textureR.SetPixel(x, y, new Color(pixelColor.r, pixelColor.r, pixelColor.r));
                textureG.SetPixel(x, y, new Color(pixelColor.g, pixelColor.g, pixelColor.g));
                textureB.SetPixel(x, y, new Color(pixelColor.b, pixelColor.b, pixelColor.b));
                textureA.SetPixel(x, y, new Color(pixelColor.a, pixelColor.a, pixelColor.a));
            }
        }

        textureR.Apply();
        textureG.Apply();
        textureB.Apply();
        textureA.Apply();

        SaveTextureAsPNG(textureR, "R_Channel");
        SaveTextureAsPNG(textureG, "G_Channel");
        SaveTextureAsPNG(textureB, "B_Channel");
        SaveTextureAsPNG(textureA, "A_Channel");
    }

    void SaveTextureAsPNG(Texture2D texture, string name)
    {
        byte[] bytes = texture.EncodeToPNG();
        var path = EditorUtility.SaveFilePanelInProject("Save Texture", name, "png", "Please enter a file name to save the texture to");
        if (path.Length != 0)
        {
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            // Optionally set texture import settings after saving the file
        }
    }
}
