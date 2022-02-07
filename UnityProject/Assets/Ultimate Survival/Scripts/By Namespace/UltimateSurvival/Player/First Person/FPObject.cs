using UnityEngine;

namespace UltimateSurvival
{
    public class FPObject : PlayerBehaviour
    {
        public Message Draw = new Message();
        public Message Holster = new Message();

        public bool IsEnabled { get; private set; }
        public int TargetFOV { get { return m_TargetFOV; } }
        public float LastDrawTime { get; private set; }
        public string ObjectName { get { return m_ObjectName; } }

        public SavableItem CorrespondingItem { get; private set; }

        #region Data
#pragma warning disable 0649

        [Header("General")]

        [SerializeField] private string m_ObjectName = "Unnamed";

        [Range(15, 100)]
        [SerializeField] private int m_TargetFOV = 45;

#pragma warning restore 0649
        #endregion

        public virtual void On_Draw(SavableItem correspondingItem)
        {
            IsEnabled = true;
            CorrespondingItem = correspondingItem;
            LastDrawTime = Time.time;

            Draw.Send();
        }

        public virtual void On_Holster()
        {
            IsEnabled = false;
            CorrespondingItem = null;

            Holster.Send();
        }
    }
}
