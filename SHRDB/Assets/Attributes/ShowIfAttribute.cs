using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string conditionBool;

    public ShowIfAttribute(string conditionBool)
    {
        this.conditionBool = conditionBool;
    }
}
