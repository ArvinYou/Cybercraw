using UnityEngine;

public class Pause : UIScenes
{
    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    public void OnBackToGameButtonClicked()
    {
        UIScenesManager.ShowUI<GamePlayUI>();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIScenesManager.ShowUI<GamePlayUI>();
        }
    }
}