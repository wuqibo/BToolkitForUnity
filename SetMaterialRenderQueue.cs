using UnityEngine;

namespace BToolkit
{
    [ExecuteInEditMode]
    public class SetMaterialRenderQueue : MonoBehaviour
    {
        public Material material;
        public int queue = 3001;

        void Start()
        {
            material.renderQueue = queue;
        }

#if UNITY_EDITOR
        void Update()
        {
            material.renderQueue = queue;
        }
#endif
    }
}