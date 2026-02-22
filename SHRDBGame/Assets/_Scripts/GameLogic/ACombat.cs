using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACombat : MonoBehaviour
{
    public Stats stats;
    public abstract void TakeDamage(float amount);
}
