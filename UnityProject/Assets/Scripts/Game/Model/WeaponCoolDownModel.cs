using Game.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCoolDownModel : MonoBehaviour
{
    public float CoolDownTime;
    public float CoolDownTimeDefault;

    public float LastFireTime { get; set; }

    public Dictionary<string, float> CoolDownValues = new Dictionary<string, float>();
}
