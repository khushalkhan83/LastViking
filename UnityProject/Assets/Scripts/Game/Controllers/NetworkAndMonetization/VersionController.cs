using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class VersionController : IVersionController, IController
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public VersionModel VersionModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        int _validationCoroutineId = -1;

        void IController.Enable()
        {
            #if UNITY_ANDROID
            _validationCoroutineId = CoroutineModel.InitCoroutine(Validation());
            #endif
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            #if UNITY_ANDROID
            CoroutineModel.BreakeCoroutine(_validationCoroutineId);
            #endif
        }

        private IEnumerator Validation()
        {
            if (Application.isEditor == true || Debug.isDebugBuild == true)
            {
                yield break;
            }

            var www = new WWW(VersionModel.PathVerions);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                OnEndDownload(www.text);
            }
            else
            {
                if (!VersionModel.IsLegal && Application.version == VersionModel.VersionLast)
                {
                    ViewsSystem.Show<VersionControlPopupView>(ViewConfigID.VersionControl);
                }
            }
        }

        private void OnEndDownload(string result)
        {
            var isLegal = IsLegal(result);

            VersionModel.SetIsLegal(isLegal);
            if (!VersionModel.IsLegal)
            {
                ViewsSystem.Show<VersionControlPopupView>(ViewConfigID.VersionControl);
            }
        }

        private bool IsLegal(string result)
        {
            var versions = result.Split('\n', '\r');
            foreach (var item in versions)
            {
                if (Application.version == item)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

