using System;
using UnityEngine;

[Serializable]
public class AnimatorTrigger
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _trigger;

    private int? _triggerHash;

    public void Trigger()
    {
        if (_animator == null)
        {
            return;
        }
        TryInitTriggerHash();
        _animator.SetTrigger(_triggerHash.Value);
    }

    public void Reset()
    {
        if (_animator == null)
        {
            return;
        }
        TryInitTriggerHash();
        _animator.ResetTrigger(_triggerHash.Value);
    }

    private void TryInitTriggerHash()
    {
        if (_triggerHash.HasValue || _animator == null)
        {
            return;
        }

        _triggerHash = Animator.StringToHash(_trigger);
    }
}