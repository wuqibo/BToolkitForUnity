using UnityEngine;

namespace BToolkit
{
    public class CloudResARMaterials : MonoBehaviour
    {
        public Material arShadowMaterial;

        public static CloudResARMaterials instance;

        void Awake()
        {
            instance = this;
        }
    }
}