using System;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
#endif

namespace BToolkit
{
    public class IOSGameCenter
    {
#if UNITY_IOS

        static IOSGameCenter instance;
        public static IOSGameCenter Instance { get { return instance ?? (instance = new IOSGameCenter()); } }
        IOSGameCenter() { }
        bool hasTryAuthenticated;
        const string Tip = "GameCenter登陆已被取消过一次，苹果底层将不再弹出，请提示用户到手机设置里登陆GameCenter";

#region 排行榜
        /// <summary>
        /// 提交排行数据
        /// </summary>
        public bool ReportScore(string leaderboardId, int score, Action<bool> OnReportScoreCallback)
        {
            if (Social.localUser.authenticated)
            {
                Social.ReportScore(score, leaderboardId, OnReportScoreCallback);
            }
            else
            {
                if (hasTryAuthenticated)
                {
                    Debug.Log(Tip);
                    return false;
                }
                else
                {
                    Social.localUser.Authenticate((bool success) =>
                    {
                        hasTryAuthenticated = true;
                        if (success)
                        {
                            Social.ReportScore(score, leaderboardId, OnReportScoreCallback);
                        }
                        else
                        {
                            Debug.LogError(">>>>>>>>>GameCenter验证失败");
                        }
                    });
                }
            }
            return true;
        }
        /// <summary>
        /// 打开排行榜UI
        /// </summary>
        public bool ShowLeaderboard()
        {
            if (Social.localUser.authenticated)
            {
                Debug.Log(">>>>>>>>>GameCenter 已验证，直接显示UI");
                GameCenterPlatform.ShowLeaderboardUI("com.fancyar.deadlands.score", TimeScope.AllTime);
            }
            else
            {
                if (hasTryAuthenticated)
                {
                    Debug.Log(Tip);
                    return false;
                }
                else
                {
                    Debug.Log(">>>>>>>>>GameCenter 开始验证");
                    Social.localUser.Authenticate((bool success) =>
                    {
                        hasTryAuthenticated = true;
                        if (success)
                        {
                            Debug.Log(">>>>>>>>>GameCenter 验证成功");
                            GameCenterPlatform.ShowLeaderboardUI("com.fancyar.deadlands.score", TimeScope.AllTime);
                        }
                        else
                        {
                            Debug.LogError(">>>>>>>>>GameCenter 验证失败");
                        }
                    });
                }
            }
            return true;
        }

        private void OnLoadScoresCallback(IAchievement[] obj)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                Debug.Log(obj[i]);
            }
        }

        private void OnLoadScoresCallback(IScore[] obj)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                Debug.Log(obj[i]);
            }
        }
#endregion

#region 成就
        /// <summary>
        /// 提交成就数据
        /// </summary>
        public bool ReportAchievement(string achievementId, float progress, Action<bool> OnReportAchievementCallback)
        {
            if (Social.localUser.authenticated)
            {
                Social.ReportProgress(achievementId, progress, OnReportAchievementCallback);
            }
            else
            {
                if (hasTryAuthenticated)
                {
                    Debug.Log(Tip);
                    return false;
                }
                else
                {
                    Social.localUser.Authenticate((bool success) =>
                    {
                        hasTryAuthenticated = true;
                        if (success)
                        {
                            Social.ReportProgress(achievementId, progress, OnReportAchievementCallback);
                        }
                        else
                        {
                            Debug.LogError(">>>>>>>>>GameCenter验证失败");
                        }
                    });
                }
            }
            return true;
        }
        /// <summary>
        /// 打开成就榜UI
        /// </summary>
        public bool ShowAchievements()
        {
            if (Social.localUser.authenticated)
            {
                Social.ShowAchievementsUI();
            }
            else
            {
                if (hasTryAuthenticated)
                {
                    Debug.Log(Tip);
                    return false;
                }
                else
                {
                    Social.localUser.Authenticate((bool success) =>
                    {
                        hasTryAuthenticated = true;
                        if (success)
                        {
                            Social.ShowAchievementsUI();
                        }
                        else
                        {
                            Debug.LogError(">>>>>>>>>GameCenter验证失败");
                        }
                    });
                }
            }
            return true;
        }
#endregion

#endif
    }
}