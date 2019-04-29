using UnityEngine;

namespace BToolkit
{
    public class DropHP : MonoBehaviour
    {

        //B3DText和TextMesh两者选用其一
        public ImgText bText3D;
        public TextMesh[] textMeshs;
        Vector3 direction, directionBack;
        float speed = 5;
        float alpha = 2;
        bool isCrit;
        float scaleSpeed = 20;

        void Start()
        {
            directionBack = direction * -1;
            float offset = 0.3f;
            direction += new Vector3(Random.Range(-offset, offset), Random.Range(-offset, offset), Random.Range(-offset, offset));
            Destroy(gameObject, 1f);
        }

        void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
            direction += directionBack * 2f * Time.deltaTime;
            if (alpha > 0f)
            {
                alpha -= Time.deltaTime * 2f;
                SetAlpha(alpha);
            }
            if (isCrit)
            {
                if (transform.localScale.x < 3f)
                {
                    transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
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
        internal static void Drop(Vector3 pos, Vector3 direction, int value, bool isCrit)
        {
            DropHP dropHP = (Instantiate(Resources.Load("Effects/DropHP")) as GameObject).GetComponent<DropHP>();
            dropHP.transform.localScale = Vector3.one;
            dropHP.transform.position = pos;
            dropHP.transform.rotation = Quaternion.LookRotation(dropHP.transform.position - Camera.main.transform.position);
            dropHP.direction = direction;
            Vector3 angle = dropHP.transform.localEulerAngles;
            angle.y = 0;
            angle.z = 0;
            dropHP.transform.localEulerAngles = angle;
            if (dropHP.bText3D)
            {
                dropHP.bText3D.text = "-" + value.ToString();
            }
            else if (dropHP.textMeshs.Length > 0)
            {
                for (int i = 0; i < dropHP.textMeshs.Length; i++)
                {
                    dropHP.textMeshs[i].text = "-" + value.ToString();
                }
            }
            dropHP.isCrit = isCrit;
        }
    }
}