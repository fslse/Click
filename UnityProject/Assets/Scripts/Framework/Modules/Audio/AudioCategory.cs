using System.Collections.Generic;
using System.Linq;
using Scripts.Fire.Log;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework.Modules.Audio
{
    public class AudioCategory
    {
        private readonly List<AudioAgent> AudioAgents;
        private bool enable = true;

        /// <summary>
        /// 音频混响器。
        /// </summary>
        public AudioMixer AudioMixer { get; }

        /// <summary>
        /// 音频混响器组。
        /// </summary>
        public AudioMixerGroup AudioMixerGroup { get; }

        /// <summary>
        /// 音频组配置。
        /// </summary>
        public AudioGroupConfig AudioGroupConfig { get; }

        /// <summary>
        /// 实例对象根节点。
        /// </summary>
        public Transform InstanceRoot { get; }

        /// <summary>
        /// 是否启用该音频轨道。
        /// </summary>
        public bool Enable
        {
            get => enable;
            set
            {
                if (enable != value)
                {
                    enable = value;
                    if (!enable)
                    {
                        Stop(false);
                    }
                }
            }
        }

        /// <summary>
        /// 音频轨道构造函数。
        /// </summary>
        /// <param name="channelCount">Channel数量。</param>
        /// <param name="root">音频模块根节点。</param>
        /// <param name="audioMixer">音频混响器。</param>
        /// <param name="audioGroupConfig">音频轨道组配置。</param>
        public AudioCategory(int channelCount, Transform root, AudioMixer audioMixer, AudioGroupConfig audioGroupConfig)
        {
            AudioMixer = audioMixer;
            AudioGroupConfig = audioGroupConfig;
            AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups($"Master/{AudioGroupConfig.audioType.ToString()}");
            AudioMixerGroup = audioMixerGroups.Length > 0 ? audioMixerGroups[0] : audioMixer.FindMatchingGroups("Master")[0];

            InstanceRoot = new GameObject($"Audio Category - {AudioMixerGroup.name}").transform;
            InstanceRoot.SetParent(root);

            AudioAgents = new List<AudioAgent>(32);
            for (int index = 0; index < channelCount; index++)
            {
                AudioAgent audioAgent = new AudioAgent();
                audioAgent.Init(this, index);
                AudioAgents.Add(audioAgent);
            }
        }

        /// <summary>
        /// 播放指定音频。
        /// </summary>
        /// <param name="path">音频资源路径。</param>
        /// <param name="async">是否异步。</param>
        /// <returns></returns>
        public AudioAgent Play(string path, bool async)
        {
            if (!enable)
            {
                return null;
            }

            int freeChannel = -1;
            float duration = -1;

            for (int i = 0; i < AudioAgents.Count; i++)
            {
                if (AudioAgents[i].IsFree)
                {
                    freeChannel = i;
                    break;
                }

                if (AudioAgents[i].Duration > duration)
                {
                    duration = AudioAgents[i].Duration;
                    freeChannel = i;
                }
            }

            if (freeChannel >= 0)
            {
                if (AudioAgents[freeChannel] == null)
                {
                    AudioAgents[freeChannel] = AudioAgent.Create(path, async, this);
                }
                else
                {
                    AudioAgents[freeChannel].Load(path, async);
                }

                return AudioAgents[freeChannel];
            }

            GameLog.LogError($"Here is no channel to play audio {path}");
            return null;
        }

        /// <summary>
        /// 停止播放该轨道所有音频。
        /// </summary>
        /// <param name="fade">是否渐出。</param>
        public void Stop(bool fade)
        {
            foreach (var audioAgent in AudioAgents.Where(agent => agent != null))
            {
                audioAgent.Stop(fade);
            }
        }

        /// <summary>
        /// 音频轨道轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds)
        {
            foreach (var audioAgent in AudioAgents.Where(agent => agent != null))
            {
                audioAgent.Update(elapseSeconds);
            }
        }
    }
}