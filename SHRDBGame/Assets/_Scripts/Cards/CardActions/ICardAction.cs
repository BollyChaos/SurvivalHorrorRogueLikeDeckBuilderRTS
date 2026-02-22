using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardAction
{
    public Transform PlayerTransform { get; set; }
    public void ExecuteCardAction(CardObject cardObj);
    public void ResetCardAction();
}
