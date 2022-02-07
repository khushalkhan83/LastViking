using UnityEngine;

namespace UltimateSurvival
{
    public class ObjectDestructor : MonoBehaviour
    {

        #region Data
#pragma warning  disable 0649

        [SerializeField] private float m_TimeOut = 1f;

        [SerializeField] private bool m_DetachChildren;

#pragma warning restore 0649
        #endregion

        private void Awake()
        {
            Invoke("DestroyNow", m_TimeOut);
        }

        private void DestroyNow()
        {
            if (m_DetachChildren)
                transform.DetachChildren();

            Destroy(gameObject);
        }
    }
}
