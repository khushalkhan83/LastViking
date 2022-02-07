namespace Encounters.Analytics
{

    #region IEncounterAnaliticsDataProvider
        
    public interface IEncounterAnaliticsDataProvider
    {
        EncounterAnaliticsData GetAnaliticsData();
    }

    public class SimpleAnaliticsDataProvider : IEncounterAnaliticsDataProvider
    {
        private readonly EncounterAnaliticsData data;

        public SimpleAnaliticsDataProvider(EncounterAnaliticsData data)
        {
            this.data = data;
        }

        public EncounterAnaliticsData GetAnaliticsData() => data;
    }

    #endregion

    #region Enums

    public enum EncounterUnloadReason
    {
        deactivate,
        farAwayDespawn
    }

    public enum EncounterType
    {
        location,
        time,
        special
    }

    public enum EncounterID
    {
        encounter_location_1,
        encounter_location_2,
        encounter_location_3,
        encounter_time_1,
        encounter_special_1,
        encounter_special_2,
        encounter_special_3,
        encounter_special_4,
    }

    #endregion

    #region Data
    public class EncounterAnaliticsData
    {
        public readonly EncounterID encounterID;
        public readonly string description;
        public readonly EncounterType encounterType;

        public EncounterAnaliticsData(EncounterID encounterID, string description, EncounterType encounterType)
        {
            this.encounterID = encounterID;
            this.description = description;
            this.encounterType = encounterType;
        }
    }
        
    #endregion

    #region Events


    public class EncounterUnloadEvent
    {
        public readonly EncounterAnaliticsData data;
        public readonly EncounterUnloadReason unloadReason;

        public EncounterUnloadEvent(EncounterAnaliticsData data, EncounterUnloadReason unloadReason)
        {
            this.data = data;
            this.unloadReason = unloadReason;
        }
    }

    public class EncounterCompleateEvent
    {
        public readonly EncounterAnaliticsData data;

        public EncounterCompleateEvent(EncounterAnaliticsData data)
        {
            this.data = data;
        }
    }

    public class EncounterInitEvent
    {
        public readonly EncounterAnaliticsData data;

        public EncounterInitEvent(EncounterAnaliticsData data)
        {
            this.data = data;
        }
    }

    #endregion
}