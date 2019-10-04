using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] private float _Height;
    [SerializeField] private float _MovementSpeed;

    public void OpenDoor()
    {
        StartCoroutine(OpeningDoor());
    }

    IEnumerator OpeningDoor()
    {
        Vector3 target = transform.position;
        target.y += _Height;
        while (transform.position.y <= _Height)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, _MovementSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
