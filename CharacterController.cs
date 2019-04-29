using UnityEngine;
using System.Collections;

namespace BToolkit
{
    public class CharacterController : MonoBehaviour
    {

        [System.Serializable]
        public class CharacterAction
        {
            public string 名称;
            public AnimationClip 动画;
            public KeyCode 触发;
            internal CharacterController character;
            bool hasPlay;
            public void Update()
            {
                if (动画)
                {
                    if (!hasPlay)
                    {
                        character.PlayAnim(character.animator, 动画.name, 0);
                        hasPlay = true;
                    }
                    else
                    {
                        AnimatorStateInfo mAnimatorStateInfo = character.animator.GetCurrentAnimatorStateInfo(0);
                        if (mAnimatorStateInfo.IsName(动画.name))
                        {
                            if (mAnimatorStateInfo.normalizedTime > 0.95f)
                            {
                                character.PlayAnim(character.animator, character.闲置动画.name, 0);
                                character.currAction = null;
                                hasPlay = false;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("动作[" + 名称 + "]没有动画");
                    character.currAction = null;
                }
            }
        }
        public enum KeyCode
        {
            无, 鼠标, Q, E, R, T, Y, U, I, O, P, F, G, H, J, K, L, Z, X, C, V, B, N, M, Num0, Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9
        }
        public Transform 摄影机;
        public float 摄影机距离 = 5, 摄影机高度 = 3;
        [Space]
        public AnimationClip 闲置动画;
        [Space]
        public AnimationClip 移动动画;
        public float 移动速度 = 6;
        public float 转向速度 = 100;
        [Space]
        public CharacterAction[] 动作;
        CharacterAction currAction;
        Animator animator;
        int count;

        void Awake()
        {
            if (!animator)
            {
                animator = GetComponentInChildren<Animator>();
            }
            count = 动作.Length;
            for (int i = 0; i < count; i++)
            {
                动作[i].character = this;
            }
        }

        void Update()
        {
            //监听鼠标点击
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < count; i++)
                {
                    CharacterAction action = 动作[i];
                    if (action.触发 == KeyCode.鼠标)
                    {
                        if (currAction == null)
                        {
                            currAction = action;
                        }
                    }
                }
            }
            if (currAction != null)
            {
                currAction.Update();
                return;
            }
            //移动
            bool isMoving = false;
            if (Input.GetKey(UnityEngine.KeyCode.W))
            {
                isMoving = true;
                transform.Translate(Vector3.forward * 移动速度 * Time.deltaTime);
            }
            else if (Input.GetKey(UnityEngine.KeyCode.S))
            {
                isMoving = true;
                transform.Translate(Vector3.back * 移动速度 * Time.deltaTime);
            }
            if (Input.GetKey(UnityEngine.KeyCode.S))
            {
                if (Input.GetKey(UnityEngine.KeyCode.A))
                {
                    isMoving = true;
                    transform.Rotate(Vector3.up * 转向速度 * Time.deltaTime);
                }
                else if (Input.GetKey(UnityEngine.KeyCode.D))
                {
                    isMoving = true;
                    transform.Rotate(Vector3.down * 转向速度 * Time.deltaTime);
                }
            }
            else
            {
                if (Input.GetKey(UnityEngine.KeyCode.A))
                {
                    isMoving = true;
                    transform.Rotate(Vector3.down * 转向速度 * Time.deltaTime);
                }
                else if (Input.GetKey(UnityEngine.KeyCode.D))
                {
                    isMoving = true;
                    transform.Rotate(Vector3.up * 转向速度 * Time.deltaTime);
                }
            }
            if (isMoving)
            {
                if (移动动画)
                {
                    PlayAnim(animator, 移动动画.name, 0);
                }
            }
            else
            {
                if (闲置动画)
                {
                    PlayAnim(animator, 闲置动画.name, 0);
                }
            }
        }

        void LateUpdate()
        {
            if (摄影机)
            {
                Vector3 cameraTargetPos = transform.position;
                cameraTargetPos -= transform.forward * 摄影机距离;
                cameraTargetPos.y = transform.position.y + 摄影机高度;
                摄影机.position += (cameraTargetPos - 摄影机.position) * 2f * Time.deltaTime;
                Quaternion rotationToTarget = Quaternion.LookRotation(transform.position - 摄影机.position);
                摄影机.rotation = Quaternion.Lerp(摄影机.rotation, rotationToTarget, 40f * Time.deltaTime);
            }
        }

        void PlayAnim(Animator animator, string clipName, float transitionDuration)
        {
            if (animator)
            {
                if (transitionDuration <= 0)
                {
                    animator.Play(clipName, 0, 0f);
                }
                else
                {
                    if (animator.GetNextAnimatorStateInfo(0).fullPathHash == 0)
                    {
                        animator.CrossFade(clipName, transitionDuration);
                    }
                    else
                    {
                        animator.Play(clipName, 0, 0f);
                    }
                }
            }
        }

        UnityEngine.KeyCode GetKeyCode(KeyCode code)
        {
            switch (code)
            {
                case KeyCode.B:
                    return UnityEngine.KeyCode.B;
                case KeyCode.C:
                    return UnityEngine.KeyCode.C;
                case KeyCode.E:
                    return UnityEngine.KeyCode.E;
                case KeyCode.F:
                    return UnityEngine.KeyCode.F;
                case KeyCode.G:
                    return UnityEngine.KeyCode.G;
                case KeyCode.H:
                    return UnityEngine.KeyCode.H;
                case KeyCode.I:
                    return UnityEngine.KeyCode.I;
                case KeyCode.J:
                    return UnityEngine.KeyCode.J;
                case KeyCode.K:
                    return UnityEngine.KeyCode.K;
                case KeyCode.L:
                    return UnityEngine.KeyCode.L;
                case KeyCode.M:
                    return UnityEngine.KeyCode.M;
                case KeyCode.N:
                    return UnityEngine.KeyCode.N;
                case KeyCode.O:
                    return UnityEngine.KeyCode.O;
                case KeyCode.P:
                    return UnityEngine.KeyCode.P;
                case KeyCode.Q:
                    return UnityEngine.KeyCode.Q;
                case KeyCode.R:
                    return UnityEngine.KeyCode.R;
                case KeyCode.T:
                    return UnityEngine.KeyCode.T;
                case KeyCode.U:
                    return UnityEngine.KeyCode.U;
                case KeyCode.V:
                    return UnityEngine.KeyCode.V;
                case KeyCode.X:
                    return UnityEngine.KeyCode.X;
                case KeyCode.Y:
                    return UnityEngine.KeyCode.Y;
                case KeyCode.Z:
                    return UnityEngine.KeyCode.Z;
                case KeyCode.Num0:
                    return UnityEngine.KeyCode.Keypad0;
                case KeyCode.Num1:
                    return UnityEngine.KeyCode.Keypad1;
                case KeyCode.Num2:
                    return UnityEngine.KeyCode.Keypad2;
                case KeyCode.Num3:
                    return UnityEngine.KeyCode.Keypad3;
                case KeyCode.Num4:
                    return UnityEngine.KeyCode.Keypad4;
                case KeyCode.Num5:
                    return UnityEngine.KeyCode.Keypad5;
                case KeyCode.Num6:
                    return UnityEngine.KeyCode.Keypad6;
                case KeyCode.Num7:
                    return UnityEngine.KeyCode.Keypad7;
                case KeyCode.Num8:
                    return UnityEngine.KeyCode.Keypad8;
                case KeyCode.Num9:
                    return UnityEngine.KeyCode.Keypad9;
                default:
                    return UnityEngine.KeyCode.None;
            }
        }

    }
}