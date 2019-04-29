using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class MoveTextureUV : MonoBehaviour
    {
        public Image image;
        public UnityEngine.Renderer render;
        public string propertyName = "_MainTex";
        public Vector2 speed = new Vector2(0.5f, 0);
        private Vector2 offset;

        void Awake()
        {
            if (image)
            {
                offset = image.material.GetTextureOffset(propertyName);
            }
            else if (render)
            {
                offset = render.material.GetTextureOffset(propertyName);
            }
        }

        void Update()
        {
            offset += speed * Time.deltaTime;
            if (image)
            {
                image.material.SetTextureOffset(propertyName, offset);
            }
            else if (render)
            {
                render.material.SetTextureOffset(propertyName, offset);
            }
        }
    }
}