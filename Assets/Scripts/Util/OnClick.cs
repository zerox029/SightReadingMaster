
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;
using UnityEngine.Events;

using UnityEngine;
using UnityEngine.UI;

//OnClick_Event is define in draggable
public class OnClick : MonoBehaviour, IPointerDownHandler {

	public EventTrigger.TriggerEvent onClickCallBack = null; 	// to set the event callback in the Unity ui editor

	public OnClick_Event mOnClick = null;						// to set the event callback in the code

	public void OnPointerDown(PointerEventData eventData){

		Util.CallBackEventTrigger( this, this.onClickCallBack );

		if( this.mOnClick != null ){ this.mOnClick(); }
    }//OnPointerDown

}//OnClick
