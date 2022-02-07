using System.Collections.Generic;
using Game.ThirdPerson.TouchSensitivity.Interfaces;
using TMPro;
using TouchControlsKit;
using UnityEngine;

namespace Game.ThirdPerson.TouchSensitivity.Implementation
{
    public class TouchSensitivity : MonoBehaviour, ITouchSensitivity
    {
        [SerializeField] private TCKTouchpad touchpad;
        // [SerializeField] private TextMeshProUGUI textField;
        // [SerializeField] private TMP_InputField inputField;
        // [SerializeField] private TextMeshProUGUI statsOutputField;

        [SerializeField] private float sensitivityModifier = 1;
        [SerializeField] private float sensitivity = 1;

        [Header("Resolution presets")]
        [SerializeField] private float lowPreset = 3f;
        [SerializeField] private float midPreset = 1.2f;
        [SerializeField] private float highPreset = 0.35f;

        private Dictionary<SensativityPreset,float> modifierByPreset;
        private void Awake()
        {
            modifierByPreset = new Dictionary<SensativityPreset, float>()
            {
                {SensativityPreset.Low,lowPreset},
                {SensativityPreset.Mid,midPreset},
                {SensativityPreset.High,highPreset}
            };
        }

        public void SetResolutionPreset(SensativityPreset preset)
        {
            sensitivityModifier = modifierByPreset[preset];
            PresentResultSensativity();
        }

        public float Sensitivity {get => sensitivity; set {sensitivity = value; PresentResultSensativity();}}

        private void Update()
        {
            touchpad.sensitivity = CalculateSensitivity();
            // textField.text = $"Sensativity: {touchpad.sensitivity}";
        }

        private float CalculateSensitivity() => sensitivity * sensitivityModifier;

        private void PresentResultSensativity()
        {
            Debug.Log($"Sensitivity {CalculateSensitivity()}");
        }
    }
}