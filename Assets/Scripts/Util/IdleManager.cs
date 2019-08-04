    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

	using UnityEngine.EventSystems;
	using UnityEngine.Events;

    public class IdleManager : Singleton<IdleManager> {

		public float idleDelay = 25.0f;
        public Button triggerButton;

		public EventTrigger.TriggerEvent triggerCallBack;

		public List<GameObject> listDisableIdleIsVisible = new List<GameObject>();

		private float mLastActiveTime = -1f;

		private Vector3 mLastMousePosition;

		static public void IsActive() {
			if( IdleManager.Instance != null ){
				IdleManager.Instance.mLastActiveTime = -1f;
			}
		}
        void Update() {
			if( Input.mousePosition != mLastMousePosition ){
				mLastMousePosition = Input.mousePosition;
				mLastActiveTime = -1f;
			}else if( Input.GetButton("Fire1") ){
				mLastActiveTime = -1f;
			}

			foreach( GameObject lDisableIfVisible in listDisableIdleIsVisible ){
				 if( lDisableIfVisible.activeInHierarchy ){
					 mLastActiveTime = -1f;
				 }
			}

			if( mLastActiveTime < 0f ){
				mLastActiveTime = Time.fixedTime;
			}else{
				if( Time.fixedTime > (mLastActiveTime+idleDelay) ){
					Debug.Log( "Trigger Idle button !!" );
					mLastActiveTime = -1f;

					if( triggerButton != null ){
						triggerButton.onClick.Invoke();
					}

					if( triggerCallBack != null ){
						BaseEventData lEventData= new BaseEventData(EventSystem.current);
						lEventData.selectedObject = this.gameObject;
						triggerCallBack.Invoke(lEventData);
					}
				}
			}
        }// Update

    }// IdleManager
