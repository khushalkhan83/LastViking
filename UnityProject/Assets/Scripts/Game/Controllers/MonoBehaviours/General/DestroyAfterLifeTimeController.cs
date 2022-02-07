using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class DestroyAfterLifeTimeController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _lifeTime;

#pragma warning restore 0649
        #endregion

        public float LifeTime => _lifeTime;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(LifeTime);
            Destroy(gameObject);
        }
    }
}
