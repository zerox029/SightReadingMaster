
// you have to add the AnimateManager in the scene

// Use Animate manager function in the editor Ui to fadein, fadeout,  etc...
// in code, use Animate.FadeIn( pObject, pDuration = default AnimatemangerDuration, pCallBack = null ) to fade in a object
// in code, use Animate.FadeOut( pObject ) to fade out a object
// in code, use Animate.Delay( pDurationFloatSecond, callback ()=>{}  )
// In code, use Animate.zoomIn, Animate.zoomOut, Animate.moveTo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void Anim_TransFormCallBack(float pT, List<Transform> pTransformList);
public delegate void Anim_CallBack(float pT, Transform pTransform);
public delegate void Anim_EndAnimCallBack();

public enum AnimateType { color, translation, localTranslation, rotation, scale };
public class AnimateManager : Singleton<AnimateManager>
{

    public float defaultFadeInDuration = 0.75f;
    public float defaultFadeOutDuration = 0.3f;

    public float defaultZoomInDuration = 0.75f;

    public float defaultZoomOutDuration = 0.3f;
    public float defaultMoveToDuration = 0.5f;

    public List<Animate> animateList = new List<Animate>();
    public void Update()
    {
        for (int lIndex = animateList.Count - 1; lIndex >= 0; lIndex--)
        {
            Animate lAnimate = animateList[lIndex];
            lAnimate.Update();
            if (!lAnimate.mIsPlaying) { animateList.RemoveAt(lIndex); }
        }
    }


    public void FadeIn(Transform pObject) { Animate.FadeIn(pObject); }
    public void FadeOut(Transform pObject) { Animate.FadeOut(pObject); }

    public void ZoomIn(Transform pObject) { Animate.ZoomIn(pObject); }
    public void ZoomOut(Transform pObject) { Animate.ZoomOut(pObject); }



    private Dictionary<string, Vector4> mOriginalData = new Dictionary<string, Vector4>();

    public Vector4 GetOriginalV4_Private(Object pObject, AnimateType pType) { bool lIsPresent; return GetOriginalValue_Util(pObject, pType, out lIsPresent); }
    public Vector3 GetOriginalV3_Private(Object pObject, AnimateType pType) { bool lIsPresent; Vector4 lReturn = GetOriginalValue_Util(pObject, pType, out lIsPresent); return (Vector3)lReturn; }

    private Vector4 GetOriginalValue_Util(Object pObject, AnimateType pType, out bool pIsPresent)
    {
        Vector4 lReturn = new Vector4(-1.0f, -1.0f, -1.0f, -1.0f);
        pIsPresent = mOriginalData.TryGetValue(pObject.GetInstanceID().ToString() + ":" + ((int)pType).ToString(), out lReturn);
        return lReturn;
    }//GetOriginalValue_Util

    public void RemoveOriginalValue_Util(Object pObject, AnimateType pType)
    {
        string lKey = pObject.GetInstanceID().ToString() + ":" + ((int)pType).ToString();
        if (mOriginalData.ContainsKey(lKey))
        {
            mOriginalData.Remove(lKey);
        }
    }//GetOriginalValue_Util

    //public void SetOriginalValue_Private( Object pObject, AnimType pType, float pData ){ SetOriginalValue_Private( pObject, pType, new Vector4( pData, 0.0f, 0.0f, 0.0f ) ); }
    public void SetOriginalValue_Private(Object pObject, AnimateType pType, Vector3 pData) { SetOriginalValue_Private(pObject, pType, (Vector4)pData); }
    public void SetOriginalValue_Private(Object pObject, AnimateType pType, Vector4 pData)
    {
        bool lIsPresent; GetOriginalValue_Util(pObject, pType, out lIsPresent);
        if (!lIsPresent)
        {
            mOriginalData.Add(pObject.GetInstanceID().ToString() + ":" + ((int)pType).ToString(), pData);
        }
    }//GetOriginalValue_Util

    //private List<>
    //private List<Vector3> animateList = new List<Animate>();
    //public void GetOriginalValue_Util( Transform pTransform );
    public void Delay(float pDuration, Anim_EndAnimCallBack pCallBack)
    {
        if (pCallBack != null)
        {
            StartCoroutine(Delay_Util(pDuration, pCallBack));
        }
    }

    private IEnumerator Delay_Util(float pDuration, Anim_EndAnimCallBack pCallBack)
    {
        yield return new WaitForSeconds(pDuration);
        pCallBack();
    }

}//AnimateManager

public class Animate
{
    public float duration = 0.5f;

    public Anim_EndAnimCallBack endCallBack = null;
    public Anim_TransFormCallBack transformCallBack = null;
    public Anim_CallBack animCallBack = null;

    public List<Transform> animGroup = new List<Transform>();
    public bool animAlpha = false;
    public float alphaBegin = 1.0f;
    public float alphaEnd = 0.0f;

    private bool mIsInit = false; // is init
    private List<CanvasGroup> canvasGroupToAnimList = new List<CanvasGroup>();

    private List<Material> materialToAnimList = new List<Material>();
    private List<Text> TextToAnimList = new List<Text>();

    private List<Image> ImageToAnimList = new List<Image>();

    public bool mIsPlaying = false;
    public bool mHideOnFadeOut = true;
    private float mBeginTime = 0;

    public Animate(Transform pTransform) { animGroup.Add(pTransform); }
    public Animate(List<Transform> pTransformList) { animGroup = pTransformList; }


    static public void Delay(float pDuration, Anim_EndAnimCallBack pCallBack) { AnimateManager.Instance.Delay(pDuration, pCallBack); }

    static public void FadeOut(Button pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeOut(pObject.transform, pDuration, pCallBack); }
    static public void FadeOut(GameObject pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeOut(pObject.transform, pDuration, pCallBack); }
    static public void FadeOut(Transform pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null)
    {

        if (pDuration < 0.0f) { pDuration = AnimateManager.Instance.defaultFadeOutDuration; }
        Animate lAnimate = new Animate(pObject); lAnimate.duration = pDuration; lAnimate.endCallBack = pCallBack;
        lAnimate.SetFadeOut();
        lAnimate.Init();
        lAnimate.Play();
    }//FadeOut

    static public void FadeIn(Button pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void FadeIn(GameObject pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void FadeIn(Transform pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null)
    {
        if (pDuration < 0.0f) { pDuration = AnimateManager.Instance.defaultFadeInDuration; }
        Util.SetActive(pObject, true);
        Animate lAnimate = new Animate(pObject); lAnimate.duration = pDuration; lAnimate.endCallBack = pCallBack; lAnimate.SetFadeIn(); lAnimate.Play();

    }//Fadein

    static public void FadeToward(Button pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null, float pAlphaBegin = 1.0f, float pAlphaEnd = 0.0f) { Animate.FadeToward(pObject.transform, pDuration, pCallBack, pAlphaBegin, pAlphaEnd); }
    static public void FadeToward(GameObject pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null, float pAlphaBegin = 1.0f, float pAlphaEnd = 0.0f) { Animate.FadeToward(pObject.transform, pDuration, pCallBack, pAlphaBegin, pAlphaEnd); }
    static public void FadeToward(Transform pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null, float pAlphaBegin = 1.0f, float pAlphaEnd = 0.0f)
    {
        if (pDuration < 0.0f) { pDuration = AnimateManager.Instance.defaultFadeOutDuration; }
        Animate lAnimate = new Animate(pObject); lAnimate.duration = pDuration; lAnimate.endCallBack = pCallBack;
        lAnimate.alphaBegin = pAlphaBegin;
        lAnimate.alphaEnd = pAlphaEnd;
        lAnimate.animAlpha = true;
        lAnimate.Init();
        lAnimate.Play();
    }//Fadein


    static public void ZoomIn(Button pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void ZoomIn(GameObject pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void ZoomIn(Transform pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null)
    {
        if (pDuration < 0.0f) { pDuration = AnimateManager.Instance.defaultZoomInDuration; }
        Util.SetActive(pObject, true);
        Animate lAnimate = new Animate(pObject); lAnimate.duration = pDuration; lAnimate.endCallBack = pCallBack;
        lAnimate.SetFadeIn();
        AnimateManager.Instance.SetOriginalValue_Private(pObject, AnimateType.scale, pObject.localScale);
        lAnimate.transformCallBack = (float pT, List<Transform> pTransformList) => {
            Vector3 lZeroScale = new Vector3(0.0f, 0.0f, 0.0f);
            foreach (Transform lTransform in pTransformList)
            {
                lTransform.localScale = Vector3.Lerp(lZeroScale, AnimateManager.Instance.GetOriginalV3_Private(lTransform, AnimateType.scale), pT);
            }
        };

        lAnimate.Play();

    }//ZoomIn

    static public void ZoomOut(Button pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void ZoomOut(GameObject pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void ZoomOut(Transform pObject, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null)
    {
        if (pDuration < 0.0f) { pDuration = AnimateManager.Instance.defaultZoomOutDuration; }

        //Util.SetActive( pObject, true );

        Animate lAnimate = new Animate(pObject); lAnimate.duration = pDuration; lAnimate.endCallBack = pCallBack;
        lAnimate.SetFadeOut();
        AnimateManager.Instance.SetOriginalValue_Private(pObject, AnimateType.scale, pObject.localScale);
        lAnimate.transformCallBack = (float pT, List<Transform> pTransformList) => {
            Vector3 lZeroScale = new Vector3(0.0f, 0.0f, 0.0f);
            foreach (Transform lTransform in pTransformList)
            {
                lTransform.localScale = Vector3.Lerp(AnimateManager.Instance.GetOriginalV3_Private(lTransform, AnimateType.scale), lZeroScale, pT);
            }
        };

        lAnimate.Play();

    }//ZoomOut

    static public void ScaleTo(Button pObject, float pDestinationScale, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void ScaleTo(GameObject pObject, float pDestinationScale, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void ScaleTo(Transform pObject, float pDestinationScale, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null)
    {
        if (pDuration < 0.0f) { pDuration = AnimateManager.Instance.defaultZoomInDuration; }
        Util.SetActive(pObject, true);
        Animate lAnimate = new Animate(pObject); lAnimate.duration = pDuration; lAnimate.endCallBack = pCallBack;
        AnimateManager.Instance.SetOriginalValue_Private(pObject, AnimateType.scale, pObject.localScale);
        lAnimate.transformCallBack = (float pT, List<Transform> pTransformList) => {
            Vector3 lDestinationScale = new Vector3(pDestinationScale, pDestinationScale, pDestinationScale);
            foreach (Transform lTransform in pTransformList)
            {
                lTransform.localScale = Vector3.Lerp(AnimateManager.Instance.GetOriginalV3_Private(lTransform, AnimateType.scale), lDestinationScale, pT);
            }
        };

        lAnimate.Play();

    }//ScaleTo

    static public void MoveTo(Button pObject, Vector3 pDestinationPosition, bool pFadeIn = false, bool pFadeOut = false, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void MoveTo(GameObject pObject, Vector3 pDestinationPosition, bool pFadeIn = false, bool pFadeOut = false, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }

    static public void MoveTo(Transform pObject, Vector3 pDestinationPosition, bool pFadeIn = false, bool pFadeOut = false, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null)
    {
        if (pDuration < 0.0f)
        {
            pDuration = AnimateManager.Instance.defaultMoveToDuration;
        }
        Animate lAnimate = new Animate(pObject);
        lAnimate.duration = pDuration;
        lAnimate.endCallBack = pCallBack;

        if (pFadeIn)
        {
            lAnimate.SetFadeIn();
            Util.SetActive(pObject, true);
        }
        if (pFadeOut)
        {
            lAnimate.SetFadeOut();
        }

        //Debug.Log(pObject.position);
        //Debug.Log(pObject.localPosition);
        //Debug.Log(pObject.GetComponent<RectTransform>().position);
        //Debug.Log(pObject.GetComponent<RectTransform>().anchoredPosition);

        AnimateManager.Instance.RemoveOriginalValue_Util(pObject, AnimateType.translation);

        AnimateManager.Instance.SetOriginalValue_Private(pObject, AnimateType.translation, pObject.position);
        lAnimate.transformCallBack = (float pT, List<Transform> pTransformList) =>
        {
            foreach (Transform lTransform in pTransformList)
            {
                Vector3 lOriginalPosition = AnimateManager.Instance.GetOriginalV3_Private(lTransform, AnimateType.translation);
                lTransform.position = Vector3.Lerp(lOriginalPosition, pDestinationPosition, pT);
            }
        };

        lAnimate.Play();
    }//MoveTo

    static public void MoveToLocal(Button pObject, Vector3 pDestinationPosition, bool pFadeIn = false, bool pFadeOut = false, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void MoveToLocal(GameObject pObject, Vector3 pDestinationPosition, bool pFadeIn = false, bool pFadeOut = false, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null) { Animate.FadeIn(pObject.transform, pDuration, pCallBack); }
    static public void MoveToLocal(Transform pObject, Vector3 pDestinationPosition, bool pFadeIn = false, bool pFadeOut = false, float pDuration = -1.0f, Anim_EndAnimCallBack pCallBack = null)
    {
        if (pDuration < 0.0f) { pDuration = AnimateManager.Instance.defaultMoveToDuration; }
        Animate lAnimate = new Animate(pObject); lAnimate.duration = pDuration; lAnimate.endCallBack = pCallBack;
        if (pFadeIn) { lAnimate.SetFadeIn(); Util.SetActive(pObject, true); }
        if (pFadeOut) { lAnimate.SetFadeOut(); }
        AnimateManager.Instance.SetOriginalValue_Private(pObject, AnimateType.localTranslation, pObject.localPosition);
        lAnimate.transformCallBack = (float pT, List<Transform> pTransformList) => {
            foreach (Transform lTransform in pTransformList)
            {
                Vector3 lOriginalPosition = AnimateManager.Instance.GetOriginalV3_Private(lTransform, AnimateType.localTranslation);
                lTransform.localPosition = Vector3.Lerp(lOriginalPosition, pDestinationPosition, pT);
            }
        };

        lAnimate.Play();
    }//MoveToLocal


    public void Play()
    {
        Init();
        SetFrame(0.0f);
        mBeginTime = Time.time;
        mIsPlaying = true;
        AnimateManager.Instance.animateList.Add(this);
    }


    private void Init()
    {
        if (!mIsInit)
        {
            mIsInit = true;
            if (animAlpha)
            {
                foreach (Transform lTransform in animGroup) { AddToFadeTransformRecursive(lTransform); }
            }
        }
    }// Start

    public void SetFadeIn() { alphaBegin = 0.0f; alphaEnd = 1.0f; animAlpha = true; }
    public void SetFadeOut() { alphaBegin = 1.0f; alphaEnd = 0.0f; animAlpha = true; }
    public void Update()
    {
        if (mIsPlaying)
        {
            float lDeltaTime = Time.time - mBeginTime;
            float lT = lDeltaTime / duration;
            if (lT > 1.0f) { lT = 1.0f; }

            SetFrame(lT);

            if (lDeltaTime > duration)
            {
                mIsPlaying = false; mIsInit = false;

                if (mHideOnFadeOut && animAlpha)
                {
                    if (alphaEnd <= 0.001f)
                    {
                        for (var lIndex = 0; lIndex < animGroup.Count; lIndex++)
                        {
                            Util.SetActive(animGroup[lIndex], false);
                        }//
                    }
                }

                if (endCallBack != null) { endCallBack(); }

            }
        }// have a progress to update
    }//update

    private void SetFrame(float pT)
    {
        if (animAlpha)
        {
            float lCurrentAlpha = alphaBegin + pT * (alphaEnd - alphaBegin);
            foreach (CanvasGroup lCanvasGroup in canvasGroupToAnimList)
            {
                Vector4 lOriginalColor = AnimateManager.Instance.GetOriginalV4_Private(lCanvasGroup, AnimateType.color);
                lCanvasGroup.alpha = lOriginalColor.w * lCurrentAlpha;
            }

            foreach (Material lMaterial in materialToAnimList)
            {
                Color lColor = lMaterial.color;
                Vector4 lOriginalColor = AnimateManager.Instance.GetOriginalV4_Private(lMaterial, AnimateType.color);
                lColor.a = lOriginalColor.w * lCurrentAlpha;
                lMaterial.color = lColor;
            }

            foreach (Text lText in TextToAnimList)
            {
                Color lColor = lText.color;
                Vector4 lOriginalColor = AnimateManager.Instance.GetOriginalV4_Private(lText, AnimateType.color);
                lColor.a = lOriginalColor.w * lCurrentAlpha;
                lText.color = lColor;
            }

            foreach (Image lImage in ImageToAnimList)
            {
                if (lImage != null)
                {
                    Color lColor = lImage.color;
                    Vector4 lOriginalColor = AnimateManager.Instance.GetOriginalV4_Private(lImage, AnimateType.color);
                    lColor.a = lOriginalColor.w * lCurrentAlpha;
                    lImage.color = lColor;
                }
            }
        }

        if (transformCallBack != null) { transformCallBack(pT, animGroup); }

        if (animCallBack != null)
        {
            foreach (Transform lTransform in animGroup)
            {
                animCallBack(pT, lTransform);
            }
        }

    }//SetFrame

    private void AddToFadeTransformRecursive(Transform pTransform, int pLevel = 0, bool pHaveCanvasGroup = false)
    {

        if (pTransform.gameObject.activeInHierarchy)
        {
            //AnimateManager.Instance.SetOriginalValue_Private( pObject, AnimateManager.AnimType.scaling, pObject.localScale );
            CanvasGroup lCanvasGroup = pTransform.GetComponent<CanvasGroup>();
            if (lCanvasGroup != null)
            { // Use canvas group to fade all UI element
                canvasGroupToAnimList.Add(lCanvasGroup);
                Vector4 lOriginalColor = new Vector4(0.0f, 0.0f, 0.0f, lCanvasGroup.alpha);
                AnimateManager.Instance.SetOriginalValue_Private(lCanvasGroup, AnimateType.color, lOriginalColor);
                pHaveCanvasGroup = true;
            }
            else
            {
                CanvasRenderer lCanvasRenderer = pTransform.GetComponent<CanvasRenderer>();
                if ((lCanvasRenderer == null) || (!pHaveCanvasGroup))
                { // It's not a UI element
                    bool lHaveSomething = false;
                    Renderer lRenderer = pTransform.GetComponent<Renderer>();

                    if (lRenderer != null)
                    {
                        Material lMaterial = lRenderer.material;
                        if (lMaterial != null)
                        {
                            materialToAnimList.Add(lMaterial); lHaveSomething = true;
                            AnimateManager.Instance.SetOriginalValue_Private(lMaterial, AnimateType.color, lMaterial.color);
                        }
                    }

                    if (!lHaveSomething)
                    {
                        Text lText = pTransform.GetComponent<Text>();
                        if (lText != null)
                        {
                            TextToAnimList.Add(lText);
                            AnimateManager.Instance.SetOriginalValue_Private(lText, AnimateType.color, lText.color);
                        }
                    }

                    if (!lHaveSomething)
                    {
                        Image lImage = pTransform.GetComponent<Image>();
                        if (lImage != null)
                        {
                            lHaveSomething = true;
                            ImageToAnimList.Add(lImage);
                            AnimateManager.Instance.SetOriginalValue_Private(lImage, AnimateType.color, lImage.color);
                        }
                    }

                }
            }// Is not a canvas group

            pLevel++;
            for (int lIndex = 0; lIndex < pTransform.childCount; lIndex++)
            {
                AddToFadeTransformRecursive(pTransform.GetChild(lIndex), pLevel, pHaveCanvasGroup);
            }


        }// Is active and visible
    }// AddToFadeTransformRecursive


}//AnimManager

