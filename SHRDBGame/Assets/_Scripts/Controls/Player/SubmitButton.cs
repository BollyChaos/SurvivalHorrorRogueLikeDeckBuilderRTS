using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class SubmitButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       GetComponent<Button>().onClick.AddListener(CallDialogManager);
    }
    void CallDialogManager()
    {
     DialogManager.Instance.ReadInputValue();   
    }

    void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }
}
