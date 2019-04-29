using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BToolkit.P2PNetwork {
    public class Ping: MonoBehaviour {

        public string ip = "";
        public float interval = 2;
        public bool autoStart;
        public Image[] icons;
        public Text text;
        UnityEngine.Ping ping;
        bool canRun;

        void Start() {
            SetText(0);
            if(autoStart) {
                StartPing(null);
            }
        }

        public void StartPing(string ip = null) {
            if(ip != null) {
                this.ip = ip;
            }
            StartCoroutine("PingIP");
        }

        public void StopPing() {
            canRun = false;
        }

        IEnumerator PingIP() {
            canRun = true;
            while(P2PNetwork.isNetworkActive && canRun) {
                ping = new UnityEngine.Ping(ip);
                while(!ping.isDone) {
                    yield return null;
                }
                //P2PDebug.Log("pingBackMS:" + ping.time);
                SetText(ping.time);
                yield return new WaitForSeconds(interval);
            }
        }

        void SetText(int ms) {
            Color color = Color.red;
            string colorStr = "#ff0000";
            if(ms < 10) {
                ms = 10;
            } else if(ms > 999) {
                ms = 999;
            }
            if(ms <= 100) {
                color = Color.green;
                colorStr = "#00ff00";
            } else if(ms <= 200) {
                color = Color.yellow;
                colorStr = "#ffff00";
            }
            text.text = "<color=" + colorStr + ">" + ms + " ms</color>";
            for(int i = 0;i < icons.Length;i++) {
                icons[i].color = color;
            }
        }
    }
}