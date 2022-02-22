using UniRx;
using UnityEngine;

public class AnimationStopper : MonoBehaviour
{
    [SerializeField] private bool _enableAnimatorOnChange;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        TrackAnimatorEnabledChanged();
    }

    private void TrackAnimatorEnabledChanged()
    {
        this.ObserveEveryValueChanged(x => _animator.enabled).Subscribe(x =>
        {
            HandleAnimatorEnabledChanged();
        })/*.AddTo(this)*/;
    }

    private void HandleAnimatorEnabledChanged()
    {
        _animator.enabled = _enableAnimatorOnChange;
    }
}
