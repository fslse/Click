using System;
using Cysharp.Threading.Tasks;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework.AudioModule
{
    /// <summary>
    /// 音频代理辅助器。
    /// </summary>
    public class AudioAgent
    {
        private Transform transform;
        private AudioSource source;

        private float volume = 1f;
        private float fadeOutTimer;
        private const float fadeOutDuration = 0.2f;

        private AudioAgentRuntimeState audioAgentRuntimeState = AudioAgentRuntimeState.None;

        /// <summary>
        /// AudioSource实例ID。
        /// </summary>
        public int InstanceId { get; private set; }

        /// <summary>
        /// 音量。
        /// </summary>
        public float Volume
        {
            get => volume;
            set
            {
                if (source)
                {
                    volume = value;
                    source.volume = volume;
                }
            }
        }

        /// <summary>
        /// 是否循环。
        /// </summary>
        public bool IsLoop
        {
            get => source && source.loop;
            set
            {
                if (source)
                {
                    source.loop = value;
                }
            }
        }

        /// <summary>
        /// 是否正在播放。
        /// </summary>
        public bool IsPlaying => source && source.isPlaying;

        /// <summary>
        /// 音频代理辅助器实例位置。
        /// </summary>
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        /// <summary>
        /// 音频代理辅助器当前是否空闲。
        /// </summary>
        public bool IsFree => source == null || audioAgentRuntimeState == AudioAgentRuntimeState.End;

        /// <summary>
        /// 音频代理辅助器播放秒数。
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// 音频代理辅助器当前音频长度。
        /// </summary>
        public float Length
        {
            get
            {
                if (source != null && source.clip != null)
                {
                    return source.clip.length;
                }

                return 0;
            }
        }

        /// <summary>
        /// 获取声源。
        /// </summary>
        /// <returns></returns>
        public AudioSource GetAudioSource()
        {
            return source;
        }

        /// <summary>
        /// 创建音频代理辅助器。
        /// </summary>
        /// <param name="path">音频资源路径。</param>
        /// <param name="async">是否异步。</param>
        /// <param name="audioCategory">音频轨道（类别）。</param>
        /// <returns></returns>
        public static AudioAgent Create(string path, bool async, AudioCategory audioCategory)
        {
            AudioAgent audioAgent = new AudioAgent();
            audioAgent.Init(audioCategory);
            audioAgent.Load(path, async);
            return audioAgent;
        }

        /// <summary>
        /// 初始化音频代理辅助器。
        /// </summary>
        /// <param name="audioCategory">音频轨道（类别）。</param>
        /// <param name="index">音频代理辅助器编号。</param>
        public void Init(AudioCategory audioCategory, int index = 0)
        {
            GameObject host = new GameObject($"Audio Agent Helper - {audioCategory.AudioMixerGroup.name} - {index}");
            host.transform.SetParent(audioCategory.InstanceRoot);
            host.transform.localPosition = Vector3.zero;
            transform = host.transform;
            source = host.AddComponent<AudioSource>();
            source.playOnAwake = false;
            AudioMixerGroup[] audioMixerGroups =
                audioCategory.AudioMixer.FindMatchingGroups($"Master/{audioCategory.AudioMixerGroup.name}/{audioCategory.AudioMixerGroup.name} - {index}");
            source.outputAudioMixerGroup = audioMixerGroups.Length > 0 ? audioMixerGroups[0] : audioCategory.AudioMixerGroup;
            source.rolloffMode = audioCategory.AudioGroupConfig.audioRolloffMode;
            source.minDistance = audioCategory.AudioGroupConfig.minDistance;
            source.maxDistance = audioCategory.AudioGroupConfig.maxDistance;
            InstanceId = source.GetInstanceID();
        }

        /// <summary>
        /// 音频代理加载请求。
        /// </summary>
        private class LoadRequest
        {
            public string path;
            public bool async;
        }

        /// <summary>
        /// 音频代理加载请求。
        /// </summary>
        private LoadRequest pendingLoad;

        /// <summary>
        /// 加载音频代理辅助器。
        /// </summary>
        /// <param name="path">资源路径。</param>
        /// <param name="async">是否异步。</param>
        public void Load(string path, bool async)
        {
            if (audioAgentRuntimeState is AudioAgentRuntimeState.None or AudioAgentRuntimeState.End)
            {
                Duration = 0;
                if (string.IsNullOrEmpty(path)) return;
                if (async)
                {
                    audioAgentRuntimeState = AudioAgentRuntimeState.Loading;
                    AssetManager.Instance.LoadAssetAsync<AudioClip>(path, OnAssetLoadComplete).Forget();
                }
                else
                {
                    OnAssetLoadComplete(AssetManager.Instance.LoadAsset<AudioClip>(path));
                }
            }
            else
            {
                pendingLoad = new LoadRequest
                {
                    path = path,
                    async = async
                };
                if (audioAgentRuntimeState == AudioAgentRuntimeState.Playing)
                {
                    Stop(true);
                }
            }
        }

        /// <summary>
        /// 停止播放。
        /// </summary>
        /// <param name="fade"></param>
        public void Stop(bool fade = false)
        {
            if (source)
            {
                if (fade)
                {
                    fadeOutTimer = fadeOutDuration;
                    audioAgentRuntimeState = AudioAgentRuntimeState.FadingOut;
                }
                else
                {
                    source.Stop();
                    audioAgentRuntimeState = AudioAgentRuntimeState.End;
                }
            }
        }

        /// <summary>
        /// 暂停播放。
        /// </summary>
        public void Pause()
        {
            if (source)
            {
                source.Pause();
            }
        }

        /// <summary>
        /// 取消暂停。
        /// </summary>
        public void UnPause()
        {
            if (source)
            {
                source.UnPause();
            }
        }

        /// <summary>
        /// 轮询音频代理辅助器。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds)
        {
            switch (audioAgentRuntimeState)
            {
                case AudioAgentRuntimeState.Playing:
                {
                    if (!source.isPlaying)
                    {
                        audioAgentRuntimeState = AudioAgentRuntimeState.End;
                    }

                    break;
                }
                case AudioAgentRuntimeState.FadingOut when fadeOutTimer > 0f:
                {
                    fadeOutTimer -= elapseSeconds;
                    source.volume = volume * fadeOutTimer / fadeOutDuration;
                    break;
                }
                case AudioAgentRuntimeState.FadingOut:
                {
                    Stop();
                    if (pendingLoad != null)
                    {
                        string path = pendingLoad.path;
                        bool async = pendingLoad.async;
                        pendingLoad = null;
                        Load(path, async);
                    }

                    source.volume = volume;
                    break;
                }
                case AudioAgentRuntimeState.None:
                    break;
                case AudioAgentRuntimeState.Loading:
                    break;
                case AudioAgentRuntimeState.End:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Duration += elapseSeconds;
        }

        /// <summary>
        /// 资源加载完成。
        /// </summary>
        /// <param name="audioClip">资源。</param>
        private void OnAssetLoadComplete(AudioClip audioClip)
        {
            if (pendingLoad != null)
            {
                audioAgentRuntimeState = AudioAgentRuntimeState.End;
                string path = pendingLoad.path;
                bool async = pendingLoad.async;
                pendingLoad = null;
                Load(path, async);
            }
            else if (audioClip != null)
            {
                source.clip = audioClip;
                audioAgentRuntimeState = AudioAgentRuntimeState.Playing;
                source.Play();
            }
            else
            {
                GameLog.LogWarning("AudioClip is null.");
                audioAgentRuntimeState = AudioAgentRuntimeState.End;
            }
        }
    }
}