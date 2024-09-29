using Scripts.Fire.Log;
using UnityEngine;

namespace Scripts.Fire
{
    /// <summary>
    /// 防止裁剪引用。
    /// <remarks>如果在主工程无引用，link.xml的防裁剪也无效。</remarks>
    /// </summary>
    [DefaultExecutionOrder(-99999)]
    public class DisStripCode : MonoBehaviour
    {
        private void Awake()
        {
            // UnityEngine.Physics
            RegisterType<Collider>();
            RegisterType<Collider2D>();
            RegisterType<Collision>();
            RegisterType<Collision2D>();
            RegisterType<CapsuleCollider2D>();

            RegisterType<Rigidbody>();
            RegisterType<Rigidbody2D>();

            RegisterType<Ray>();
            RegisterType<Ray2D>();

            // UnityEngine.Graphics
            RegisterType<Mesh>();
            RegisterType<MeshRenderer>();

            // UnityEngine.Animation
            RegisterType<AnimationClip>();
            RegisterType<AnimationCurve>();
            RegisterType<AnimationEvent>();
            RegisterType<AnimationState>();
            RegisterType<Animator>();
            RegisterType<Animation>();

            // IOSCamera ios下相机权限的问题，用这种方法就可以解决了 问题防裁剪。
            foreach (var _ in WebCamTexture.devices)
            {
            }
        }

        private static void RegisterType<T>()
        {
#if UNITY_EDITOR
            GameLog.LogDebug("DisStripCode RegisterType", typeof(T).Name);
#endif
        }

        private void Start()
        {
            Destroy(gameObject);
        }
    }
}