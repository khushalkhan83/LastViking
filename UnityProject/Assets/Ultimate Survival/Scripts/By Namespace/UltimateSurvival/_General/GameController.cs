using UnityEngine;

namespace UltimateSurvival
{
    public class GameController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private SurfaceDatabase m_SurfaceDatabase;
        [SerializeField] private ItemDatabase m_ItemDatabase;
        [SerializeField] private Camera _worldCamera;
        [SerializeField] private AudioUtils _audioUtils;

#pragma warning restore 0649
        #endregion

        public static AudioUtils Audio { get; private set; }
        public static Camera WorldCamera { get; private set; }
        public static SurfaceDatabase SurfaceDatabase { get; private set; }

        private void OnEnable()
        {
            WorldCamera = _worldCamera;
            Audio = _audioUtils;
            SurfaceDatabase = m_SurfaceDatabase;
        }
    }
}
