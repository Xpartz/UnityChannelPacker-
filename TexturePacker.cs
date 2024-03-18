using UnityEngine;
using UnityEditor;

public class TexturePacker : EditorWindow
{
    private Texture2D textureR;
    private Texture2D textureG;
    private Texture2D textureB;
    private Texture2D textureA;
    private Texture2D mergedTexture;

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
        int width = textureR ? textureR.width : textureG ? textureG.width : textureB ? textureB.width : textureA.width;
        int height = textureR ? textureR.height : textureG ? textureG.height : textureB ? textureB.height : textureA.height;

        Texture2D mergedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float r = textureR ? textureR.GetPixel(x, y).r : 0;
                float g = textureG ? textureG.GetPixel(x, y).g : 0;
                float b = textureB ? textureB.GetPixel(x, y).b : 0;
                float a = textureA ? textureA.GetPixel(x, y).a : 0;

                mergedTexture.SetPixel(x, y, new Color(r, g, b, a));
            }
        }

        mergedTexture.Apply();

        byte[] bytes = mergedTexture.EncodeToPNG();
        string path = EditorUtility.SaveFilePanelInProject("Save Merged Texture", "MergedTexture", "png", "Please enter a file name to save the texture to");
        if (path.Length != 0)
        {
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.isReadable = true;
                importer.SaveAndReimport();
            }
        }
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
           
        }
    }
}
