using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Game.Models;
using System;
using Core;
using UnityEngine.Timeline;
using Extensions;

// -> single tiime activator
public class _TestCutsceneManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector _playableDirector;

    [SerializeField] private ColliderTriggerModel _krakenTrigger;

    [SerializeField] private LayerMask _activatorLayer;
    [SerializeField] private Transform _movingTarget;
    [SerializeField] private _TestMoving _moving;
    [SerializeField] private GameObject _krakenObject;

    public event Action OnCutsceneEnded;

    // called by signal reciver (work with Playable Director)
    public void SetCutsceneEnded() => OnCutsceneEnded?.Invoke();

    private void OnEnable()
    {
        StartPirate();
        _krakenTrigger.OnEnteredTrigger += OnKrakenTrigger;
    }

    private void OnDisable()
    {
        _krakenTrigger.OnEnteredTrigger -= OnKrakenTrigger;
    }

    private void OnKrakenTrigger(Collider obj)
    {
        if (obj.gameObject.InsideLayerMask(_activatorLayer))
        {
            _krakenTrigger.OnEnteredTrigger -= OnKrakenTrigger;
            StartKraken();
        }
    }

    private void StartPirate()
    {
        _moving.SetTarget(_movingTarget.position);
        _moving.StartMoving();
    }

    private void StartKraken()
    {
        //_krakenObject.SetActive(true);
        _moving.DisableMoving();
        _playableDirector.Play();
    }
}
