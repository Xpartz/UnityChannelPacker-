using UnityEngine;
using UnityEditor;

public class TexturePacker : EditorWindow
{
    private Texture2D textureR;
    private Texture2D textureG;
    private Texture2D textureB;
    private Texture2D textureA;
    private Texture2D mergedTexture;


    private Texture2D previewTextureR;
    private Texture2D previewTextureG;
    private Texture2D previewTextureB;
    private Texture2D previewTextureA;


    private bool hasRedChannel = false;
    private bool hasGreenChannel = false;
    private bool hasBlueChannel = false;
    private bool hasAlphaChannel = false;


    [MenuItem("Window/Texture Packer")]
    public static void ShowWindow()
    {
        var window = GetWindow<TexturePacker>("Texture Packer");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Source Textures", EditorStyles.boldLabel);
        textureR = (Texture2D)EditorGUILayout.ObjectField("Channel R:", textureR, typeof(Texture2D), false);
        textureG = (Texture2D)EditorGUILayout.ObjectField("Channel G:", textureG, typeof(Texture2D), false);
        textureB = (Texture2D)EditorGUILayout.ObjectField("Channel B:", textureB, typeof(Texture2D), false);
        textureA = (Texture2D)EditorGUILayout.ObjectField("Channel A:", textureA, typeof(Texture2D), false);

        if (GUILayout.Button("Merge"))
        {
            MergeTextures();
        }

        

        GUILayout.Label("RGB Texture", EditorStyles.boldLabel);
        var newMergedTexture = (Texture2D)EditorGUILayout.ObjectField("RGB Texture:", mergedTexture, typeof(Texture2D), false);

        if (mergedTexture != null)
        {
            if (GUILayout.Button("Split"))
            {
                SplitTexture(mergedTexture);
            }
        }
        if (newMergedTexture != mergedTexture)
        {
            mergedTexture = newMergedTexture;
            GeneratePreviewTextures();
        }

        if (mergedTexture != null)
        {
            GUILayout.Label("Preview channels", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();

            if (previewTextureR) GUILayout.Label(previewTextureR, GUILayout.Width(50), GUILayout.Height(50));
            if (previewTextureG) GUILayout.Label(previewTextureG, GUILayout.Width(50), GUILayout.Height(50));
            if (previewTextureB) GUILayout.Label(previewTextureB, GUILayout.Width(50), GUILayout.Height(50));
            if (previewTextureA) GUILayout.Label(previewTextureA, GUILayout.Width(50), GUILayout.Height(50));


            GUILayout.EndHorizontal();
        }
    }


    void MergeTextures()
    {
        SetTextureReadable(textureR, true);
        SetTextureReadable(textureG, true);
        SetTextureReadable(textureB, true);
        SetTextureReadable(textureA, true);
        int width = textureR ? textureR.width : textureG ? textureG.width : textureB ? textureB.width : 0;
        int height = textureR ? textureR.height : textureG ? textureG.height : textureB ? textureB.height : 0;
        TextureFormat format = textureA != null ? TextureFormat.RGBA32 : TextureFormat.RGB24;

        mergedTexture = new Texture2D(width, height, format, false);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float r = textureR ? textureR.GetPixel(x, y).r : 0;
                float g = textureG ? textureG.GetPixel(x, y).g : 0;
                float b = textureB ? textureB.GetPixel(x, y).b : 0;
                float a = textureA != null ? textureA.GetPixel(x, y).grayscale : 1.0f;

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
        mergedTexture = null;
        SetTextureReadable(textureR, false);
        SetTextureReadable(textureG, false);
        SetTextureReadable(textureB, false);
        SetTextureReadable(textureA, false);
    }




    void SplitTexture(Texture2D sourceTexture)
    {
        int width = sourceTexture.width;
        int height = sourceTexture.height;

        SetTextureReadable(mergedTexture, true);

        if (hasRedChannel)
        {
            Texture2D textureR = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = sourceTexture.GetPixel(x, y);
                    textureR.SetPixel(x, y, new Color(pixelColor.r, pixelColor.r, pixelColor.r));
                }
            }
            textureR.Apply();
            SaveTextureAsPNG(textureR, $"{sourceTexture.name}_R.png");
        }

        if (hasGreenChannel)
        {
            Texture2D textureG = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = sourceTexture.GetPixel(x, y);
                    textureG.SetPixel(x, y, new Color(pixelColor.g, pixelColor.g, pixelColor.g));
                }
            }
            textureG.Apply();
            SaveTextureAsPNG(textureG, $"{sourceTexture.name}_G.png");
        }

        if (hasBlueChannel)
        {
            Texture2D textureB = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = sourceTexture.GetPixel(x, y);
                    textureB.SetPixel(x, y, new Color(pixelColor.b, pixelColor.b, pixelColor.b));
                }
            }
            textureB.Apply();
            SaveTextureAsPNG(textureB, $"{sourceTexture.name}_B.png");
        }

        if (hasAlphaChannel)
        {
            Texture2D textureA = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = sourceTexture.GetPixel(x, y);
                    textureA.SetPixel(x, y, new Color(pixelColor.a, pixelColor.a, pixelColor.a));
                }
            }
            textureA.Apply();
            SaveTextureAsPNG(textureA, $"{sourceTexture.name}_A.png");
        }

        SetTextureReadable(mergedTexture, false);
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


    void GeneratePreviewTextures()
    {
        if (mergedTexture == null)
        {
            previewTextureR = null;
            previewTextureG = null;
            previewTextureB = null;
            previewTextureA = null;
            return;
        }
        SetTextureReadable(mergedTexture, true);
        int width = mergedTexture.width;
        int height = mergedTexture.height;

        previewTextureR = new Texture2D(width, height, TextureFormat.RGBA32, false);
        previewTextureG = new Texture2D(width, height, TextureFormat.RGBA32, false);
        previewTextureB = new Texture2D(width, height, TextureFormat.RGBA32, false);
        previewTextureA = new Texture2D(width, height, TextureFormat.RGBA32, false);

        hasRedChannel = false;
        hasGreenChannel = false;
        hasBlueChannel = false;
        hasAlphaChannel = false;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = mergedTexture.GetPixel(x, y);

                previewTextureR.SetPixel(x, y, new Color(pixelColor.r, 0, 0, pixelColor.r > 0 ? 1 : 1));
                if (pixelColor.r > 0) hasRedChannel = true;

                previewTextureG.SetPixel(x, y, new Color(0, pixelColor.g, 0, pixelColor.g > 0 ? 1 : 1));
                if (pixelColor.g > 0) hasGreenChannel = true;

                previewTextureB.SetPixel(x, y, new Color(0, 0, pixelColor.b, pixelColor.b > 0 ? 1 : 1));
                if (pixelColor.b > 0) hasBlueChannel = true;

                
                previewTextureA.SetPixel(x, y, new Color(1, 1, 1, pixelColor.a));
                if (pixelColor.a < 1) hasAlphaChannel = true;
                


            }
        }

        if (hasRedChannel) previewTextureR.Apply(); else previewTextureR = null;
        if (hasGreenChannel) previewTextureG.Apply(); else previewTextureG = null;
        if (hasBlueChannel) previewTextureB.Apply(); else previewTextureB = null;
        if (hasAlphaChannel) previewTextureA.Apply(); else previewTextureA = null;

        SetTextureReadable(mergedTexture, false);

    }

    private bool SetTextureReadable(Texture2D texture, bool readable)
    {
        if (texture == null) return false;

        string assetPath = AssetDatabase.GetAssetPath(texture);
        var importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);

        if (importer != null && importer.isReadable != readable)
        {
            importer.isReadable = readable;
            importer.SaveAndReimport();
            return true;
        }
        return false;
    }




}

