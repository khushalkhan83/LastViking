using Game.Audio;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

public class ToolsAudio : MonoBehaviour
{
    private AudioSystem AudioSystem => AudioSystem.Instance;
    private HotBarModel HotBarModel => ModelsSystem.Instance._hotBarModel;

    public AudioID AudioID;

    public void OnEnable()
    {
        if (HotBarModel.IsCanPlayAudio)
        {
            AudioSystem.PlayOnce(AudioID);
        }
    }
}
