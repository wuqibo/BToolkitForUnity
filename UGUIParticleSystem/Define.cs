using System;
using UnityEngine;

namespace BToolkit.UGUIParticle
{
    public enum Space
    {
        Local,
        World
    }
    public enum ValueType
    {
        Single,
        Between
    }

    public enum ShapeType
    {
        Circle,
        Square
    }

    public struct Tiles
    {
        public int x, y;
    }

    [Serializable]
    public class FloatValue
    {
        public ValueType type;
        public float single, min, max;
        public float curr { get { return (type == ValueType.Single) ? single : UnityEngine.Random.Range(min,max); } }
    }

    [Serializable]
    public class ColorValue
    {
        public ValueType type;
        public Color single, min, max;
        public Color curr { get { return (type == ValueType.Single) ? single : GetBetweenColor(min,max); } }
        Color GetBetweenColor(Color colorA,Color colorB)
        {
            float ran = UnityEngine.Random.Range(0f,1f);
            float r = colorA.r + (colorB.r - colorA.r) * ran;
            float g = colorA.g + (colorB.g - colorA.g) * ran;
            float b = colorA.b + (colorB.b - colorA.b) * ran;
            float a = colorA.a + (colorB.a - colorA.a) * ran;
            return new Color(r,g,b,a);
        }
    }

    [Serializable]
    public class Function
    {
        public bool isOpen, isUse;
    }

    [Serializable]
    public class Emission:Function
    {
        public float rate = 10;
        public float timer;
    }

    [Serializable]
    public class Shape:Function
    {
        public ShapeType type;
        public float radius;
        public int arc = 360;
        public Vector2 size = new Vector2(600,200);
    }

    [Serializable]
    public class ColorOverLifetime:Function
    {
        public Gradient gradient;
    }

    [Serializable]
    public class Wind:Function
    {
        public ValueType type;
        public Vector2 single = new Vector2(-1,0), min = new Vector2(-1,0), max = new Vector2(-2,0);
        public Vector2 curr { get { return (type == ValueType.Single) ? single : new Vector2(UnityEngine.Random.Range(min.x,max.x),UnityEngine.Random.Range(min.y,max.y)); } }
    }

    [Serializable]
    public class ScaleOverLifetime:Function
    {
        public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
    }

    [Serializable]
    public class RotationOverLifetime:Function
    {
        public ValueType type;
        public float single = 45, min, max = 45;
        public float curr { get { return (type == ValueType.Single) ? single : UnityEngine.Random.Range(min,max); } }
    }

    [Serializable]
    public class Renderer
    {
        public bool isOpen;
        public bool textureSheetAnimation;
        public Sprite sprite;
        public Sprite[] sprites = new Sprite[0];
        public int frameCount;
        public bool openArr;
        public int fps = 12;
        public Material material;
    }
}