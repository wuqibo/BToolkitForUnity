using UnityEngine;
using UnityEngine.UI;

namespace Lobby.UI.Effect
{
    public class FlowLight : MonoBehaviour
    {
        float timer;
        Material material;
        Vector2 offset;

        void Awake()
        {
            material = GetComponent<Image>().material;
            material.shader = Shader.Find("BToolkit/FlowLight");
            Go();
        }

        void Go()
        {
            offset.x = 1;
            this.enabled = true;
            timer = Random.Range(3f, 8f);
            Invoke("Go", timer);
        }

        void Update()
        {
            material.SetTextureOffset("_MainTex", offset);
            offset.x -= 1f * Time.deltaTime;
            if (offset.x < -1f)
            {
                this.enabled = false;
            }
        }

    }
}