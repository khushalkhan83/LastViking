using UnityEngine;

namespace Game.Controllers
{
    abstract public class MovementControllerBase : MonoBehaviour
    {
        abstract public void Move(float deltaTime);
    }
}
