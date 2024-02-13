using UnityEditor;
using UnityEngine;
public class TextureImportSettings : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        Debug.Log("OnPreprocessTexture");
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.filterMode = FilterMode.Point;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        textureImporter.compressionQuality = 100; // Ranges from 0-100
    }
}
