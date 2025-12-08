using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchButton : MonoBehaviour
{
   

    public void OnButtonPress()
    {
        FindObjectOfType<FlashLightController>().ReadFlashLightInput();
    }
}
