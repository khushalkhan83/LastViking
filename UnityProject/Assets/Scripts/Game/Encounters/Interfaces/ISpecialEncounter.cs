using ActivityLog.Data;
using Game.LocalNotifications;

namespace Encounters
{
    public interface ISpecialEncounter: IEncounter, IPushNotificationProducer, IActivityLogEnterenceProducer
    {
        bool IsActivated { get; }
        void SaveSpawnerIndex(int index);
        bool TryGetSpawnerIndex(out int index);
    }
}