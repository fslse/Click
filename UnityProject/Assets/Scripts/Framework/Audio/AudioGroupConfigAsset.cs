using UnityEngine;

namespace Framework.Audio
{
    [CreateAssetMenu(menuName = "Custom/AudioGroup", fileName = "AudioGroupConfigAsset", order = -1)]
    public class AudioGroupConfigAsset : ScriptableObject
    {
        [SerializeField] private AudioGroupConfig[] audioGroupConfigs;
        public AudioGroupConfig[] AudioGroupConfigs => audioGroupConfigs;
    }
}