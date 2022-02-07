using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public class SceneReflection : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ReflectionProbe m_ReflectionProbe;

#pragma warning restore 0649
        #endregion

        private IEnumerator Start()
        {
            var waitInterval = new WaitForSeconds(0.2f);

            while (true)
            {
                m_ReflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
                m_ReflectionProbe.RenderProbe();
                yield return waitInterval;
            }
        }
    }
}
