using UnityEngine;
using UnityEngine.UI;

public class DebugDamageDisplay : MonoBehaviour
{
    [SerializeField] private Text textField;

    public void SetData(string message)
    {
        textField.text = message;
    }

    private void Start() {
        SetData("Hit me");
    }
}