using UnityEngine;

namespace BToolkit
{
    [CreateAssetMenu(menuName = "BToolkit/LanguageData")]
    public class LanguageData : ScriptableObject
    {

        [Header("Common")]
        public string ok;
        public string cancel;
        [Header("Main Menu")]
        public Sprite gameLogo;
        public Sprite btnStart;
        public string dailyRewards;
        [Header("Bluetooth")]
        public string bluetoothGun;
        public string pleaseUseARGun;
        public string buy;
        public string bluetooth;
        public string close;
        public string bluetoothDisable;
        public string active;
        public string pleaseScan;
        public string scanGuide;
        public string congratulate;
        public string activationSuccess;
        public string sorry;
        public string jumpToBTSettings;
        [Header("Loading")]
        public string loadingTip;
        [Header("Shop")]
        public string shop;
        public string watchADForDiamonds;
        public string give;
        [Header("Settings")]
        public string settings;
        public string rightHeaded;
        public string leftHanded;
        public string northUpward;
        public string frontUpward;
        public string quit;
        public string resumeGame;
        public string company;
        [Header("Battle")]
        public string scanSite;
        public string locateScene;
        public string needToScanSite;
        public string touchScreenToStart;
        public string atkByMoving;
        public string openAROutdoor;
        public string startGame;
        public string cannotScanSite;
        public string pleaseEnsureLight;
        public string youAreDead;
        public string wantYouRespawn;
        public string respawn;
        public string leave;
        public string gameOver;
        public string touchScreenGoon;
        public string bossComming;
        [Header("Score")]
        public string score;
        public Sprite pojilu;
        public string totalkills;
        public string headshots;
        public string totaltime;
        public string restart;
    }
}