using Core;
using UnityEngine;

namespace Game.Models
{
    public class InputModel : MonoBehaviour
    {
        private UniqueAction<PlayerActions> OnInputAction { get; } = new UniqueAction<PlayerActions>();

        public IUniqueEvent<PlayerActions> OnInput => OnInputAction;

        public void Input(PlayerActions playerAction) => OnInputAction.Invoke(playerAction);
    }
}
    