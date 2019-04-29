using UnityEngine;
using System.Collections;
using UnityEditor;

public class AnimatorPlayer {

    double m_PreviousTime;
    bool m_Playing;
    float m_RunningTime;
    float m_RecorderStopTime;
    internal Animator animator;

    public AnimatorPlayer(Animator animator) {
        this.animator = animator;
        EditorApplication.update += InspectorUpdate;
    }

    public void Play(string animName) {
        if (!Application.isPlaying) {
            if (!animator) {
                Debug.LogError("animator is null");
                return;
            }
            Bake(animName);
            m_RunningTime = 0f;
            m_PreviousTime = EditorApplication.timeSinceStartup;
            m_Playing = true;
        }
    }

    void Bake(string animName) {
        float frameRate = 30f;
        int frameCount = 1000;
        animator.Rebind();
        animator.StopPlayback();
        animator.recorderStartTime = 0;
        animator.Play(animName);
        // 开始记录指定的帧数
        animator.StartRecording(frameCount);
        float deltaTime = 1.0f / frameRate;
        for (var i = 0;i < frameCount - 1;i++) {
            animator.Update(deltaTime);
        }
        // 完成记录
        animator.StopRecording();
        // 开启回放模式
        animator.StartPlayback();
        m_RecorderStopTime = animator.recorderStopTime;
    }

    public void OnDisable() {
        EditorApplication.update -= InspectorUpdate;
    }

    void InspectorUpdate() {
        if (!Application.isPlaying) {
            if (m_Playing) {
                float deltaTime = (float)(EditorApplication.timeSinceStartup - m_PreviousTime);
                m_PreviousTime = EditorApplication.timeSinceStartup;
                m_RunningTime += deltaTime;
                if (m_RunningTime > m_RecorderStopTime) {
                    m_Playing = false;
                    return;
                }
                // 设置回放的时间位置
                animator.playbackTime = m_RunningTime;
                animator.Update(0);
            }
        }
    }
}
