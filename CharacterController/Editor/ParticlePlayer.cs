using UnityEngine;
using System.Collections;
using UnityEditor;

public class ParticlePlayer {

    float m_RunningTime,m_StopTime;
    double m_PreviousTime;
    bool m_Playing;
    ParticleSystem particleSystem;

    public ParticlePlayer() {
        EditorApplication.update += InspectorUpdate;
    }

    public void Play(ParticleSystem particleSystem) {
        if (!Application.isPlaying && particleSystem) {
            this.particleSystem = particleSystem;
            m_RunningTime = 0f;
            m_StopTime = particleSystem.duration;
            m_PreviousTime = EditorApplication.timeSinceStartup;
            m_Playing = true;
        }
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

                if (!particleSystem) {
                    m_Playing = false;
                    return;
                }
                if (!particleSystem.loop) {
                    if (m_RunningTime >= m_StopTime) {
                        m_Playing = false;
                        return;
                    }
                }
                particleSystem.Simulate(m_RunningTime,true);
                SceneView.RepaintAll();
            }
        }
    }
}
