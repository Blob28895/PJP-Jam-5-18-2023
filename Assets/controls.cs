using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controls : MonoBehaviour
{
    public GameObject controlsMessage;
    public GameObject showHideMessage;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
		{
            controlsMessage.SetActive(!controlsMessage.activeInHierarchy);
            showHideMessage.SetActive(!showHideMessage.activeInHierarchy);
		}
    }
}
