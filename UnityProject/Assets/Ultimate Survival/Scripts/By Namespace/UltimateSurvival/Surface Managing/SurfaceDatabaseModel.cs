using UnityEngine;

namespace UltimateSurvival
{
    public class SurfaceDatabaseModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private SurfaceDatabase _surfaceDatabase;

#pragma warning restore 0649
        #endregion

        private SurfaceDatabase __surfaceDatabase;

        public SurfaceDatabase SurfaceDatabase
        {
            get
            {
                if (!__surfaceDatabase)
                {
                    __surfaceDatabase = Instantiate(_surfaceDatabase);
                }

                return __surfaceDatabase;
            }
        }
    }
}
