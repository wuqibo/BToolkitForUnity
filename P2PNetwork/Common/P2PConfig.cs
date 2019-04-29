using UnityEngine;

namespace BToolkit.P2PNetwork
{

    [CreateAssetMenu(menuName = "BToolkit/P2PNetwork/P2PConfig")]
    public class P2PConfig : ScriptableObject
    {
        public bool canDebug = true;
        public bool showGUIInfo = true;
        public bool isTestInSamePC = true;
        public int Port = 4999;
        public int HeartbeatInterval = 2;//秒
        public int ConnTimeout = 5;//秒

        static P2PConfig instance;
        public static P2PConfig Instance
        {
            get
            {
                return instance ?? (instance = Resources.Load<P2PConfig>("P2PConfig"));
            }
        }
    }
}
