using System.Collections;using System.Collections.Generic;
using UnityEngine.EventSystems;using UnityEngine.Events;
using UnityEngine;using UnityEngine.UI;

public delegate void PlaceOnGrib_CallBack( Transform pTransform, int pX, int pY );
public class GridArea{

	private float mAreaStartX = 0;
	private float mAreaWidth = 0;

	private float mAreaStartY = 0;
	private float mAreaHeight = 0;

	private bool mSetPosition = true;

    protected Transform mParentArea = null;

	public GridArea( Transform pArea, Transform pSampleItem = null, bool pSetPosition = true ){

		this.mParentArea = pArea;
		this.mSetPosition = pSetPosition;

		Vector3[] lAreaCornerArray = new Vector3[4];
		pArea.GetComponent<RectTransform>().GetLocalCorners( lAreaCornerArray );

		this.mAreaStartX = lAreaCornerArray[1].x;
		this.mAreaWidth = lAreaCornerArray[2].x - lAreaCornerArray[1].x;

		this.mAreaStartY = lAreaCornerArray[1].y;
		this.mAreaHeight = lAreaCornerArray[1].y - lAreaCornerArray[0].y;

		if( pSampleItem != null ){
			Vector3[] lItemCornerArray = new Vector3[4];
			pSampleItem.GetComponent<RectTransform>().GetLocalCorners( lItemCornerArray );
			this.mAreaWidth -= lItemCornerArray[2].x - lItemCornerArray[1].x;
			this.mAreaHeight -= Mathf.Abs( lItemCornerArray[1].y - lItemCornerArray[0].y );
			if( this.mAreaHeight < 0f ){ this.mAreaHeight = 0f; }
		}// have to fit the sample item in the area ?
	}//Constructors

	public Vector3 GetPosition( int pX = 0, int pY = 0, int pXCount = 1, int pYCount = 1 ){

		if( pXCount <= 1 ){ pXCount =2; }
		if( pYCount <= 1 ){ pYCount =2; }

		return new Vector3(  	 this.mAreaStartX + this.mAreaWidth * pX / (pXCount-1)
								,this.mAreaStartY - this.mAreaHeight * pY / (pYCount-1)
								, 0.0f);
	}


	public void CloneRandomTransformOnGrid( Transform [] pSourceTransform, int pXCount, int pYCount = 1, PlaceOnGrib_CallBack pCallBack = null ){
		for( int lY = 0; lY < pYCount; lY++ ){
			for( int lX = 0; lX < pXCount; lX++ ){
				Transform lClone  = GameObject.Instantiate( pSourceTransform[ Util.GetRandom( pSourceTransform.Length-1 ) ] );
				if( this.mSetPosition ){
					lClone.transform.localPosition = this.GetPosition( pX:lX, pXCount: pXCount, pY:lY, pYCount: pYCount );
				}

				lClone.SetParent( this.mParentArea, false );
				Util.SetPivot( lClone, 0f, 1f );

				if( pCallBack != null ){
					pCallBack( lClone, lX, lY );
				}
			}
		}// Create array of transform
	}//CloneRandomTransformOnGrid

	public void CloneTransformOnGrid( Transform [] pSourceTransform, int pXCount, int pYCount = 1, PlaceOnGrib_CallBack pCallBack = null ){

		int lCloneIndex = 0;

		for( int lY = 0; lY < pYCount; lY++ ){
			for( int lX = 0; lX < pXCount; lX++ ){
				Transform lClone  = GameObject.Instantiate( pSourceTransform[ lCloneIndex % pSourceTransform.Length ] );
				if( this.mSetPosition ){
					lClone.transform.localPosition = this.GetPosition( pX:lX, pXCount: pXCount, pY:lY, pYCount: pYCount );
				}

				lClone.SetParent( this.mParentArea, false );
				Util.SetPivot( lClone, 0f, 1f );

				if( pCallBack != null ){
					pCallBack( lClone, lX, lY );
				}
				lCloneIndex++;
			}
		}// Create array of transform
	}//CloneTransformOnGrid

}//GridArea

