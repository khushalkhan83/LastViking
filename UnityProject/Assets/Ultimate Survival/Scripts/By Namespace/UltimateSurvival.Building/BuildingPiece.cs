using Game.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UltimateSurvival.Building
{
    public class BuildingPiece : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string m_PieceName;
        [SerializeField] private int m_itemID;

        [SerializeField] private RequiredItem[] m_RequiredItems;

        [SerializeField] private Vector3 m_RotationAxis = Vector3.forward;

        [Header("Setup")]

        [SerializeField] private bool m_ShowBounds;

        [SerializeField] private Bounds m_Bounds;

        [Tooltip("If left empty, it will automatically get populated with the first MeshFilter found.")]
        [SerializeField] private MeshFilter m_MainMesh;

        [SerializeField] private List<Renderer> m_IgnoredRenderers;

        [SerializeField] private List<Collider> m_IgnoredColliders;

        [Header("Placing")]

        [SerializeField] private BuildingSpace m_NeededSpace;

        [SerializeField] private BuildingSpace[] m_SpacesToOccupy;

        [SerializeField] private bool m_RequiresSockets;

        [SerializeField] private float m_OutOfGroundHeight;

        [SerializeField] private bool m_AllowUnderTerrainMovement;

        [Header("Stability")]

        [SerializeField] private bool m_CheckStability = true;

        [Space]

        [SerializeField] private LayerMask m_StabilityCheckMask;

        [SerializeField] private bool m_ShowStabilityBox;

        [SerializeField] private Bounds m_StabilityBox;

        [Header("Terrain Protection")]

        [SerializeField] private bool m_EnableTP;

        [SerializeField] private bool m_ShowTP;

        [SerializeField] private Bounds m_TPBox;

        [Header("Sound And Effects")]

        [SerializeField] private SoundPlayer m_BuildAudio;

        [SerializeField] private GameObject m_PlacementFX;

#pragma warning restore 0649
        #endregion

        private NavMeshObstacle _navMeshObstacle;

        public BuildingHolder Building { get; set; }
        public BuildingPiece AttachedOn { get; set; }
        public BuildingPiece BuildableObject { get; internal set; }

        public bool AllowUnderTerrainMovement => m_AllowUnderTerrainMovement;
        public bool RequiresSockets => m_RequiresSockets;
        public float OutOfGroundHeight => m_OutOfGroundHeight;
        public string Name => m_PieceName;
        public int ItemID => m_itemID;
        public Vector3 RotationAxis => m_RotationAxis;
        public Bounds Bounds => new Bounds(transform.position + transform.TransformVector(m_Bounds.center), m_Bounds.size);
        public PieceState State => m_State;
        public BuildingSpace NeededSpace => m_NeededSpace;
        public BuildingSpace[] SpacesToOccupy => m_SpacesToOccupy;
        public SoundPlayer BuildAudio => m_BuildAudio;
        public GameObject PlacementFX => m_PlacementFX;
        public MeshFilter MainMesh => m_MainMesh;
        public RequiredItem[] RequiredItems => m_RequiredItems;
        public List<Renderer> Renderers => m_Renderers;
        public Socket[] Sockets => m_Sockets;

        private Dictionary<Renderer, Material[]> m_InitialMaterials = new Dictionary<Renderer, Material[]>();
        private Socket[] m_Sockets = new Socket[0];
        private PieceState m_State = PieceState.Preview;
        private List<Collider> m_Colliders = new List<Collider>();
        private List<Renderer> m_Renderers = new List<Renderer>();

        private GameObject Item;
        private bool m_Initialized;

        private void Awake()
        {
            if (m_MainMesh == null)
                m_MainMesh = GetComponentInChildren<MeshFilter>();

            _navMeshObstacle = GetComponentInChildren<NavMeshObstacle>();

            // Get all the renderers without the ones from the ignore list.
            GetComponentsInChildren<Renderer>(m_Renderers);
            m_Renderers.RemoveAll((Renderer r) => { return m_IgnoredRenderers.Contains(r); });

            for (int i = 0; i < m_Renderers.Count; i++)
                m_InitialMaterials.Add(m_Renderers[i], m_Renderers[i].sharedMaterials);

            // Get all the colliders without the ones from the ignore list.
            GetComponentsInChildren<Collider>(m_Colliders);
            m_Colliders.RemoveAll((Collider col) => { return m_IgnoredColliders.Contains(col); });

            for (int i = 0; i < m_Colliders.Count; i++)
            {
                for (int j = 0; j < m_Colliders.Count; j++)
                {
                    if (m_Colliders[i] != m_Colliders[j])
                        Physics.IgnoreCollision(m_Colliders[i], m_Colliders[j]);
                }
            }

            m_Sockets = GetComponentsInChildren<Socket>();

            m_Initialized = true;
        }

        public void SetState(PieceState state)
        {
            if (!m_Initialized)
                Awake();

            if (state == PieceState.Preview)
            {
                SetMaterials((Material)null);

                if (_navMeshObstacle!= null)
                {
                    _navMeshObstacle.enabled = false;
                }

                foreach (var col in m_Colliders)
                {
                    if (col)
                        col.enabled = false;
                    else
                        Debug.LogError("A collider was found null in the collider list!", this);
                }

                foreach (var socket in m_Sockets)
                    socket.gameObject.SetActive(false);
            }
            else if (state == PieceState.Placed)
            {
                SetMaterials(m_InitialMaterials);

                if (_navMeshObstacle != null)
                {
                    _navMeshObstacle.enabled = true;
                }

                foreach (var col in m_Colliders)
                    col.enabled = true;

                foreach (var socket in m_Sockets)
                    socket.gameObject.SetActive(true);

                if (m_CheckStability && AttachedOn != null)
                {
                    var eventHandler = AttachedOn.GetComponent<EntityEventHandler>();
                    if (eventHandler != null)
                        eventHandler.Death.AddListener(On_SocketDeath);
                }
            }

            m_State = state;
        }

        private void On_SocketDeath()
        {
            if (gameObject != null)
            {
                EntityEventHandler entityHandler = GetComponent<EntityEventHandler>();
                entityHandler.ChangeHealth.Try(new HealthEventData(-Mathf.Infinity));
            }
        }

        private void OnDestroy()
        {
            if (AttachedOn != null)
                AttachedOn.GetComponent<EntityEventHandler>().Death.RemoveListener(On_SocketDeath);
        }

        private void SetMaterials(Material material)
        {
            for (int r = 0; r < m_Renderers.Count; r++)
            {
                Material[] oldMats = m_Renderers[r].materials;
                for (int m = 0; m < oldMats.Length; m++)
                    oldMats[m] = material;

                m_Renderers[r].materials = oldMats;
            }
        }

        private void SetMaterials(Dictionary<Renderer, Material[]> materials)
        {
            for (int r = 0; r < m_Renderers.Count; r++)
            {
                Material[] oldMats = m_Renderers[r].materials;
                for (int m = 0; m < oldMats.Length; m++)
                    oldMats[m] = materials[m_Renderers[r]][m];

                m_Renderers[r].materials = oldMats;
            }
        }

        public bool IsBlockedByTerrain()
        {
            if (!m_EnableTP)
                return false;

            Collider[] overlappingStuff = Physics.OverlapBox(transform.position + transform.TransformVector(m_TPBox.center), m_TPBox.extents, transform.rotation, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < overlappingStuff.Length; i++)
            {
                if (overlappingStuff[i] as TerrainCollider != null)
                    return true;
            }

            return false;
        }

        public bool HasCollider(Collider col)
        {
            for (int i = 0; i < m_Colliders.Count; i++)
            {
                if (m_Colliders[i] == col)
                    return true;
            }

            return false;
        }

        public bool HasSupport(out Collider collider)
        {
            var colsInRadius = Physics.OverlapBox(transform.position + transform.TransformVector(m_StabilityBox.center), m_StabilityBox.extents, transform.rotation, m_StabilityCheckMask);
            for (int i = 0; i < colsInRadius.Length; i++)
                if (!colsInRadius[i].isTrigger && !HasCollider(colsInRadius[i]))
                {
                    collider = colsInRadius[i];
                    return true;
                }

            collider = null;

            return false;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            var oldMatrix = Gizmos.matrix;
            Gizmos.color = Color.blue;

            // Draw the bounds
            if (m_ShowBounds)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.TransformVector(m_Bounds.center), transform.rotation, m_Bounds.size);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }

            // Draw the terrain protection box
            Gizmos.color = Color.yellow;
            if (m_ShowTP)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.TransformVector(m_TPBox.center), transform.rotation, m_TPBox.size);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }

            // Draw the stability box
            Gizmos.color = Color.red;
            if (m_ShowStabilityBox)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.TransformVector(m_StabilityBox.center), transform.rotation, m_StabilityBox.size);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }

            Gizmos.matrix = oldMatrix;
        }
#endif
    }
}