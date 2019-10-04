using UnityEngine;

public class DoorControl : MonoBehaviour
{
    private Tile[] _Tiles;
    private Door _DoorMesh;

    private void Awake()
    {
        _DoorMesh = GetComponentInChildren<Door>();
        _Tiles = GetComponentsInChildren<Tile>();
    }

    private void Start()
    {
        foreach (var item in _Tiles)
        {
            item.SetIsWall();
        }
    }
    public void OpenDoor()
    {
        foreach (var tile in _Tiles)
        {
            tile.SetIsNotWall();
        }
        _DoorMesh.OpenDoor();
    }
}
