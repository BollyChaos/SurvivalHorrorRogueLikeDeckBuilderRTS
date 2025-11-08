using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
[CreateAssetMenu(menuName = "Dialog/Phrase")]
[System.Serializable]
public class PhraseSO : ScriptableObject
{
    public enum Speaker
    {
        ALEX,
        ELEANOR,
        SON,
        DAUGHTER,
        NARRATOR
    }
    [SerializeField] public Speaker speaker;
    [TextArea(3,10)]
    [SerializeField] private string text;
    public string GetText { get { return text; } }
    public float textDuration = 2.5f;

    //public IEnumerator<char> GetNextChar()
    //{
    //    int strIdx = -1;
    //    while (strIdx < text.Length)
    //    {
    //        strIdx++;
    //        if (!skip)
    //        {

    //            yield return text.ElementAt<char>(strIdx);

    //        }

    //    }
    //    isFinished = true;
    //}
    public IEnumerator GetNextChar()
    {
        float delay = textDuration / text.Length;
        var enumerator = text.GetEnumerator();

        while (enumerator.MoveNext())
        {

            yield return enumerator.Current;

            yield return new WaitForSeconds(delay);
        }
    }

    public void Reset()
    {

        text.GetEnumerator().Reset();
    }
}
