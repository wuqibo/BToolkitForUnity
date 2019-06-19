using System;
using System.IO;
using System.Windows.Forms;
using UnityEngine;

namespace BToolkit
{
    public class TextureHelper
    {
        /// <summary>
        /// 裁切多余部分变成正方形
        /// </summary>
        public static Texture2D CutTextureToSquare(Texture2D source)
        {
            int sourceW = source.width;
            int sourceH = source.height;
            if (sourceW > sourceH)
            {
                return CutTextureToSize(source, sourceH, sourceH);
            }
            else
            {
                return CutTextureToSize(source, sourceW, sourceW);
            }
        }

        /// <summary>
        /// 裁切图片成特定尺寸
        /// </summary>
        public static Texture2D CutTextureToSize(Texture2D source, int width, int height)
        {
            float sourceW = source.width;
            float sourceH = source.height;
            if (sourceW == width && sourceH == height)
            {
                return source;
            }
            Texture2D result = null;
            if (sourceW / sourceH > width / (float)height)
            {
                if (sourceH < height)
                {
                    float ratio = width / (float)height;
                    height = (int)sourceH;
                    width = (int)(height * ratio);
                }
                result = new Texture2D(width, height, source.format, false);
                float sourceWScale = height * sourceW / sourceH;
                float offset = (sourceWScale - width) * 0.5f;
                for (int i = 0; i < height; ++i)
                {
                    for (int j = 0; j < width; ++j)
                    {
                        Color newColor = source.GetPixelBilinear((j + offset) / sourceWScale, i / (float)height);
                        result.SetPixel(j, i, newColor);
                    }
                }
            }
            else
            {
                if (sourceW < width)
                {
                    float ratio = height / (float)width;
                    width = (int)sourceW;
                    height = (int)(width * ratio);
                }
                result = new Texture2D(width, height, source.format, false);
                float sourceHScale = width * sourceH / sourceW;
                float offset = (sourceHScale - height) * 0.5f;
                for (int i = 0; i < height; ++i)
                {
                    for (int j = 0; j < width; ++j)
                    {
                        Color newColor = source.GetPixelBilinear(j / (float)width, (i + offset) / sourceHScale);
                        result.SetPixel(j, i, newColor);
                    }
                }
            }
            result.Apply();
            return result;
        }

        /// <summary>
        /// 压扁图片成特定尺寸
        /// </summary>
        public static Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }
            result.Apply();
            return result;
        }

        /// <summary>
        /// 添加水印
        /// </summary>
        public static Texture2D AddWaterMarking(Texture2D texture, Texture2D marking)
        {
            int markW = (int)(texture.width * 0.5f);
            int markH = (int)(markW * marking.height / (float)marking.width);
            int offset = (int)(markH * 0.5f);
            int left = texture.width - markW - offset;
            int markXIndex = 0;
            int markYIndex = 0;
            for (int i = offset; i < offset + markH; i++)
            {
                markXIndex = 0;
                for (int j = left; j < left + markW; j++)
                {
                    Color bgColor = texture.GetPixel(j, i);
                    Color markColor = marking.GetPixelBilinear(markXIndex/(float)markW, markYIndex / (float)markH);
                    if (markColor.a > 0)
                    {
                        Color blendColor = Color.Lerp(bgColor, markColor, 0.5f);
                        texture.SetPixel(j, i, blendColor);
                    }
                    markXIndex++;
                }
                markYIndex++;
            }
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// OpenFileDialog To Base64（需要System.Windows.Forms）
        /// </summary>
        public static string GetBase64FromOpenFile(OpenFileDialog openFileDialog)
        {
            Stream ms = openFileDialog.OpenFile();
            try
            {
                byte[] bytes = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(bytes, 0, Convert.ToInt32(ms.Length));
                return Convert.ToBase64String(bytes);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Base64 to Texture2D
        /// </summary>
        public static Texture2D BytesToTexture(byte[] bytes, int width, int height)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            try
            {
                tex.LoadImage(bytes);
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(ex.Message);
            }
            return tex;
        }

        /// <summary>
        /// Base64 to Texture2D
        /// </summary>
        public static Texture2D Base64ToTexture(string base64, int width, int height)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            try
            {
                byte[] bytes = System.Convert.FromBase64String(base64);
                tex.LoadImage(bytes);
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(ex.Message);
            }
            return tex;
        }

        /// <summary>
        /// Texture2D to Base64
        /// </summary>
        public static string TextureToBase64(Texture2D texture2d)
        {
            if (texture2d != null)
            {
                return System.Convert.ToBase64String(TextureToBytes(texture2d));
            }
            return null;
        }

        /// <summary>
        /// Texture转byte[]
        /// </summary>
        public static byte[] TextureToBytes(Texture texture)
        {
            return ((Texture2D)texture).EncodeToJPG();
        }

        /// <summary>
        /// Base64 to Byets
        /// </summary>
        public static byte[] Base64ToBytes(string base64)
        {
            if (!string.IsNullOrEmpty(base64))
            {
                return System.Convert.FromBase64String(base64);
            }
            return null;
        }

        /// <summary>
        /// Bytes to Base64
        /// </summary>
        public static string BytesToBase64(byte[] textureBytes)
        {
            if (textureBytes != null)
            {
                return System.Convert.ToBase64String(textureBytes);
            }
            return null;
        }

        /// <summary>
        /// Texture to Sprite
        /// </summary>
        public static Sprite TextureToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Change HashCode
        /// </summary>
        public static Texture2D ChangeMD5(Texture2D texture)
        {
            if (texture)
            {
                int texW = texture.width;
                int texH = texture.height;
                int offsetX = UnityEngine.Random.Range(0, texW);
                int offsetY = UnityEngine.Random.Range(0, texW);
                Color oldColor = texture.GetPixelBilinear(offsetX / (float)texW, offsetY / (float)texH);
                Color newColor = new Color(oldColor.r * 0.9f, oldColor.g * 0.9f, oldColor.b * 0.9f, oldColor.a * 0.9f);//改变一点点颜色
                texture.SetPixel(offsetX, offsetY, newColor);
                texture.Apply();
                return texture;
            }
            return null;
        }

    }
}