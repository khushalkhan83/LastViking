using Core.StateMachine;
using Core.States.Parametrs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDeathEffect : EffectBase
{
    private const float GRAVITY = 9.8f;

    [SerializeField]
    private Transform[] _parts;
    [SerializeField]
    private PositionParametr _hitPoint;
    [SerializeField]
    private PositionParametr _hitDirection;
    [SerializeField]
    private FloatParametr _hitForce;
    [SerializeField]
    private Animator _animator;

    private Coroutine _deathCorouting;

    [SerializeField,Range(0.01f,5f)]
    float _animationSpeed = 1;
    [SerializeField,Range(0f,10f)]
    float _animationDuration;
    [SerializeField, Range(1f, 5f)]
    float _hitInfluence;
    [SerializeField, Range(0f, 100f)]
    float _rotationSpeed = 1;

    private Vector3[] _directions;
    private Vector3[] _rotations;
    private float[] _rotationSpeeds;

    public Transform[] Parts => _parts;
    public PositionParametr HitPoint => _hitPoint;
    public PositionParametr HitDirection => _hitDirection;
    public FloatParametr HitForce => _hitForce;
    public Animator Animator => _animator;



    public override void Apply()
    {
        if (_deathCorouting!=null)
        {
            StopCoroutine(_deathCorouting);
        }
        _deathCorouting = StartCoroutine(DeathRouting());        
    }

    private IEnumerator DeathRouting()
    {
        Animator.enabled = false;
        var hitDirection = HitDirection.Position.normalized;
        _directions = new Vector3[Parts.Length];
        _rotations = new Vector3[Parts.Length];
        _rotationSpeeds = new float[Parts.Length];

        for (int i = 0; i < Parts.Length; i++)
        {
            var offset = Parts[i].transform.position - HitPoint.transform.position;
            _directions[i] = (hitDirection * _hitInfluence + offset.normalized).normalized;
            _rotations[i] = Vector3.Cross(transform.position - HitPoint.transform.position, hitDirection).normalized;
            _rotationSpeeds[i] = offset.magnitude * _rotationSpeed * HitForce.Float;
        }

        var time = 0f;
        while (time <= _animationDuration / _animationSpeed)
        {
            time += Time.deltaTime;
            var gravityVelocity = GRAVITY * Vector3.down * time;
            for (int i = 0; i < Parts.Length; i++)
            {
                var baseVelocity = HitForce.Float * _directions[i];
                var velocity = baseVelocity + gravityVelocity;
                Parts[i].transform.position += (_animationSpeed * velocity * Time.deltaTime);
                Parts[i].transform.RotateAround(Parts[i].transform.position,_rotations[i], time * _rotationSpeeds[i] * _animationSpeed);
            }
            yield return null;
        }
    }
}
