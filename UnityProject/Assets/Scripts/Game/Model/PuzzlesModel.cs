using System;
using System.Collections.Generic;
using Game.Puzzles;
using UnityEngine;

namespace Game.Models
{
    public class PuzzlesModel : MonoBehaviour
    {
        public event Action<string> OnPuzzleActivated;

        public HashSet<TriggeredDoorWithChest> ActivePuzzles {get;private set;} = new HashSet<TriggeredDoorWithChest>();

        public void PuzzleActivated(string puzzleName) => OnPuzzleActivated?.Invoke(puzzleName);

        public void AddActivePuzzle(TriggeredDoorWithChest puzzle) => ActivePuzzles.Add(puzzle);
        public void RemoveActivePuzzle(TriggeredDoorWithChest puzzle) => ActivePuzzles.Remove(puzzle);
    }
}
