using System.Linq;
using UnityEngine;

namespace Jape
{
	public static class TextureExt
    {
        public static Texture2D Copy(this Texture2D baseTexture, bool apply = true)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(baseTexture.width, baseTexture.height, default, RenderTextureFormat.Default);
            Graphics.Blit(baseTexture, renderTexture);

            RenderTexture cacheTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;

            Texture2D texture = new(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

            RenderTexture.active = cacheTexture;
            RenderTexture.ReleaseTemporary(renderTexture);

            if (apply)
            {
                texture.Apply();
            }

            return texture;
        }

        public static Texture2D Colorize(this Texture2D baseTexture, Color color)
        {
            Texture2D texture = baseTexture.Copy(false);

            Color tempColor = color;
            Color[] renderPixels = texture.GetPixels().Select(pixel => pixel * tempColor).ToArray();

            texture.SetPixels(renderPixels);
            texture.Apply();

            return texture;
        }
    }
}