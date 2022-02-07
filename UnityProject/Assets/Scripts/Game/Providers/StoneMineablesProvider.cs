using UnityEngine;

namespace Game.Providers
{
    public class StoneMineablesProvider : MonoBehaviour
    {

        [SerializeField] private MeshRenderer[] _meshRenderers;

        public MeshRenderer[] MeshRenderers => _meshRenderers;

        [SerializeField] private MinebleFractureObject[] _stonesMinable;

        public int StonesCount => _stonesMinable.Length;
    }
}
