using Core.Views;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class CreatureHealthController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _pivot;

        [Range(0f, 100f)]
        [SerializeField] private float _maxDistance;
        [Range(0f, 100f)]
        [SerializeField] private float _nearDistance = 4f;

        [Range(0f, 20)]
        [SerializeField] private float _baseScale;
        [SerializeField] private float _scaleNear = 1f;
        [SerializeField] private float _scaleMax = 0.3f;

        [SerializeField] float _vertivalSpacing = 150;
        [SerializeField] float _horisontalSpacing = 150;
#pragma warning restore 0649
        #endregion

        private RectTransform myRectTr;
        private float maxX;
        private float minX;
        private float maxY;
        private float minY;
        private float maxDx;
        private float maxDy;
        private float midX;
        private float midY;

        Vector2 centerOfScreen = Vector2.zero;
        bool isCenterCalced = false;
        private Vector2 CenterOfScreen
        {
            get
            {
                if (!isCenterCalced)
                {
                    centerOfScreen = myRectTr.rect.size / 2f;
                    isCenterCalced = true;
                }
                return centerOfScreen;
            }
        }

        public Transform Pivot => _pivot;
        public float MaxDistance => _maxDistance;
        public float BaseScale => _baseScale;

        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public ViewsSystem ViewsSystem => ViewsSystem.Instance;

        private Transform _camera;
        public Transform Camera => _camera ?? (_camera = GameObject.FindGameObjectWithTag("MainCamera").transform);
        public WorldCameraModel WorldCameraModel => ModelsSystem.Instance._worldCameraModel;
        public PlayerDeathModel PlayerDeathModel => ModelsSystem.Instance._playerDeathModel;


        public CreatureHealthView View { get; private set; }

        public bool IsCanVisible => Health.Health < Health.HealthMax && !Health.IsDead;

        private void OnEnable()
        {
            Health.OnChangeHealth += OnChangeHealthHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevival;
            PlayerDeathModel.OnRevival += OnRevival;
            OnChangeHealthHandler();
            UpdateViewPosition();
        }

        private void OnDisable()
        {
            Health.OnChangeHealth -= OnChangeHealthHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevival;
            PlayerDeathModel.OnRevival -= OnRevival;
            HideView();
        }

        private float __distance;
        private void OnChangeHealthHandler()
        {
            if (IsCanVisible)
            {
                __distance = (Camera.position - transform.position).magnitude;
                if (__distance > MaxDistance || WorldCameraModel.WorldCamera.WorldToViewportPoint(Pivot.position).z < 0)
                {
                    HideView();
                }
                else
                {
                    ShowView();

                    View.SetFillAmount(Health.Health / Health.HealthMax);
                }
            }
            else
            {
                HideView();
            }
        }

        private void OnRevival() => OnChangeHealthHandler();

        private void ShowView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<CreatureHealthView>(ViewConfigID.CreatureHealth);
                View.OnHide += OnHideHandler;
                View.transform.SetAsFirstSibling();

                myRectTr = View.GetComponent<RectTransform>();
                maxX = myRectTr.rect.width - _horisontalSpacing;
                minX = _horisontalSpacing;
                maxY = myRectTr.rect.height - _vertivalSpacing;
                minY = _vertivalSpacing;
                maxDx = (maxX - minX) / 2f;
                maxDy = (maxY - minY) / 2f;
                midX = (maxX + minX) / 2f;
                midY = (maxY + minY) / 2f;
            }
        }

        private void HideView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            View = null;
        }

        private void LateUpdate()
        {
            OnChangeHealthHandler();
            UpdateViewPosition();
        }

        private void UpdateViewPosition()
        {
            if(View != null)
            {
                Vector3 nowPos = ConvertPosition(Pivot.position);
                float willScale = 0;

                if (nowPos.z >= 0)
                {
                    float dist = Vector3.Distance(Pivot.position, WorldCameraModel.WorldCamera.transform.position);
                    if (dist < _maxDistance)
                    {
                        if (IsNearScreenCenter(nowPos))
                        {
                            willScale = 1f;
                        }
                        else
                        {
                            float lerpVal = Mathf.Clamp01((dist - _nearDistance) / (_maxDistance - _nearDistance));
                            willScale = Mathf.Lerp(_scaleNear, _scaleMax, lerpVal);
                        }
                    }
                    else
                    {
                        willScale = 0f;
                    }
                }
                View.SetAnchoredPosition(nowPos);
                View.SetScale(willScale * BaseScale);
            }
        }

        private Vector3 ConvertPosition(Vector3 p)
        {   
            Vector3 nowPoint = WorldCameraModel.WorldCamera.WorldToScreenPoint(p);
            float aspect = Screen.height / myRectTr.rect.height;
            nowPoint /= aspect;

            Vector3 planeV = new Vector3(nowPoint.x-myRectTr.rect.width/2f, nowPoint.y-myRectTr.rect.height/2f,0f);
            Vector3 planeV_Norm = planeV.normalized;
            float lerpValX = Mathf.Abs( planeV_Norm.x);
            float lerpValY = Mathf.Abs( planeV_Norm.y);

            bool willArrow = false;

            float willX = Mathf.Clamp(nowPoint.x, midX - maxDx * lerpValX, midX + maxDx * lerpValX);
            if (willX != nowPoint.x)
                willArrow = true;
            nowPoint.x = willX;

            float willY = Mathf.Clamp(nowPoint.y, midY - maxDy * lerpValY, midY + maxDy * lerpValY);
            if (willY != nowPoint.y)
                willArrow = true;
            nowPoint.y = willY;

            if (nowPoint.z < 0)
            {
                nowPoint.x = midX + maxDx * (nowPoint.x < midX ? lerpValX : (-lerpValX));
                nowPoint.y = midY + maxDy * (nowPoint.y < midY ? lerpValY : (-lerpValY));

                willArrow = true;
            }

            if (willArrow)
            {
                nowPoint.z = 0f;
            }
            return nowPoint;
        }

        bool IsNearScreenCenter(Vector2 pos)
        {
            return (Vector2.SqrMagnitude(pos - CenterOfScreen) < 1000f);
        }

    }
}
