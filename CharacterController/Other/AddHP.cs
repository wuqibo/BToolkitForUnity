using UnityEngine;

namespace BToolkit
{
    public class AddHP : MonoBehaviour
    {

        //B3DText和TextMesh两者选用其一
        public ImgText bText3D;
        public TextMesh[] textMeshs;
        Vector3 direction;
        float speed = 1;
        float alphaDelta = 1f;
        float alpha = 2;

        void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
            if (alpha > 0f)
            {
                alpha -= Time.deltaTime * alphaDelta;
                SetAlpha(alpha);
                if (alpha <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }

        void SetAlpha(float a)
        {
            if (bText3D)
            {
                Color color = bText3D.color;
                color.a = a;
                bText3D.color = color;
            }
            else if (textMeshs.Length > 0)
            {
                Color color = textMeshs[0].color;
                color.a = a;
                for (int i = 0; i < textMeshs.Length; i++)
                {
                    textMeshs[i].color = color;
                }
            }
        }

        /// <summary>
        /// 加入direction参数，用于应对AR应用中角色头顶方向旋转不定，方便自定义角色头顶方向
        /// </summary>
        internal static void Add(Vector3 pos, Vector3 direction, int value)
        {
            AddHP addHP = (Instantiate(Resources.Load("Effects/AddHP")) as GameObject).GetComponent<AddHP>();
            addHP.transform.localScale = Vector3.one;
            addHP.transform.position = pos;
            addHP.transform.rotation = Quaternion.LookRotation(addHP.transform.position - Camera.main.transform.position);
            Vector3 angle = addHP.transform.localEulerAngles;
            angle.y = 0;
            angle.z = 0;
            addHP.transform.localEulerAngles = angle;
            addHP.direction = direction;
            if (addHP.bText3D)
            {
                addHP.bText3D.text = "+" + value.ToString();
            }
            else if (addHP.textMeshs.Length > 0)
            {
                for (int i = 0; i < addHP.textMeshs.Length; i++)
                {
                    addHP.textMeshs[i].text = "+" + value.ToString();
                }
            }
        }
    }
}