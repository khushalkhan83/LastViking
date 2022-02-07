using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Views
{
    public interface IDragAndDropInventoryView
    {
        void SetIsVisibleDragAndDrop(bool value);       
        void SetDragAndDropIcon(Sprite value);
        void SetDragAndDropPosition(Vector2 value);
    }
}
