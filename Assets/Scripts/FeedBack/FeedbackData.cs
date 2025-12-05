using System;
using FMODUnity;
using UnityEngine;

namespace Feedback
{
    [System.Serializable]
    public class FeedbackData
    {
        [Header("WaveStart Feedback Settings")]
        public EventReference startSFX;
        public ParticleSystem waveStartVFX;
        public float startShaderEffectDuration = 2f;
        public float textDuration = 1f;

        [Space(10)]

        [Header("WaveEnd Feedback Settings")]
        public EventReference endSFX;
        public ParticleSystem waveEndVFX;
        public float endShaderEffectDuration = 2f;
    }
}