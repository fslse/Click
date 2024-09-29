using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using Scripts.Fire.Singleton;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework.Audio
{
    /// <summary>
    /// 音效管理，为游戏提供统一的音效播放接口。
    /// </summary>
    /// <remarks>场景3D音效挂到场景物件、技能3D音效挂到技能特效上，并在AudioSource的Output上设置对应分类的AudioMixerGroup</remarks>
    [UsedImplicitly]
    public class AudioModule : Singleton<AudioModule>
    {
        private readonly AudioMixer audioMixer;
        private readonly AudioCategory[] audioCategories = new AudioCategory[(int)AudioType.Max];
        private readonly float[] categoriesVolume = new float[(int)AudioType.Max];
        private readonly bool unityAudioDisabled;
        private float volume = 1f;
        private bool enable = true;

        #region Public Propreties

        /// <summary>
        /// 实例对象根节点。
        /// </summary>
        public Transform InstanceRoot { get; }

        /// <summary>
        /// 总音量控制。
        /// </summary>
        public float Volume
        {
            get => unityAudioDisabled ? 0f : volume;
            set
            {
                if (unityAudioDisabled) return;
                volume = value;
                AudioListener.volume = volume;
            }
        }

        /// <summary>
        /// 总开关。
        /// </summary>
        public bool Enable
        {
            get => !unityAudioDisabled && enable;
            set
            {
                if (unityAudioDisabled) return;
                enable = value;
                AudioListener.volume = enable ? volume : 0f;
            }
        }

        /// <summary>
        /// 音乐音量。
        /// </summary>
        public float MusicVolume
        {
            get => unityAudioDisabled ? 0.0f : categoriesVolume[(int)AudioType.Music];
            set
            {
                if (unityAudioDisabled)
                {
                    return;
                }

                float v = Mathf.Clamp(value, 0.0001f, 1.0f);
                categoriesVolume[(int)AudioType.Music] = v;
                audioMixer.SetFloat("MusicVolume", Mathf.Log10(v) * 20f);
            }
        }

        /// <summary>
        /// 音效音量。
        /// </summary>
        public float SoundVolume
        {
            get => unityAudioDisabled ? 0.0f : categoriesVolume[(int)AudioType.Sound];
            set
            {
                if (unityAudioDisabled)
                {
                    return;
                }

                float v = Mathf.Clamp(value, 0.0001f, 1.0f);
                categoriesVolume[(int)AudioType.Sound] = v;
                audioMixer.SetFloat("SoundVolume", Mathf.Log10(v) * 20f);
            }
        }

        /// <summary>
        /// UI音效音量。
        /// </summary>
        public float UISoundVolume
        {
            get => unityAudioDisabled ? 0.0f : categoriesVolume[(int)AudioType.UISound];
            set
            {
                if (unityAudioDisabled)
                {
                    return;
                }

                float v = Mathf.Clamp(value, 0.0001f, 1.0f);
                categoriesVolume[(int)AudioType.UISound] = v;
                audioMixer.SetFloat("UISoundVolume", Mathf.Log10(v) * 20f);
            }
        }

        /// <summary>
        /// 语音音量。
        /// </summary>
        public float VoiceVolume
        {
            get => unityAudioDisabled ? 0.0f : categoriesVolume[(int)AudioType.Voice];
            set
            {
                if (unityAudioDisabled)
                {
                    return;
                }

                float v = Mathf.Clamp(value, 0.0001f, 1.0f);
                categoriesVolume[(int)AudioType.Voice] = v;
                audioMixer.SetFloat("VoiceVolume", Mathf.Log10(v) * 20f);
            }
        }

        /// <summary>
        /// 音乐开关
        /// </summary>
        public bool MusicEnable
        {
            get
            {
                if (unityAudioDisabled)
                {
                    return false;
                }

                if (audioMixer.GetFloat("MusicVolume", out var db))
                {
                    return db > -80f;
                }

                return false;
            }
            set
            {
                if (unityAudioDisabled)
                {
                    return;
                }

                audioCategories[(int)AudioType.Music].Enable = value;

                // 音乐采用0音量方式，避免恢复播放时的复杂逻辑
                if (value)
                {
                    audioMixer.SetFloat("MusicVolume", Mathf.Log10(categoriesVolume[(int)AudioType.Music]) * 20f);
                }
                else
                {
                    audioMixer.SetFloat("MusicVolume", -80f);
                }
            }
        }

        /// <summary>
        /// 音效开关。
        /// </summary>
        public bool SoundEnable
        {
            get => !unityAudioDisabled && audioCategories[(int)AudioType.Sound].Enable;
            set
            {
                if (unityAudioDisabled)
                {
                    return;
                }

                audioCategories[(int)AudioType.Sound].Enable = value;
            }
        }

        /// <summary>
        /// UI音效开关。
        /// </summary>
        public bool UISoundEnable
        {
            get => !unityAudioDisabled && audioCategories[(int)AudioType.UISound].Enable;
            set
            {
                if (unityAudioDisabled)
                {
                    return;
                }

                audioCategories[(int)AudioType.UISound].Enable = value;
            }
        }

        /// <summary>
        /// 语音开关。
        /// </summary>
        public bool VoiceEnable
        {
            get => !unityAudioDisabled && audioCategories[(int)AudioType.Voice].Enable;
            set
            {
                if (unityAudioDisabled)
                {
                    return;
                }

                audioCategories[(int)AudioType.Voice].Enable = value;
            }
        }

        #endregion

        private AudioModule()
        {
            InstanceRoot = new GameObject("Audio").transform;
            InstanceRoot.SetParent(GameApp.Instance.transform);
            InstanceRoot.localPosition = Vector3.zero;

            audioMixer = AssetManager.Instance.LoadAsset<AudioMixer>("Assets/AssetPackages/Audio/AudioMixer.mixer");
            var audioGroupConfigs = AssetManager.Instance.LoadAsset<AudioGroupConfigAsset>("Assets/AssetPackages/Audio/AudioGroupConfigAsset.asset").AudioGroupConfigs;

#if UNITY_EDITOR
            try
            {
                TypeInfo typeInfo = typeof(AudioSettings).GetTypeInfo();
                PropertyInfo propertyInfo = typeInfo.GetDeclaredProperty("unityAudioDisabled");
                unityAudioDisabled = (bool)propertyInfo.GetValue(null);
                if (unityAudioDisabled)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                GameLog.LogError(e.ToString());
            }
#endif

            for (int index = 0; index < (int)AudioType.Max; ++index)
            {
                AudioType audioType = (AudioType)index;
                AudioGroupConfig audioGroupConfig = audioGroupConfigs!.First(a => a.audioType == audioType);
                audioCategories[index] = new AudioCategory(audioGroupConfig.AgentHelperCount, InstanceRoot, audioMixer, audioGroupConfig);
                categoriesVolume[index] = audioGroupConfig.Volume;
            }

            MonoManager.Instance.AddListener(() =>
            {
                foreach (var audioCategory in audioCategories)
                {
                    audioCategory?.Update(Time.deltaTime);
                }
            });
        }

        /// <summary>
        /// 播放，如果超过最大发声数采用fadeout的方式复用最久播放的AudioSource。
        /// </summary>
        /// <param name="type">声音类型</param>
        /// <param name="path">声音文件路径</param>
        /// <param name="loop">是否循环播放</param>>
        /// <param name="vol">音量（0-1.0）</param>
        /// <param name="async">是否异步加载</param>
        /// <returns>音频代理辅助器</returns>
        public AudioAgent Play(AudioType type, string path, bool loop = false, float vol = 1.0f, bool async = false)
        {
            if (unityAudioDisabled)
            {
                return null;
            }

            AudioAgent audioAgent = audioCategories[(int)type].Play(path, async);
            if (audioAgent != null)
            {
                audioAgent.IsLoop = loop;
                audioAgent.Volume = vol;
            }

            return audioAgent;
        }

        /// <summary>
        /// 停止某类声音播放。
        /// </summary>
        /// <param name="type">声音类型。</param>
        /// <param name="fade">是否渐消。</param>
        public void Stop(AudioType type, bool fade)
        {
            if (unityAudioDisabled)
            {
                return;
            }

            audioCategories[(int)type].Stop(fade);
        }

        /// <summary>
        /// 停止所有声音。
        /// </summary>
        /// <param name="fade">是否渐消。</param>
        public void StopAll(bool fade)
        {
            if (unityAudioDisabled)
            {
                return;
            }

            for (int i = 0; i < (int)AudioType.Max; ++i)
            {
                if (audioCategories[i] != null)
                {
                    audioCategories[i].Stop(fade);
                }
            }
        }
    }
}