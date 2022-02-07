namespace Game.Controllers
{
    public enum ControllerProcessingID
    {
        None = 0,

        /// <summary>
        /// Disable all except these, and enable these if they are disabled
        /// </summary>
        EnableOnlyThese = 2,

        /// <summary>
        /// Enable all except these if they are disabled
        /// </summary>
        EnableNotThese = 3,

        AddThese = 4,
        RemoveThese = 5,
    }
}
