using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TioController : EnemyController
{
    [SerializeField]private float hearingRange;
    private Vector3? lastHeardSoundPosition;

    private void Awake()
    {
        SetChaseSpeed(7);
        SetPatrolSpeed(3);
        base.Awake();
        SetState(new TioPatrolling(this));
    }
    #region sonidos

    public void OnSoundHeard(Vector3 soundPosition)
    {
        float distance = Vector3.Distance(transform.position, soundPosition);
        if (distance <= hearingRange)
        {
            lastHeardSoundPosition = soundPosition;
            SetState(new AbueloChasing(this, soundPosition));
        }
    }
    public Vector3? GetLastHeardSoundPosition()
    {
        return lastHeardSoundPosition;
    }

    #endregion
}
