using UnityEngine;
using System.Collections;

namespace BToolkit
{
    public class OutlineController : MonoBehaviour
    {

        Material[] materials;
        float outlineAlpha;
        Color outlineColor;

        void Awake()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            materials = new Material[renderers.Length];
            for (int i = 0; i < materials.Length; i++)
            {
#if UNITY_4_6
#else
                renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
#endif
                renderers[i].receiveShadows = false;
                materials[i] = renderers[i].material;
            }
            SetOutlineAlpha(0f);
        }

        void Update()
        {
            if (outlineAlpha > 0f)
            {
                outlineAlpha -= Time.deltaTime;
                if (outlineAlpha < 0f)
                {
                    outlineAlpha = 0f;
                }
                SetOutlineAlpha(outlineAlpha);
            }
        }

        public void ShowOutline(Color color)
        {
            outlineAlpha = 1f;
            outlineColor = color;
        }

        void SetOutlineAlpha(float alpha)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                outlineColor.a = alpha;
                if (material.HasProperty("_OutlineColor"))
                {
                    material.SetColor("_OutlineColor", outlineColor);
                }
            }
        }
    }
}