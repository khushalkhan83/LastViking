using UnityEngine;

namespace UltimateSurvival
{
    public class EntityEventHandler : MonoBehaviour
    {
        public ChangedValue<float> Health = new ChangedValue<float>(100f);
        public Attempt<HealthEventData> ChangeHealth = new Attempt<HealthEventData>();
        public ChangedValue<bool> IsGrounded = new ChangedValue<bool>(true);
        public ChangedValue<Vector3> Velocity = new ChangedValue<Vector3>(Vector3.zero);
        public Message<float> Land = new Message<float>();
        public Message Death = new Message();
        public Message Respawn = new Message();
    }
}
