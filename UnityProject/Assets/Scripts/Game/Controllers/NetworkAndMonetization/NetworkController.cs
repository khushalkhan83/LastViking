using Core;
using Core.Controllers;
using Game.Models;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Controllers
{
    public class NetworkController : INetworkController, IController
    {
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private int _processCheckConnection = -1;

        void IController.Enable()
        {
            NetworkModel.OnCheckConnection += OnCheckConnectionHandler;
            ApplicationCallbacksModel.ApplicationFocus += OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause += OnApplicationPause;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            StopCheckConnectionProcess();

            NetworkModel.OnCheckConnection -= OnCheckConnectionHandler;
            ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;
            CoroutineModel.BreakeCoroutine(_processCheckConnection);
        }

        private void OnCheckConnectionHandler()
        {
            StartCheckConnectionProcess();
        }

        private void StartCheckConnectionProcess()
        {
            StopCheckConnectionProcess();
            _processCheckConnection = CoroutineModel.InitCoroutine(CheckConnectionProcess(OnCheckConnectionHandler));
        }

        private void StopCheckConnectionProcess()
        {
            CoroutineModel.BreakeCoroutine(_processCheckConnection);
        }

        private void OnCheckConnectionHandler(bool isHasConnection)
        {
            NetworkModel.SetIsHasConnection(isHasConnection);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                NetworkModel.UpdateInternetConnectionStatus();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                NetworkModel.UpdateInternetConnectionStatus();
            }
        }

        IEnumerator CheckConnectionProcess(Action<bool> callback)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                callback?.Invoke(false);
                yield break;
            }

            using (var request = new UnityWebRequest(NetworkModel.ServerAddress))
            {
                yield return request.SendWebRequest();
                var ishasConnection = !request.isNetworkError;
                callback?.Invoke(ishasConnection);
            }
        }
    }
}
