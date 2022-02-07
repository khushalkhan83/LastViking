using System;

namespace CustomeEditorTools.EditorGameSettingsData
{
    [System.Serializable]
    public class DateTimeWraper
    {
        long ticks;
        public DateTimeWraper(DateTime dt) => ticks = dt.Ticks;
        public DateTime Value => new DateTime(ticks);
    }
}