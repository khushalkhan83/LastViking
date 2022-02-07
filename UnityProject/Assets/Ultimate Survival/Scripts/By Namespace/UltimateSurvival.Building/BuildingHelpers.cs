using Game.Models;
using UnityEngine;
using Extensions;

namespace UltimateSurvival.Building
{
    /// <summary>
    /// A collection of helpers, used by the PlayerBuilder to spawn previews, snap, place, etc.
    /// </summary>
    [System.Serializable]
    public class BuildingHelpers
    {

        #region Data
#pragma warning disable 0649

        [SerializeField] private LayerMask m_BuildingPieceMask;

        [SerializeField] private LayerMask m_FreePlacementMask;

        [SerializeField] private LayerMask buildingBlockersMask;

        [SerializeField] private int m_BuildRange;

        [Header("Preview Pulsing Effect")]

        [SerializeField] private bool m_UsePulseEffect = true;

        [SerializeField] private bool m_PulseWhenSnapped = true;

        [SerializeField] private float m_PulseEffectDuration = 2f;

        [SerializeField] private float m_PulseMin = 0.4f;

        [SerializeField] private float m_PulseMax = 0.9f;

#pragma warning restore 0649
        #endregion

        public BuildingPiece CurrentPreviewPiece { get; private set; }

        public bool HasSocket { get; set; }

        public bool PlacementAllowed { get; private set; } = true;
        public Color PreviewColor { get => _previewColor; set => _previewColor = value; }

        public float RotationOffset { get; set; }

        private GameObject m_CurrentPreview;

        private Socket m_LastValidSocket;
        private Transform m_Transform;
        private AlphaPulse m_Pulse;

        private BuildingPiece m_CurrentPrefab;
        private Color _previewColor;

        private bool _snapped;

        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;

        public bool IsPreviewExists => m_CurrentPreview != null;
        public GameObject CurrentPreview => m_CurrentPreview;

        public void Initialize(Transform t, PlayerEventHandler pl, AudioSource aS)
        {
            m_Transform = t;
            m_Pulse = new AlphaPulse(PreviewColor, m_PulseMin, m_PulseMax);
        }

        public void ManagePreview()
        {
            ManageCollision();

            if (m_UsePulseEffect)
                ApplyPulse();

            var renderers = CurrentPreviewPiece.Renderers;

            for (int r = 0; r < renderers.Count; r++)
            {
                Material[] newMats = renderers[r].materials;
                for (int m = 0; m < newMats.Length; m++)
                    newMats[m].color = PreviewColor;

                renderers[r].materials = newMats;
            }
        }

        private void ManageCollision()
        {
            bool overlapsStuff = CurrentPreviewPiece.IsBlockedByTerrain();

            if (!overlapsStuff)
            {
                Collider[] overlappingStuff = Physics.OverlapBox(CurrentPreviewPiece.Bounds.center, CurrentPreviewPiece.Bounds.extents, CurrentPreviewPiece.transform.rotation, buildingBlockersMask, QueryTriggerInteraction.Ignore);

                for (int o = 0; o < overlappingStuff.Length; o++)
                {
                    if (!CurrentPreviewPiece.HasCollider(overlappingStuff[o]))
                    {
                        bool isTerrain = overlappingStuff[o].gameObject.InsideLayerMask(m_BuildingPieceMask);

                        if (!isTerrain)
                        {
                            var piece = overlappingStuff[o].GetComponent<BuildingPiece>();

                            bool isSameBuilding = piece && HasSocket && piece.Building == m_LastValidSocket.Piece.Building;

                            if (!isSameBuilding)
                            {
                                overlapsStuff = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (HasSocket)
            {
                PlacementAllowed = !overlapsStuff;
            }
            else
                PlacementAllowed = !CurrentPreviewPiece.RequiresSockets && !overlapsStuff && _snapped;

            UpdatePreviewColor();
        }

        private void UpdatePreviewColor()
        {
            PreviewColor = PlacementAllowed ? new Color(0, 1, 0, PreviewColor.a) : new Color(1, 0, 0, PreviewColor.a);
        }

        private void ApplyPulse()
        {
            if (!m_PulseWhenSnapped && HasSocket)
            {
                _previewColor.a = 1;

                return;
            }

            m_Pulse.StartPulse(m_PulseEffectDuration);

            _previewColor.a = m_Pulse.UpdatePulse();
        }

        public void LookForSnaps()
        {
            Collider[] buildingPieces = Physics.OverlapSphere(m_Transform.position, m_BuildRange, m_BuildingPieceMask, QueryTriggerInteraction.Ignore);

            if (buildingPieces.Length > 0)
                HandleSnapPreview(buildingPieces);
            else if (!RaycastAndPlace())
                HandleFreePreview();
        }

        private void HandleFreePreview()
        {
            Transform toCurrentPos = (CurrentPreviewPiece.OutOfGroundHeight == 0) ? m_Transform : GameController.WorldCamera.transform;
            Vector3 currentPos = toCurrentPos.position + toCurrentPos.forward * m_BuildRange;

            if (CurrentPreviewPiece.OutOfGroundHeight == 0)
            {
                RaycastHit hit;
                Vector3 startPos = m_CurrentPreview.transform.position + new Vector3(0, 0.25f, 0);

                bool raycast = Physics.Raycast(startPos, Vector3.down, out hit, 1f, m_FreePlacementMask, QueryTriggerInteraction.Ignore); // 1f 

                if (raycast)
                {
                    _snapped = true;
                    currentPos.y = hit.point.y;
                }
                else
                {
                    _snapped = false;
                }
            }
            else
            {
                float minMove = CurrentPreviewPiece.AllowUnderTerrainMovement ? (m_Transform.position.y - CurrentPreviewPiece.OutOfGroundHeight) : 0;

                currentPos.y = Mathf.Clamp(currentPos.y, minMove, (m_Transform.position.y + CurrentPreviewPiece.OutOfGroundHeight));
                _snapped = true;
            }

            m_CurrentPreview.transform.position = currentPos;
            m_CurrentPreview.transform.rotation = m_Transform.rotation * m_CurrentPrefab.transform.localRotation * Quaternion.Euler(CurrentPreviewPiece.RotationAxis * RotationOffset);

            m_LastValidSocket = null;
            HasSocket = false;
        }

        private void HandleSnapPreview(Collider[] buildingPieces)
        {
            var cam = Camera.main;

            if(cam == null) 
                return;

            var ray = cam.ViewportPointToRay(Vector3.one * 0.5f);

            var smallestAngleToSocket = Mathf.Infinity;
            Socket targetSocket = null;

            for (int bp = 0; bp < buildingPieces.Length; bp++)
            {
                BuildingPiece buildingPiece = buildingPieces[bp].GetComponent<BuildingPiece>();
                if (buildingPiece == null || buildingPiece.Sockets.Length == 0)
                    continue;

                for (int s = 0; s < buildingPiece.Sockets.Length; s++)
                {
                    Socket socket = buildingPiece.Sockets[s];

                    if (socket.SupportsPiece(CurrentPreviewPiece))
                    {
                        if ((socket.transform.position - m_Transform.position).sqrMagnitude < Mathf.Pow(m_BuildRange, 2))
                        {
                            float angleToSocket = Vector3.Angle(ray.direction, socket.transform.position - ray.origin);

                            if (angleToSocket < smallestAngleToSocket && angleToSocket < 35f)
                            {
                                smallestAngleToSocket = angleToSocket;
                                targetSocket = socket;
                            }
                        }
                    }
                }
            }

            if (targetSocket != null)
            {
                Socket.PieceOffset pieceOffset;
                if (targetSocket.GetPieceOffsetByName(m_CurrentPrefab.Name, out pieceOffset))
                {
                    m_CurrentPreview.transform.position = targetSocket.transform.position + targetSocket.transform.TransformVector(pieceOffset.PositionOffset);
                    m_CurrentPreview.transform.rotation = targetSocket.transform.rotation * pieceOffset.RotationOffset;
                    m_LastValidSocket = targetSocket;
                    HasSocket = true;

                    return;
                }
            }

            if (RaycastAndPlace())
            {
                _snapped = true;
            }
            else
            {
                HandleFreePreview();
            }
        }

        private bool RaycastAndPlace()
        {
            var cam = Camera.main;
            var ray = cam.ViewportPointToRay(Vector3.one * 0.5f);

            if (Physics.Raycast(ray, out var hitInfo, m_BuildRange, m_FreePlacementMask, QueryTriggerInteraction.Ignore))
            {
                m_CurrentPreview.transform.position = hitInfo.point;
                m_CurrentPreview.transform.rotation = m_Transform.rotation * m_CurrentPrefab.transform.localRotation * Quaternion.Euler(CurrentPreviewPiece.RotationAxis * RotationOffset);

                return true;
            }

            return false;
        }

        public void SpawnPreview(GameObject prefab)
        {
            m_CurrentPreview = Object.Instantiate(prefab);
            m_CurrentPreview.transform.position = Vector3.one * 10000f;//Remove this shit
            m_CurrentPreview.transform.SetParent(m_Transform);

            CurrentPreviewPiece = m_CurrentPreview.GetComponent<BuildingPiece>();
            CurrentPreviewPiece.SetState(PieceState.Preview);

            m_CurrentPrefab = prefab.GetComponent<BuildingPiece>();

            PreviewColor = CurrentPreviewPiece.Renderers[0].material.color;
        }

        public void PlacePiece()
        {
            if (m_CurrentPreview == null)
                return;

            var model = m_CurrentPrefab.GetComponent<WorldObjectModel>();
            var result = WorldObjectCreator.Create(model.WorldObjectID, m_CurrentPreview.transform.position, m_CurrentPreview.transform.rotation);

            var piece = result.GetComponent<BuildingPiece>();
            piece.SetState(PieceState.Placed);

            if (piece.PlacementFX)
                Object.Instantiate(piece.PlacementFX, piece.transform.position, piece.transform.rotation);

            m_LastValidSocket = null;
            RotationOffset = 0f;
            HasSocket = false;
        }

        private bool IntersectsSocket(Ray ray, Socket socket)
        {
            Vector3 L = socket.transform.position - ray.origin;
            float tca = Vector3.Dot(L, ray.direction);

            if (tca < 0f)
                return false;

            float d2 = Vector3.Dot(L, L) - tca * tca;
            if (d2 > socket.Radius * socket.Radius)
                return false;

            return true;
        }

        public void ClearPreview()
        {
            if (m_CurrentPreview != null)
            {
                Object.Destroy(m_CurrentPreview.gameObject);

                m_CurrentPreview = null;
                CurrentPreviewPiece = null;
            }
        }
    }
}
