using System;
using Game.Models;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDamagedListener : MonoBehaviour
{
    [SerializeField] private PlayerDamageReceiver player;
    [SerializeField] private UnityEvent onDamaged;

    #region MonoBehaviour
    private void Awake()
    {
        if (player == null)
        {
            throw new System.Exception("Initialization error");
        }
    }
    private void OnEnable()
    {
        player.OnTakeDamage += OnTakeDamage;
    }

    private void OnDisable()
    {
        player.OnTakeDamage -= OnTakeDamage;
    }
    #endregion

    private void OnTakeDamage(float arg1, GameObject arg2)
    {
        RiseDamagedEvents();
    }
    
    [Button]
    private void RiseDamagedEvents() => onDamaged?.Invoke();
}
