using UnityEngine;
using UnityEngine.Serialization;
using System;

namespace UnityStandardAssets.CinematicEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Anti-aliasing")]
    public class AntiAliasing : MonoBehaviour
    {
        public enum Method
        {
            Smaa,
            Fxaa
        }

        [SerializeField]
        private SMAA m_SMAA = new SMAA();

        [SerializeField]
        private FXAA m_FXAA = new FXAA();

        [SerializeField, HideInInspector]
        private int m_Method = (int)Method.Smaa;
        public int method
        {
            get { return m_Method; }

            set
            {
                if (m_Method == value)
                    return;

                m_Method = value;
                UpdateUsedAAInterface();
            }
        }

        private IAntiAliasing m_Interface;
        public IAntiAliasing current
        {
            get
            {
                return m_Interface;
            }
        }

        private Camera m_Camera;
        public Camera cameraComponent
        {
            get
            {
                if (m_Camera == null)
                    m_Camera = GetComponent<Camera>();

                return m_Camera;
            }
        }

        private void OnEnable()
        {
            m_SMAA.OnEnable(this);
            m_FXAA.OnEnable(this);

            UpdateUsedAAInterface();
        }

        private void UpdateUsedAAInterface()
        {
            if (method == (int)Method.Smaa)
                m_Interface = m_SMAA;
            else
                m_Interface = m_FXAA;
        }

        private void OnDisable()
        {
            m_SMAA.OnDisable();
            m_FXAA.OnDisable();
        }

        private void OnPreCull()
        {
            m_Interface.OnPreCull(cameraComponent);
        }

        private void OnPostRender()
        {
            m_Interface.OnPostRender(cameraComponent);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            m_Interface.OnRenderImage(cameraComponent, source, destination);
        }
    }
}
