using UnityEngine;
using UnityEngine.UI;

namespace BToolkit
{
    public class UGUIAdditive:MonoBehaviour
    {
        void Start()
        {
            if(SpecialMaterials.instance)
            {
                Image image = GetComponent<Image>();
                if(image)
                {
                    image.material = SpecialMaterials.instance.uguiAdditive;
                }
            }
        }
    }
}