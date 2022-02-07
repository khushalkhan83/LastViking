using System.Collections;
using System.Collections.Generic;
using Game.ThirdPerson.TouchSensitivity.Interfaces;
using TMPro;
using UnityEngine;

namespace Game.ThirdPerson.TouchSensitivity
{
    public class TestSensitivityController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField sensitivityInputField;
        private ITouchSensitivity sensitivity;

        private void Awake()
        {
            sensitivity = GetComponent<ITouchSensitivity>();

            sensitivityInputField.onValueChanged.AddListener((value) => sensitivity.Sensitivity = float.Parse(value));
        }
    }
}