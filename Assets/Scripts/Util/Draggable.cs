using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void OnClick_Event();
public delegate void EndDrag_Event();

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool draggable = true;
    public bool clickable = true;
    public bool restorePosition = true;
    public bool animateRestoredPos = false;
    public bool setAsLastSiblingOnDrag = true;
    public bool setAsLastSiblingOnClick = true;

    public enum areaDropped { glissezArea = 0, cardArea = 1, exitArea = 2 }
    public areaDropped myAreaDropped = areaDropped.cardArea;
    public EventTrigger.TriggerEvent onMouseDownCallBack = null;    // to set the event callback in the Unity ui editor
    public OnClick_Event mOnMouseDownDown_EventCb = null;
    public EventTrigger.TriggerEvent onMouseUpCallBack = null;		// to set the event callback in the Unity ui editor
    public OnClick_Event mOnMouseUp_EventCb = null;
    public EventTrigger.TriggerEvent endDropCardCallBack = null;	// to set the event callback in the Unity ui editor
    public EndDrag_Event mEndDrag_EventCb = null;


    private Vector3 mBeginDragPosition = new Vector3(); // initial position of the object
    private Vector3 mBeginDragMousePosition = new Vector3(); // initial position of the mouse

    private bool mRayCastSavedState = false;
    private bool mDragAccepted = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!this.clickable) { return; }

        Util.CallBackEventTrigger(this, this.onMouseDownCallBack);

        if (this.mOnMouseDownDown_EventCb != null) { this.mOnMouseDownDown_EventCb(); }

        if (this.setAsLastSiblingOnClick)
        {
            this.transform.SetAsLastSibling();
        }

        this.mBeginDragMousePosition = eventData.position;

    }//OnPointerDown

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!this.clickable) { return; }
        Util.CallBackEventTrigger(this, this.onMouseUpCallBack);
        if (this.mOnMouseUp_EventCb != null) { this.mOnMouseUp_EventCb(); }
    }//OnPointerUp



    private void SaveObjectState()
    {
        Image lImage = this.GetComponent<Image>();
        if (lImage)
        {
            this.mRayCastSavedState = lImage.raycastTarget;
            lImage.raycastTarget = false;
        }

        this.mBeginDragPosition = this.transform.position;
    }//SaveObjectState

    private void RestoreObjectState(bool pRestorePosition = false)
    {
        Image lImage = this.GetComponent<Image>();
        if (lImage)
        {
            lImage.raycastTarget = this.mRayCastSavedState;
        }

        if (pRestorePosition) { RestorePosition(); }
    }//RestoreObjectState

    public void RestorePosition(float pAnimDuration = 0.5f)
    {
        if (!animateRestoredPos) { this.transform.position = this.mBeginDragPosition; }
        else
        {
            draggable = false; clickable = false;
            Animate.MoveTo(transform, this.mBeginDragPosition, false, false, pAnimDuration, () => {
                draggable = true; clickable = true;
            });
        }
    }

    public void ResetObjectState(bool pRestorePosition = false)
    {
        this.mRayCastSavedState = true;
        this.RestoreObjectState(pRestorePosition: pRestorePosition);
    }//ResetObjectState

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!this.draggable) { return; }
        this.SaveObjectState();
        this.GetComponent<Image>().raycastTarget = false;
        //this.mBeginDragMousePosition = eventData.position;
        this.mDragAccepted = false;

        //Debug.Log(this.name);

    }//OnBeginDrag

    public void OnDrag(PointerEventData eventData)
    {
        if (!this.draggable) { return; }
        this.transform.position = (Vector3)eventData.position - this.mBeginDragMousePosition + this.mBeginDragPosition;
    }//OnDrag
    public void OnEndDrag(PointerEventData pEventData)
    {
        if (!this.draggable) { return; }

        Debug.Log("OnEndDrag: " + mDragAccepted);

        this.RestoreObjectState(pRestorePosition: !this.mDragAccepted && restorePosition);
        float halfWidth = this.transform.GetComponent<RectTransform>().rect.width * 0.5f;
        float halfHeight = this.transform.GetComponent<RectTransform>().rect.height * 0.5f;

        Util.CallBackEventTrigger(this, this.endDropCardCallBack);

        if (this.mEndDrag_EventCb != null) { this.mEndDrag_EventCb(); }
    }//OnEndDrag

    public void DragAccepted()
    {
        this.mDragAccepted = true;
    }//DragAccepted

    public void SetInteractable(bool pBool)
    {
        clickable = pBool;
        draggable = pBool;
        // For some reason we need do disable raycastTarget 
        //  for clickable and draggable doesn't prevent some specific interactions
        this.GetComponent<Image>().raycastTarget = pBool;
    }

}//Draggable