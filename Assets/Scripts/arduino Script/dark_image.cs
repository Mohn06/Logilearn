using UnityEngine;
using System.IO;

public class CreatePerfectSquareFade : MonoBehaviour
{
    [ContextMenu("Generate Perfect Square Fade")]
    void GenerateTexture()
    {
        int size = 512;
        float fadeWidth = 0.30f;

        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        Color baseColor = new Color(0.15f, 0.15f, 0.15f, 0.1f);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float nx = (float)x / (size - 1);
                float ny = (float)y / (size - 1);

                float fadeX = Mathf.Clamp01(nx / fadeWidth) * Mathf.Clamp01((1f - nx) / fadeWidth);
                float fadeY = Mathf.Clamp01(ny / fadeWidth) * Mathf.Clamp01((1f - ny) / fadeWidth);

                float alpha = fadeX * fadeY;

                texture.SetPixel(x, y, new Color(baseColor.r, baseColor.g, baseColor.b, alpha));
            }
        }

        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string path = Application.dataPath + "/PerfectSquareNoCorners.png";
        File.WriteAllBytes(path, bytes);

        Debug.Log("Square!");
    }
}