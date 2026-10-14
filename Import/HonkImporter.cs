using System;
using System.IO;
using UnityEngine;

namespace HonkAPI.Import
{
    public static class HonkImporter
    {
        public static Texture2D LoadTexture(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            if (ImageConversion.LoadImage(tex, bytes))
            {
                return tex;
            }
            return null;
        }

        public static Material CreateMaterial(Texture2D texture)
        {
            Material mat = new Material(Shader.Find("Standard"));
            if (texture != null)
            {
                mat.mainTexture = texture;
            }
            return mat;
        }

        public static Material CreateColoredMaterial(Color color)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            return mat;
        }
    }
}
