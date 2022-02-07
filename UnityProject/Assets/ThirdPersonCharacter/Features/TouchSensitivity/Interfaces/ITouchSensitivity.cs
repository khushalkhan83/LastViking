namespace Game.ThirdPerson.TouchSensitivity.Interfaces
{
    public interface ITouchSensitivity
    {
        void SetResolutionPreset(SensativityPreset preset);
        float Sensitivity {get; set;}
    }

    public enum SensativityPreset { Low, Mid, High }

    // public class SetSensitivityRequest
    // {
    //     public float sensitivity;

    //     public SetSensitivityRequest(float sensitivity)
    //     {
    //         this.sensitivity = sensitivity;
    //     }
    // }
}