using UnityEngine;
using UnityEngine.UI;

public class FPSCountView : MonoBehaviour
{
    [SerializeField] private Text _fpsCountText;
    public void SetFPSCount(int count) => _fpsCountText.text = count.ToString();
}
