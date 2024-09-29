using System;
using UnityEngine;

namespace Framework.AudioModule
{
    /// <summary>
    /// 音频轨道组配置。
    /// </summary>
    [Serializable]
    public sealed class AudioGroupConfig
    {
        [SerializeField] private string name;

        [SerializeField] private bool mute;

        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        [SerializeField] private int agentHelperCount = 1;

        public AudioType audioType;

        public AudioRolloffMode audioRolloffMode = AudioRolloffMode.Logarithmic;

        public float minDistance = 1f;

        public float maxDistance = 500f;

        // 属性

        public string Name => name;

        public bool Mute => mute;

        public float Volume => volume;

        public int AgentHelperCount => agentHelperCount;
    }

    [CreateAssetMenu(menuName = "Custom/AudioGroup", fileName = "AudioGroupConfigAsset", order = -1)]
    public class AudioGroupConfigAsset : ScriptableObject
    {
        [SerializeField] private AudioGroupConfig[] audioGroupConfigs;
        public AudioGroupConfig[] AudioGroupConfigs => audioGroupConfigs;
    }
}