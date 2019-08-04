
using System;using System.Collections;using System.Collections.Generic;using UnityEngine; using UnityEngine.UI;using UnityEngine.EventSystems;
public class Util{

	static public void ParentShowOnlyMe( Transform pMe ){
		SetActiveOneChildren( pMe.parent, pMe.GetSiblingIndex() );
	}
	static public void SetActiveOneChildren( Transform pParent, int pChildrenIndex ){
		for( int lIndex = 0; lIndex < pParent.childCount; lIndex++ ){
			Util.SetActive( pParent.GetChild( lIndex ), lIndex == pChildrenIndex );
		}
	}//showOnlyOneChildren

	static public Transform GetOneChildrenRandom( Transform pParent ){
		int lChildrenIndex = GetRandom( pParent.childCount - 1 );
		if( lChildrenIndex < pParent.childCount ){
			return pParent.GetChild( lChildrenIndex );
		}
		return null;
	}//GetOneChildrenRandom

	static public void SetActiveAllChildren( Transform pParent, bool pActive = true ){
			foreach (Transform lChild in pParent ){
				lChild.gameObject.SetActive( pActive );
		}
	}//setActiveAllChildren

	static public void SetActiveAllChildrenRecursive( Transform pParent, bool pActive = true ){
			foreach (Transform lChild in pParent ){
				lChild.gameObject.SetActive( pActive );
				SetActiveAllChildrenRecursive( lChild, pActive );
		}
	}//setActiveAllChildrenRecursive

	static public void DestroyAllChildren( Transform pParent ){
		foreach (Transform lChild in pParent) {
			GameObject.Destroy(lChild.gameObject);
		}
	}//DestroyAllChildren

	static public bool GetIsVisible( Transform pObject ){ return GetIsVisible( pObject.gameObject ); }
	static public bool GetIsVisible( GameObject pObject ){ return pObject.activeInHierarchy; }

	static public void SetActive( Draggable pObject, bool pActive = true ){if( pObject != null ){SetActive(pObject.gameObject,pActive );} }
	static public void SetActive( Transform pObject, bool pActive = true ){if( pObject != null ){SetActive(pObject.gameObject,pActive );} }
	static public void SetActive( Button pObject, bool pActive = true ){if( pObject != null ){SetActive(pObject.gameObject,pActive );} }
	static public void SetActive( Text pObject, bool pActive = true ){if( pObject != null ){SetActive(pObject.gameObject,pActive );} }
	static public void SetActive( GameObject pObject, bool pActive = true ){
		if( pObject != null ){
			if( pActive ){
				while( !pObject.activeInHierarchy ){
					pObject.SetActive( true );
					Transform lParent = pObject.transform.parent;
					if( lParent == null ){ break; }
					pObject = lParent.gameObject;
				}
			}else{
				pObject.SetActive( false );
			}
		}
	}//SetActiveForce

	static public long GetCurrentTimeStamp(){
		TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0,DateTimeKind.Utc));
		return (long) span.TotalSeconds;
	}

	private static bool mRandomIsInit = false;
	static public void InitRandom(){
		if( !Util.mRandomIsInit ){
			Util.mRandomIsInit = true;
			//Debug.Log( GetCurrentTimeStamp() );
			//Debug.Log( (int) GetCurrentTimeStamp() );
			UnityEngine.Random.InitState( (int) GetCurrentTimeStamp() );
		}
	}

	static public float GetRandom(){ return UnityEngine.Random.value; }
	static public int GetRandom( int pMaxValueIncluded ){ return (int) (GetRandom() * (pMaxValueIncluded+0.5f)); }

	static public void ShuffleArray( string [] pArray ){
		int lIndex = pArray.Length*2; float lFloatMax = (float)lIndex - 0.01f;
		while( lIndex > 0 ){ lIndex--;
			int lSrcIndex = ((int)( Util.GetRandom() * lFloatMax )) % pArray.Length;
			int lDstIndex = ((int)( Util.GetRandom() * lFloatMax )) % pArray.Length;
			string lTmp = pArray[lDstIndex]; pArray[lDstIndex] = pArray[lSrcIndex]; pArray[lSrcIndex] = lTmp;
		}
	}// ShuffleArray

	static public void ShuffleArray( int [] pArray ){
		int lIndex = pArray.Length*2; float lFloatMax = (float)lIndex - 0.01f;
		while( lIndex > 0 ){ lIndex--;
			int lSrcIndex = ((int)  ( Util.GetRandom() * lFloatMax )) % pArray.Length;
			int lDstIndex = ((int)  ( Util.GetRandom() * lFloatMax )) % pArray.Length;
			int lTmp = pArray[lDstIndex]; pArray[lDstIndex] = pArray[lSrcIndex]; pArray[lSrcIndex] = lTmp;
		}
	}// ShuffleArray

    static public void ShuffleArray<T>(T[] pArray)
    {
        int lIndex = pArray.Length * 2; float lFloatMax = (float)lIndex - 0.01f;
        while (lIndex > 0)
        {
            lIndex--;
            int lSrcIndex = ((int)(Util.GetRandom() * lFloatMax)) % pArray.Length;
            int lDstIndex = ((int)(Util.GetRandom() * lFloatMax)) % pArray.Length;
            T lTmp = pArray[lDstIndex]; pArray[lDstIndex] = pArray[lSrcIndex]; pArray[lSrcIndex] = lTmp;
        }
    }// ShuffleArray

    static public int [] GetIndexArray( int pIndexCount ){  // return array [0,1,2,3,4, ...., pIndexCount-1 ]
		int [] lIndexArray = new int[ pIndexCount ];
		for( int lIndex = 0; lIndex < pIndexCount; lIndex++ ){
			lIndexArray[ lIndex ] = lIndex;
		}
		return lIndexArray;
	}//GetIndexArray

	static public void CallBackEventTrigger( MonoBehaviour pThis,  EventTrigger.TriggerEvent pCallBack ){
       if( pCallBack != null ){
			BaseEventData lEventData = new BaseEventData( EventSystem.current );
			lEventData.selectedObject = pThis.gameObject;
			pCallBack.Invoke( lEventData );
		}
	}//CallBackEventTrigger

	static public void SetImage( Transform pDstTransform, Transform pSrcImageTransform ){
		SetImage( pDstTransform.gameObject.GetComponent<Image>(), pSrcImageTransform.gameObject.GetComponent<Image>() );
	}
	static public void SetImage( Transform pDstTransform, Image pSrcImage ){
		SetImage( pDstTransform.gameObject.GetComponent<Image>(), pSrcImage );
	}
	static public void SetImage( Image pDstImage, Image pSrcImage ){
		pDstImage.sprite = pSrcImage.sprite;
	}

	static public void SetPivot( Transform pTransform, float pX, float pY ){
		RectTransform lRectTransform = pTransform.GetComponent<RectTransform>();
		if( lRectTransform != null ){ lRectTransform.pivot = new Vector2( pX, pY );	}
	}//SetPivot

	static public void DebugArray( int [] pArray ){
		string lMessage = "";
		for( int lIndex = 0; lIndex < pArray.Length; lIndex ++ ){
			lMessage += pArray[ lIndex ] +",";
		}
		Debug.Log( pArray );
		Debug.Log( lMessage );
	}// DebugArray

	/*
	static public void PlayVideo( GameObject pObject, Anim_EndAnimCallBack pCallBack = null ) { // return Video duration
		Debug.Log( "PlayVideo");
		Debug.Log( pObject);
		if( pObject == null ){ return; }
		Util.SetActive( pObject );
		if( pObject.activeInHierarchy ){
			float lDuration = 0.0f;
			MovieTexture lVideoTexture = (MovieTexture)pObject.GetComponent<Renderer>().material.mainTexture;
			if( lVideoTexture != null ){
				lVideoTexture.Stop(); lVideoTexture.Play();
				lDuration = lVideoTexture.duration;
			}

			AudioSource lSound = pObject.GetComponent<AudioSource>();
			if( lSound != null ){
				lSound.Stop(); lSound.Play(); //Debug.Log( "have a audio, duration : " + lSound.clip.length + "  " + this.name );
			}

			Debug.Log( lDuration);
			Animate.Delay( lDuration, pCallBack );
		}//PlayVideo

		//return lDuration;
	}//playVideo


	static public void LoopVideo( GameObject pObject ) {  //todo merge with playvideo
		if( pObject == null ){ return; }
		Util.SetActive( pObject );
		if( pObject.activeInHierarchy ){
			float lDuration = 0.0f;
			MovieTexture lVideoTexture = (MovieTexture)pObject.GetComponent<Renderer>().material.mainTexture;
			if( lVideoTexture != null ){
				lVideoTexture.Stop(); lVideoTexture.loop = true; lVideoTexture.Play();
				lDuration = lVideoTexture.duration;
			}

			AudioSource lSound = pObject.GetComponent<AudioSource>();
			if( lSound != null ){
				lSound.Stop(); lSound.loop = true; lSound.Play(); //Debug.Log( "have a audio, duration : " + lSound.clip.length + "  " + this.name );
			}
		}//PlayVideo

		//return lDuration;
	}//playVideo
	*/

}//UtilManager
