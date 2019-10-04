using System;
using System.Collections.Generic;
using UnityEngine;

public class UIScenesManager : MonoBehaviour {
    public static UIScenes _CurrentScene;
    public static Dictionary<Type, UIScenes> _Scenes;

    private void Awake () {
        // Debug.Log ("component in children: " + GetComponentsInChildren<UIScenes> (true).ToString ());
        _Scenes = new Dictionary<Type, UIScenes>();
        // Debug.Log ("_Scenes: " + _Scenes.ToString ());
        foreach (var scene in GetComponentsInChildren<UIScenes> (true)) {
            // Debug.Log ("Scene: " + scene.GetType ().ToString ());
            // Debug.Log ("Scene: " + scene.ToString ());
            _Scenes.Add (scene.GetType (), scene);
            scene.gameObject.SetActive (false);
        }
        ShowUI<ControlCanvas> ();
    }
    public static void ShowUI<T> () where T : UIScenes {
        if (_CurrentScene != null) {
            _CurrentScene.gameObject.SetActive (false);
        }
        var type = typeof (T);
        if (_Scenes.ContainsKey (type) == false) {
            return;
        }
        _CurrentScene = _Scenes[type];
        _CurrentScene.gameObject.SetActive (true);
    }
}