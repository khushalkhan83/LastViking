using AlmostEngine.Preview;
using Core;
using Core.Controllers;
using Core.Views;
using CustomeEditorTools;
using Game.Controllers;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI_Template.Animations;
using UnityEngine;
using StackViews = System.Collections.Generic.Stack<Core.Views.IView>;

namespace Game.Views
{
    public class ViewsSystem : MonoBehaviour
    {
        private static ViewsSystem instance;
        public static ViewsSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    var variable = Resources.Load<ViewsSystemVariable>("SO_System_Views");
                    instance = variable.Value;
                }
                return instance;
            }
        }

        #region Data
#pragma warning disable 0649
        [SerializeField] private ViewsSystemVariable _variable;
        [SerializeField] private ViewsProvider _viewsProvider;
        [SerializeField] private LayersProvider _layersProvider;
        [SerializeField] private ViewsMapper _ViewsMapper;
        [SerializeField] private InjectionSystem _injectionSystem;

        [Header("Pool settings")]
        [SerializeField] private Transform _defaultPool;
        [SerializeField] private Transform _baseLayerPool;

#pragma warning restore 0649
        #endregion

        public Provider<ViewID, Component> ViewsProvider => _viewsProvider;
        public Provider<LayerID, Transform> LayersProvider => _layersProvider;
        public ViewsMapper ViewsMapper => _ViewsMapper;
        public InjectionSystem InjectionSystem => _injectionSystem;

        public Hashtable PoolViews { get; } = new Hashtable();
        public Hashtable ActiveControllers { get; } = new Hashtable();
        public Hashtable PoolControllers { get; } = new Hashtable();
        public IDictionary<ViewConfigID, HashSet<IView>> ActiveViews { get; } = new Dictionary<ViewConfigID, HashSet<IView>>();
        public IDictionary<IView, ViewConfigData> Configs { get; } = new Dictionary<IView, ViewConfigData>();

        private UniqueAction<ViewConfigID> OnBeginShowAction { get; } = new UniqueAction<ViewConfigID>();
        private UniqueAction<ViewConfigID> OnEndShowAction { get; } = new UniqueAction<ViewConfigID>();
        private UniqueAction<ViewConfigID> OnBeginHideAction { get; } = new UniqueAction<ViewConfigID>();
        private UniqueAction<ViewConfigID> OnEndHideAction { get; } = new UniqueAction<ViewConfigID>();

        public IUniqueEvent<ViewConfigID> OnBeginShow => OnBeginShowAction;
        public IUniqueEvent<ViewConfigID> OnEndShow => OnEndShowAction;
        public IUniqueEvent<ViewConfigID> OnBeginHide => OnBeginHideAction;
        public IUniqueEvent<ViewConfigID> OnEndHide => OnEndHideAction;

        public event Action OnHideAll;

        //

        private void Awake()
        {
            _variable.Value = this;
        }

        private IViewController GetController(IView view) => (IViewController)ActiveControllers[view];

        private void ActivateController(IView view, IViewController viewController)
        {
            ActiveControllers[view] = viewController;
            viewController.Enable(view);
        }

        private void ActivateController(IView view, IDataViewController data, IViewController viewController)
        {
            if (data != null)
            {
                ((IViewControllerData)viewController).Set(data);
            }
            ActivateController(view, viewController);
        }

        private void RegisterActiveView(ViewConfigID viewConfigID, IView view)
        {
            HashSet<IView> pool;
            if (!ActiveViews.ContainsKey(viewConfigID))
            {
                pool = new HashSet<IView>();
                ActiveViews[viewConfigID] = pool;
            }
            else
            {
                pool = ActiveViews[viewConfigID];
            }

            pool.Add(view);
        }

        private void UnregisterActiveView(ViewConfigID viewConfigID, IView view)
        {
            if (ActiveViews.ContainsKey(viewConfigID))
            {
                ActiveViews[viewConfigID].Remove(view);
            }
        }

        protected bool TryGetViewFromPool(Type type, out IView result)
        {
            if (PoolViews.ContainsKey(type))
            {
                var views = (StackViews)PoolViews[type];
                if (views.Count > 0)
                {
                    result = views.Pop();
                    return true;
                }
            }

            result = default;
            return false;
        }

        protected StackViews GetViews(Type type)
        {
            if (PoolViews.ContainsKey(type))
            {
                return (StackViews)PoolViews[type];
            }

            var views = new StackViews();

            PoolViews[type] = views;

            return views;
        }

        protected void SetViewToPool(IView view) => GetViews(view.GetType()).Push(view);

        protected IView CreateView(ViewID viewID, Transform container) => (IView)Instantiate(ViewsProvider[viewID], container);

        private bool TryGetViewControllerPool(Type typeController, out IViewController viewController)
        {
            if (PoolControllers.ContainsKey(typeController))
            {
                var stack = (Stack<IViewController>)PoolControllers[typeController];
                if (stack.Count > 0)
                {
                    viewController = stack.Pop();
                    PoolControllers.Remove(typeController);
                    return true;
                }
            }

            viewController = default;
            return false;
        }

        protected bool TryGetController(Type typeController, out IViewController controller)
        {
            if (typeController != null)
            {
                if (!TryGetViewControllerPool(typeController, out controller))
                {
                    controller = (IViewController)gameObject.AddComponent(typeController);
                    InjectionSystem.Inject(controller);
                }

                return true;
            }

            controller = default;
            return false;
        }

        //

        public bool IsShow(ViewConfigID viewConfigID) => ActiveViews.ContainsKey(viewConfigID) && ActiveViews[viewConfigID].Count > 0;

        public T Show<T>(ViewConfigID viewConfigID, IDataViewController data = default)
            where T : Component, IView
            => (T)Show(viewConfigID, data);

        public T Show<T>(ViewConfigID viewConfigID, Transform container, IDataViewController data = default)
            where T : Component, IView
            => (T)Show(viewConfigID, container, data);

        public IView Show(ViewConfigID viewConfigID, IDataViewController data = default)
        {
            var viewConfigData = ViewsMapper[viewConfigID];
            var container = LayersProvider[viewConfigData.LayerID];
            var view = GetView(container, viewConfigData.View, viewConfigData.ViewID);

            Configs[view] = viewConfigData;

            RegisterActiveView(viewConfigData.ViewConfigID, view);

            if (TryGetController(viewConfigData.Controller, out var controller))
            {
                ActivateController(view, data, controller);
            }

            OnBeginShowAction.Invoke(viewConfigData.ViewConfigID);

            view.OnShow += OnShowViewHandler;
            view.Show();

            return view;
        }

        public IView Show(ViewConfigID viewConfigID, Transform container, IDataViewController data = default)
        {
            var viewConfigData = ViewsMapper[viewConfigID];
            var view = GetView(container, viewConfigData.View, viewConfigData.ViewID);

            Configs[view] = viewConfigData;

            if (TryGetController(viewConfigData.Controller, out var controller))
            {
                ActivateController(view, data, controller);
            }

            OnBeginShowAction.Invoke(viewConfigData.ViewConfigID);

            view.OnShow += OnShowViewHandler;
            view.Show();

            return view;
        }

        private IView GetView(Transform container, Type type, ViewID viewID)
        {
            Component component;
            if (TryGetViewFromPool(type, out var view))
            {
                component = (Component)view;
                component.transform.SetParent(container);
            }
            else
            {
                view = CreateView(viewID, container);
                component = (Component)view;
            }

            component.gameObject.SetActive(true);

            return view;
        }

        public void Hide(IView view)
        {
            var viewConfigData = Configs[view];

            UnregisterActiveView(viewConfigData.ViewConfigID, view);
            HideView(view, viewConfigData);
        }

        private void HideView(IView view, ViewConfigData viewConfigData)
        {
            if (viewConfigData.Controller != null)
            {
                GetController(view)?.Disable();
            }

            view.OnHide += OnHideViewHandler;
            view.Hide();

            OnBeginHideAction.Invoke(viewConfigData.ViewConfigID);
        }

        public void HideAll()
        {
            foreach (var views in ActiveViews.Values.ToList())
            {
                if (views.Count > 0)
                {
                    var viewsList = views.ToList();
                    foreach (var view in viewsList)
                    {
                        var poolStack = GetViews(view.GetType());
                        bool isViewInPool = poolStack != null ? poolStack.Contains(view) : false;
                        if (!isViewInPool)
                        {
                            HideView(view, Configs[view]);
                        }
                    }
                    views.Clear();
                }
            }

            OnHideAll?.Invoke();
        }

        public void ShowLayer(LayerID layerID) => SetVisibleLayer(layerID, true);
        public void HideLayer(LayerID layerID) => SetVisibleLayer(layerID, false);

        protected void SetVisibleLayer(LayerID layerID, bool isVisible) => LayersProvider[layerID].gameObject.SetActive(isVisible);

        private void OnShowViewHandler(IView view)
        {
            view.OnShow -= OnShowViewHandler;

            OnEndShowAction.Invoke(Configs[view].ViewConfigID);
        }

        private void OnHideViewHandler(IView view)
        {
            view.OnHide -= OnHideViewHandler;
            var viewComponent = (Component)view;

            var viewConfigData = Configs[view];

            bool viewGameObjectExist = viewComponent != null;

            if (viewGameObjectExist)
            {
                viewComponent.gameObject.SetActive(false);
                viewComponent.transform.SetParent(GetPool());
            }


            SetViewToPool(view);
            RelesaseViewController(view);

            OnEndHideAction.Invoke(Configs[view].ViewConfigID);

            Transform GetPool()
            {
                if (viewConfigData.LayerID == LayerID.Base)
                {
                    return _baseLayerPool;
                }
                else return _defaultPool;
            }
        }

        private void RelesaseViewController(IView view)
        {
            if (ActiveControllers.ContainsKey(view))
            {
                var controller = (IViewController)ActiveControllers[view];
                ActiveControllers.Remove(view);
                CacheViewController(controller.GetType(), controller);
            }
        }

        private void CacheViewController(Type viewControllerType, IViewController viewController)
        {
            Stack<IViewController> stack;
            if (PoolControllers.ContainsKey(viewControllerType))
            {
                stack = (Stack<IViewController>)PoolControllers[viewControllerType];
            }
            else
            {
                stack = new Stack<IViewController>();
                PoolControllers[viewControllerType] = stack;
            }

            stack.Push(viewController);
        }
        [Button] void TestSaveAreas() => ValidateViewsSafeArea(true);
        [Button] void TestAnimations() => ValidateUIOpenAnimations(true);

        public bool ValidateViewsSafeArea(bool showPopup = false, bool forceShowPopupOnError = false)
        {
            bool isValid = true;

            var views = Enum.GetValues(typeof(ViewConfigID)) as ViewConfigID[];

            Dictionary<LayerID, StringBuilder> stringContainerByLayer = new Dictionary<LayerID, StringBuilder>();

            var layers = (Enum.GetValues(typeof(LayerID)) as LayerID[]).ToList();

            foreach (var layer in layers)
            {
                stringContainerByLayer.Add(layer, new StringBuilder());
            }

            foreach (var viewID in views)
            {
                if (viewID == ViewConfigID.None) continue;

                var data = ViewsMapper[viewID];

                var viewPrefab = ViewsProvider[data.ViewID];

                var safeArea = viewPrefab.GetComponentInChildren<SafeArea>(true);

                StringBuilder sb;

                try
                {
                    sb = stringContainerByLayer[data.LayerID];
                }
                catch (System.Exception)
                {
                    Debug.Log("Error");
                    throw;
                }

                SafeArea layerSafeArea = GetSafeAreaFromLayerCanvas(data);

                if (safeArea != null)
                {
                    if (layerSafeArea != null)
                    {
                        sb.AppendLine($"++ {viewID} has Safe Area. Olso it contains safe area on Layer");
                    }
                    else
                    {
                        sb.AppendLine($"+ {viewID} has Safe Area");
                    }
                }
                else
                {
                    if (data.LayerID == LayerID.None)
                    {
                        sb.AppendLine($"+ {data.ViewID} has no Layer");
                        continue;
                    }

                    if (layerSafeArea != null)
                    {
                        sb.AppendLine($"+ {data.ViewID} has Safe Area on {data.LayerID.ToString()}");
                    }
                    else
                    {
                        sb.AppendLine($"- {data.ViewID} no safe area");
                        isValid = false;
                    }
                }
            }

            StringBuilder output = new StringBuilder();

            foreach (var layer in layers)
            {
                var sb = stringContainerByLayer[layer];
                output.AppendLine($"======={layer.ToString()} Layer=======");
                output.AppendLine(sb.ToString());
                output.AppendLine();
            }

            if (showPopup)
            {
                EditorOutputUtil.PresentMessage(output.ToString());
            }
            else
            {
                if (!isValid && forceShowPopupOnError)
                    EditorOutputUtil.PresentMessage(output.ToString());
            }

            return isValid;

            SafeArea GetSafeAreaFromLayerCanvas(ViewConfigData data)
            {
                if (data.LayerID == LayerID.None) return null;
                var layer = LayersProvider[data.LayerID];
                var layerSafeArea = layer.GetComponent<SafeArea>();
                return layerSafeArea;
            }
        }

        public bool ValidateUIOpenAnimations(bool showPopup = false, bool forceShowPopupOnError = false)
        {
            Type animationComponentType = typeof(OpenAnimation);
            string animationComponentaName = animationComponentType.Name;
            var layersWithAnimationEffect = new LayerID[] { LayerID.Popup, LayerID.Window };

            bool isValid = true;

            var views = Enum.GetValues(typeof(ViewConfigID)) as ViewConfigID[];

            Dictionary<LayerID, StringBuilder> stringContainerByLayer = new Dictionary<LayerID, StringBuilder>();

            var layers = (Enum.GetValues(typeof(LayerID)) as LayerID[]).ToList();

            foreach (var layer in layers)
            {
                stringContainerByLayer.Add(layer, new StringBuilder());
            }

            foreach (var viewID in views)
            {
                if (viewID == ViewConfigID.None) continue;

                var data = ViewsMapper[viewID];

                var viewPrefab = ViewsProvider[data.ViewID];

                var windowOpenEffect = viewPrefab.GetComponentInChildren(animationComponentType, true);

                StringBuilder sb;

                try
                {
                    sb = stringContainerByLayer[data.LayerID];
                }
                catch (System.Exception)
                {
                    Debug.Log("Error");
                    throw;
                }

                var layerID = data.LayerID;
                if (windowOpenEffect != null)
                {
                    if (layersWithAnimationEffect.Contains(layerID))
                    {
                        sb.AppendLine($"+ {viewID} has {animationComponentaName} component");
                    }
                    else
                    {
                        sb.AppendLine($"- {viewID} not expected {animationComponentaName} component for layer {layerID}");
                        isValid = false;
                    }
                }
                else
                {
                    if (layersWithAnimationEffect.Contains(layerID))
                    {
                        sb.AppendLine($"- {data.ViewID} missing {animationComponentaName} component");
                        isValid = false;
                    }
                    else
                    {
                        sb.AppendLine($"+ {data.ViewID} {layerID} no need {animationComponentaName} component");
                    }
                }
            }

            StringBuilder output = new StringBuilder();

            foreach (var layer in layers)
            {
                var sb = stringContainerByLayer[layer];
                output.AppendLine($"======={layer.ToString()} Layer=======");
                output.AppendLine(sb.ToString());
                output.AppendLine();
            }

            if (showPopup)
            {
                EditorOutputUtil.PresentMessage(output.ToString());
            }
            else
            {
                if (!isValid && forceShowPopupOnError)
                    EditorOutputUtil.PresentMessage(output.ToString());
            }

            return isValid;
        }
    }
}
