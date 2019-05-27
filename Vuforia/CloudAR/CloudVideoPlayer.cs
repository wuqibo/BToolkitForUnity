﻿using System;
using UnityEngine;

namespace BToolkit
{
    public class CloudVideoPlayer : MonoBehaviour
    {
        /// <summary>
        /// 当前使用的播放器类型
        /// </summary>
        public bool isAVProPlayer { get; protected set; }
        /// <summary>
        /// 当前视频宽
        /// </summary>
        public float videoW { get; protected set; }
        /// <summary>
        /// 当前视频高
        /// </summary>
        public float videoH { get; protected set; }

        protected Action PlayedAction;
        protected bool canListenPlayed;

        public void RegisterPlayedEvent(Action Callback) {
            this.PlayedAction = Callback;
        }

        public virtual void Play(string videoUrl) { }
    }
}