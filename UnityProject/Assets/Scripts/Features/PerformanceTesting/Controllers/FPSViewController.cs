using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSViewController : MonoBehaviour
{
    [SerializeField] private FPSCountView _view;
    [SerializeField] private FPSCountModel _fPSCountModel;

    private void OnEnable()
    {
        _fPSCountModel.OnAvarageFPSChanged += HandleOnAvarageFPSChanged;
    }

    private void OnDisable()
    {
        _fPSCountModel.OnAvarageFPSChanged -= HandleOnAvarageFPSChanged;
    }

    private void HandleOnAvarageFPSChanged(int count)
    {
        _view.SetFPSCount(count);
    }
}
