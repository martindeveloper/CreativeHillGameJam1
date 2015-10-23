using UnityEngine;
using System.Collections;

public class DoorEntity : MonoBehaviour {
    public bool IsOpened = false;

    public GameObject ClosedPrefab;
    public GameObject OpenedPrefab;

    private GameObject CurrentPrefab;

    public void Start()
    {
        if (IsOpened)
        {
            SwitchToOpened();
        }else
        {
            SwitchToClosed();
        }
    }

    public void SwitchToOpened()
    {
        Transform transform = GetComponent<Transform>();

        Destroy(CurrentPrefab);

        CurrentPrefab = (GameObject)Instantiate(OpenedPrefab, transform.position, Quaternion.identity);
        CurrentPrefab.GetComponent<Transform>().parent = transform;
    }

    public void SwitchToClosed()
    {
        Transform transform = GetComponent<Transform>();

        Destroy(CurrentPrefab);

        CurrentPrefab = (GameObject)Instantiate(ClosedPrefab, transform.position, Quaternion.identity);
        CurrentPrefab.GetComponent<Transform>().parent = transform;
    }
}
