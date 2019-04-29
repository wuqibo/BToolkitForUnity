using UnityEngine;

namespace BToolkit {
    [ExecuteInEditMode]
    public class HeadBar: MonoBehaviour {

        public enum Type {
            WorldSpace,
            UGUI
        }
        public Type type = Type.WorldSpace;
        float hpPercent, hpPercentFollow, mpPercent;
        public float HPPercent {
            set {
                if(hpPercent != value) {
                    hpPercent = value;
                    if(hpPercent > 1f) {
                        hpPercent = 1f;
                    }
                    if(hpPercent < 0f) {
                        hpPercent = 0f;
                    }
                    SetMeshRendererSlider(hpBar,hpPercent,"HP Bar");
                    if(hpShadow) {
                        hpFollowSpeed = Mathf.Abs(hpPercent - hpPercentFollow) * 2f;
                    }
                }
            }
            get {
                return hpPercent;
            }
        }
        public float MPPercent {
            set {
                if(mpPercent != value) {
                    mpPercent = value;
                    if(mpPercent > 1f) {
                        mpPercent = 1f;
                    }
                    if(mpPercent < 0f) {
                        mpPercent = 0f;
                    }
                    SetMeshRendererSlider(mpBar,mpPercent,"MP Bar");
                }
            }
            get {
                return mpPercent;
            }
        }
        float hpFollowSpeed;
        Transform camTrans;
        public Actor actor;
        //WorldSpace
        public Transform bg;
        public MeshRenderer hpBar, hpShadow, mpBar;
        public TextMesh nameText, levelText;
        //UGUI

        void Awake() {
            camTrans = Camera.main.transform;
            SetChildrenPos();
        }

        void Update() {
            if(type == Type.WorldSpace) {
                if(hpShadow) {
                    if(HPPercent > hpPercentFollow) {
                        hpPercentFollow = HPPercent;
                    } else {
                        hpPercentFollow -= hpFollowSpeed * Time.deltaTime;
                    }
                    SetMeshRendererSlider(hpShadow,hpPercentFollow,"HP Shadow");
                }
            } else if(type == Type.UGUI) {

            }
            if(actor) {
                transform.localPosition = new Vector3(0f,actor.headBarHeight,0f);
                transform.rotation = Quaternion.LookRotation(transform.position - camTrans.position);
                Vector3 angle = transform.eulerAngles;
                angle.y = 0;
                angle.z = 0;
                transform.eulerAngles = angle;
                if(actor.actorProperties.HPMax > 0) {
                    HPPercent = actor.actorProperties.HP / (float)actor.actorProperties.HPMax;
                }
                if(actor.actorProperties.MPMax > 0) {
                    MPPercent = actor.actorProperties.MP / (float)actor.actorProperties.MPMax;
                }
            }
#if UNITY_EDITOR
            SetChildrenPos();
#endif
        }

        public void SetActor(Actor actor) {
            this.actor = actor;
            base.transform.SetParent(actor.transform,false);
        }

        void SetMeshRendererSlider(MeshRenderer meshRenderer,float value,string barName) {
            if(meshRenderer) {
                bool remind = false;
                if(Application.isPlaying) {
                    if(meshRenderer.material.HasProperty("_Value")) {
                        meshRenderer.material.SetFloat("_Value",value);
                    } else {
                        remind = true;
                    }
                } else {
                    if(meshRenderer.sharedMaterial.HasProperty("_Value")) {
                        meshRenderer.sharedMaterial.SetFloat("_Value",value);
                    } else {
                        remind = true;
                    }
                }
                if(remind) {
                    Debug.LogError(barName + "必须使用Shader: Tool/Slider_ReceiveLight 或 Slider_SelfLight");
                }
            }
        }

        void SetChildrenPos() {
            if(type == Type.WorldSpace) {
                if(bg) {
                    bg.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 3000;
                    if(hpBar) {
                        Vector3 pos = hpBar.transform.localPosition;
                        pos.z = bg.localPosition.z - 0.002f;
                        hpBar.transform.localPosition = pos;
                    }
                    if(hpShadow) {
                        Vector3 pos = hpShadow.transform.localPosition;
                        pos.z = bg.localPosition.z - 0.001f;
                        hpShadow.transform.localPosition = pos;
                    }
                    if(mpBar) {
                        Vector3 pos = mpBar.transform.localPosition;
                        pos.z = bg.localPosition.z - 0.001f;
                        mpBar.transform.localPosition = pos;
                    }
                    if(nameText) {
                        Vector3 pos = nameText.transform.localPosition;
                        pos.z = bg.localPosition.z - 0.001f;
                        nameText.transform.localPosition = pos;
                        nameText.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 3001;
                    }
                    if(levelText) {
                        Vector3 pos = levelText.transform.localPosition;
                        pos.z = bg.localPosition.z - 0.001f;
                        levelText.transform.localPosition = pos;
                        levelText.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 3001;
                    }
                }
            } else if(type == Type.UGUI) {

            }
        }
    }
}