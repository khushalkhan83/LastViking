using Core.Views;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class MainTargetView : ViewBase
    {
        public void SetPosition(Vector3 position) => transform.position = position;
    }
}
