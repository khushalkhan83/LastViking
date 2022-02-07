using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
    /// <summary>
    /// 
    /// </summary>
    public class Ragdoll : MonoBehaviour
    {
        [System.Serializable]
        public class BoneToReparent
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private Transform m_Bone;

            [SerializeField] private Transform m_NewParent;

#pragma warning restore 0649
            #endregion

            public void Reparent()
            {
                m_Bone.SetParent(m_NewParent, true);
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Rigidbody m_Pelvis;

        [Header("Helpers")]

        [SerializeField] private BoneToReparent[] m_BonesToReparent;

        [SerializeField] private Texture m_SurfaceTexture;

        [SerializeField] private bool m_AutoAssignHitboxes = true;

#pragma warning restore 0649
        #endregion

        public bool enableRagdoll;

        //private void Update()
        //{
        //    //if (enableRagdoll)
        //    //{
        //    //    enableRagdoll = false;
        //    //    Enable();
        //    //}
        //}

        public void Enable()
        {
            //foreach (var bone in m_Bones)
            //{
            //    bone.isKinematic = false;
            //    bone.gameObject.layer = LayerMask.NameToLayer(m_RagdollLayer);
            //}

            //foreach (var bone in m_BonesToReparent)
            //    bone.Reparent();
        }

        public void Disable()
        {
            //foreach (var bone in m_Bones)
            //{
            //    bone.isKinematic = true;
            //    bone.gameObject.layer = LayerMask.NameToLayer(m_RagdollLayer);
            //}
        }

        private void Awake()
        {
            //m_Bones = GetComponentsInChildren<CharacterJoint>().Select(joint => joint.GetComponent<Rigidbody>()).ToList();
            //m_Bones.Add(m_Pelvis);
            //Disable();

            //foreach (var bone in m_Bones)
            //{
            //    if (m_AutoAssignHitboxes && bone.gameObject.GetComponent<HitBox>() == null)
            //        bone.gameObject.AddComponent<HitBox>();

            //    if (m_SurfaceTexture && bone.gameObject.GetComponent<SurfaceIdentity>() == null)
            //    {
            //        var si = bone.gameObject.AddComponent<SurfaceIdentity>();
            //        si.Texture = m_SurfaceTexture;
            //    }
            //}
        }
    }
}
