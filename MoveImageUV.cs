using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BToolkit
{
    public class MoveImageUV : MonoBehaviour
    {

        public enum Direction
        {
            Vertical, Horizontal
        }
        public Direction direction;
        public float speed = 1f;
        public string textureName;
        private Material mat;
        private Vector2 offset;

        void Awake()
        {
            mat = GetComponent<Image>().material;
        }

        void Update()
        {
            if (direction == Direction.Vertical)
            {
                offset.y += speed * Time.deltaTime;
                if (offset.y >= 1)
                {
                    offset.y = 0;
                }
            }
            else
            {
                offset.x += speed * Time.deltaTime;
                if (offset.x >= 1)
                {
                    offset.x = 0;
                }
            }
            mat.SetTextureOffset(textureName, offset);
        }
    }
}