using System;
using UnityEngine;

namespace Game.Models
{
    public class LoseMainViewModel : MonoBehaviour
    {
        public event Action OnPlayAgain;

        public void PlayAgain() => OnPlayAgain?.Invoke();
    }
}
