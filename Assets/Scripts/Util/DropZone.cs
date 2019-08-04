using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void DropZone_Event( Draggable pDrag );
public delegate bool DropZone_Accept( Draggable pDrag );

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public DropZone_Event mPointerEnter_EventCb = null;
	public DropZone_Event mPointerExit_EventCb = null;
	public DropZone_Event mDrop_EventCb = null;

	public DropZone_Accept mDrop_CanAccept = null;

    public void OnPointerEnter(PointerEventData eventData){
        if( this.mPointerEnter_EventCb != null ){
			Draggable lDrag = eventData.pointerDrag.GetComponent<Draggable>();
			this.mPointerEnter_EventCb( lDrag );
		}
    }//OnPointerEnter

    public void OnPointerExit(PointerEventData eventData){
        if( this.mPointerExit_EventCb != null ){
			Draggable lDrag = eventData.pointerDrag.GetComponent<Draggable>();
			this.mPointerExit_EventCb( lDrag );
		}
    }//OnPointerExit

    public void OnDrop(PointerEventData eventData){

		// Debug.Log( "OnDrop dropzone" );

		Draggable lDrag = eventData.pointerDrag.GetComponent<Draggable>();

		bool lAcceptDrop = true;

		if( this.mDrop_CanAccept != null ){ lAcceptDrop = this.mDrop_CanAccept( lDrag ); }

		if( lAcceptDrop ){
			if( this.mDrop_EventCb != null ){
				this.mDrop_EventCb( lDrag );
				lDrag.DragAccepted();
			}
		}else{

		}
    }//OnDrop

}//DropZone
