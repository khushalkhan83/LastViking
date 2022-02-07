using Core;
using Core.Controllers;
using Game.Models;
// using Firebase;
// using Firebase.Analytics;

namespace Game.Controllers
{
    public class FirebaseController : IFirebaseController, IController
    {
        [Inject] public FirebaseModel FirebaseModel { get; private set; }

        // public FirebaseApp _FirebaseInstance { get; private set; }

        void IController.Enable()
        {
        }

        void IController.Start()
        {
            FixDependencies();
        }

        void IController.Disable()
        {
        }

        private void FixDependencies()
        {
            // FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            //     var dependencyStatus = task.Result;
            //     if (dependencyStatus == DependencyStatus.Available)
            //     {
            //         _FirebaseInstance = FirebaseApp.DefaultInstance;
            //         FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            //         FirebaseModel.SetFirebaseReady();
            //     }
            //     else
            //     {
            //         string.Format(
            //             " ----- Could not resolve all Firebase dependencies: {0}", dependencyStatus).Error();
            //     }
            // });
        }
    }
}
