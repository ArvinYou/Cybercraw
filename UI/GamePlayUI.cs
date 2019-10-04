using UnityEngine;

public class GamePlayUI : UIScenes {
    private void OnEnable () {
        Time.timeScale = 1f;
    }

    private void Update () {
        if(Input.GetKeyDown(KeyCode.Escape)){
            UIScenesManager.ShowUI<Pause>();
        }else if(Input.GetKeyDown(KeyCode.Tab)){
            UIScenesManager.ShowUI<ControlCanvas>();
        }
    }
}