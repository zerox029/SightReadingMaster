using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AnimatedDraggable : Draggable
{
    private Vector3? mStartPosUI;
    private Vector3? mStartPos = null;
    public virtual void Initialize()
    {
        if (mStartPosUI == null)
        {
            mStartPos = transform.position;
            mStartPosUI = GetComponent<RectTransform>().anchoredPosition;
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = (Vector3)mStartPosUI;
        }
        base.draggable = true;
        base.clickable = true;
        base.restorePosition = true;
        base.animateRestoredPos = true;
    }
    public void MoveTo(Transform pDestination, UnityAction pEndCallback = null)
    {
        SetInteractable(false);
        Animate.MoveTo(transform, pDestination.position, false, false, 1f, () => {
            if (pEndCallback != null)
                pEndCallback();
        });
    }
    public void ReturnToStartPos(float pDuration = 0.5f, UnityAction pEndCallback = null)
    {
        SetInteractable(false);
        Animate.MoveTo(transform, (Vector3)mStartPos, false, false, pDuration, () => {
            if (pEndCallback != null)
                pEndCallback();
            SetInteractable(true);
            GetComponent<Image>().raycastTarget = true;
        });
    }
}