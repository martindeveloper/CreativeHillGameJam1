using UnityEngine;
using System.Collections;

public class DoorEntity : MonoBehaviour {
    public bool IsOpened = false;

    public Sprite ClosedTop;
    public Sprite ClosedBottom;

    public Sprite OpenedTop;
    public Sprite OpenedBottom;

    public SpriteRenderer TopRenderer;
    public SpriteRenderer BottomRenderer;

    public int NumberOfKeysToUnlock = 1;
    private int ActiveNumberOfKeys = 0;

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

    public void Unlock()
    {
        ActiveNumberOfKeys++;

        if(NumberOfKeysToUnlock == ActiveNumberOfKeys)
        {
            SwitchToOpened();
        }
    }

    public void SwitchToOpened()
    {
        TopRenderer.sprite = OpenedTop;
        BottomRenderer.sprite = OpenedBottom;

        IsOpened = true;
    }

    public void SwitchToClosed()
    {
        TopRenderer.sprite = ClosedTop;
        BottomRenderer.sprite = ClosedBottom;

        IsOpened = false;
    }
}
