using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Dialog")]
[System.Serializable]
public class DialogSO : ScriptableObject
{
    public string ConvName;
   [ExposedScriptableObject]
   public List<PhraseSO> phrases;
//    int idxPhrase = 0;
   
//    bool isFinished=false;

    public IEnumerator NextPhrase()
    {
        var PhraseIt=phrases.GetEnumerator();
        while (PhraseIt.MoveNext()) {

            yield return PhraseIt.Current;
        }
       
    }
   
   
    public void Reset()
    {
        foreach (PhraseSO phrase in phrases)
        {
            phrase.Reset();
        }
        // idxPhrase = 0;
        // isFinished= false;  
        phrases.GetEnumerator().Dispose();
    }
}
