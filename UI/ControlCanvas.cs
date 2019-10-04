using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCanvas : UIScenes
{

    private void OnEnable()
    {
        Time.timeScale = 0.1f;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)){
            UIScenesManager.ShowUI<GamePlayUI>();
        }
    }

}
