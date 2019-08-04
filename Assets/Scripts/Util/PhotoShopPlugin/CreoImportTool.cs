using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreoImportTool : MonoBehaviour
{
    public TextAsset jsonFile;
    public Transform rootParent;

    public int screenWidth = 0;
    public int screenHeight = 0;

    private JSONNode mJsonData;

    private static CreoImportTool_originalData[] mOriginalDataArray = null;

    private static void PrepareFindObject_Util()
    {
        CreoImportTool.mOriginalDataArray = Resources.FindObjectsOfTypeAll<CreoImportTool_originalData>();
    }

    /// <summary>
    /// Find a GameObject by name in a hierarchy of GameObjects
    /// If the hierarchy is not respected, is returned null as value
    /// </summary>
    /// <param name="parents">The hierarchy of parents - the order has importance</param>
    /// <param name="name">The name of searched GameObject</param>
    /// <returns></returns>
    public static GameObject FindObject(List<Transform> parents, string name, bool checkImg, string pFullPath)
    {
        GameObject rez = null;

        if (pFullPath.Length > 0)
        { // Find object with the full path first
          // have to call prepare findobject once before one or multiples findobject
            Debug.Log("Find  " + pFullPath + " search in : " + CreoImportTool.mOriginalDataArray.Length);

            foreach (CreoImportTool_originalData lToTest in CreoImportTool.mOriginalDataArray)
            {
                if (lToTest.originalImportPath == pFullPath)
                {
                    return lToTest.gameObject;
                }// have the same imported path
            }// pass all imported object
        }

        if (parents.Count == 0)
        {
            return null;
        }
        else if (parents.Count == 1)
        {
            Transform[] trs = parents[0].GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trs)
            {
                if (t.name == name)
                {
                    if (checkImg)
                    {
                        Image img = t.GetComponent<Image>();
                        if (img != null)
                            rez = t.gameObject;
                        else
                            rez = null;
                    }
                    else
                    {
                        Image img = t.GetComponent<Image>();
                        if (img == null)
                            rez = t.gameObject;
                        else
                            rez = null;
                    }
                }
            }
        }
        else if (parents.Count > 1)
        {
            Transform[] trs = parents[parents.Count - 1].GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trs)
            {
                if (t.name == parents[parents.Count - 2].name)
                {
                    List<Transform> newParents = new List<Transform>(parents);
                    newParents.RemoveAt(parents.Count - 1);
                    rez = FindObject(newParents, name, checkImg, pFullPath);
                }
            }
        }

        return rez;
    }

    /// <summary>
    /// The PUBLIC method which is called from outside - entry point
    /// </summary>
    public void ImportJson()
    {
#if UNITY_EDITOR
        if (AssetDatabase.IsValidFolder("Assets/Images"))
        {
            if ((jsonFile != null) && (rootParent != null))
            {
                Debug.Log(jsonFile.text);
                mJsonData = JSON.Parse(jsonFile.text);
                CreateObjsFromJson(rootParent, "Images");
            }
            else
            {
                Debug.LogWarning("Empty...");
            }
        }
        else
        {
            Debug.LogError("The folder with importing images not exists in 'Assets' of the project. Name of the folder should be: Assets/Images");
        }
#endif
    }

    /// <summary>
    /// Main method called to create all visual elements
    /// </summary>
    /// <param name="parent">Transform of the parent in hierarchy</param>
    private void CreateObjsFromJson(Transform parent, string pPath)
    {
        CreoImportTool.PrepareFindObject_Util();
        JSONNode lImageArray = mJsonData["Images"];
        CreateObjsFromJson(new List<Transform> { parent }, lImageArray, pPath);
    }

    /// <summary>
    /// Create the entire hierarchy of GameObjects from json File. Uses recursivity to create all visual elements
    /// </summary>
    /// <param name="parents">The list of the hierarchy Transforms</param>
    /// <param name="jsonObj">JSON nodes - the object which contains all information from json File</param>
    private void CreateObjsFromJson(List<Transform> parents, JSONNode jsonObj, string pPath)
    {
        Debug.Log("CreateObjsFromJson");

        foreach (KeyValuePair<string, JSONNode> lPair in jsonObj.AsObject)
        {
            // Debug.Log(lPair.Key + " *****************");
            // Debug.Log(lPair.Value.ToString());

            string lObjectFullPath = pPath + "." + lPair.Key;

            if (lPair.Value["src"] != null)
            {
                CreateObject(parents, lPair.Key, lPair.Value, lObjectFullPath);
            }
            else if (lPair.Value.GetType() == typeof(JSONArray))
            {
                JSONArray nodes = lPair.Value as JSONArray;

                foreach (JSONNode node in nodes)
                {
                    CreateObject(parents, lPair.Key, node, lObjectFullPath);
                }
            }
            else
            {
                // Check if we have a button
                if (lPair.Key.Contains(".btn"))
                {
                    GameObject buttonObject = null;
                    foreach (KeyValuePair<string, JSONNode> buttonState in lPair.Value.AsObject)
                    {
                        if (buttonState.Key.ToLower().Contains(".idle") || buttonState.Key.ToLower().Contains(".normal"))
                        {
                            buttonObject = CreateObject(parents, lPair.Key, buttonState.Value, lObjectFullPath);
                        }
                    }

                    if (buttonObject != null)
                    {
                        Button buttonComponent = buttonObject.AddComponent<Button>();

                        // Disable navigation of the button
                        Navigation buttonNavigation = new Navigation();
                        buttonNavigation.mode = Navigation.Mode.None;
                        buttonComponent.navigation = buttonNavigation;

                        if (lPair.Value.Count > 1)
                        {
                            buttonComponent.transition = Selectable.Transition.SpriteSwap;
                            SpriteState buttonSpriteState = new SpriteState();

                            foreach (KeyValuePair<string, JSONNode> buttonState in lPair.Value.AsObject)
                            {
                                if (buttonState.Key.ToLower().Contains(".over") || buttonState.Key.ToLower().Contains(".highlighted"))
                                {
                                    buttonSpriteState.highlightedSprite = ConvertToSprite(buttonState.Value["src"]);
                                }
                                else if (buttonState.Key.ToLower().Contains(".down") || buttonState.Key.ToLower().Contains(".pressed"))
                                {
                                    buttonSpriteState.pressedSprite = ConvertToSprite(buttonState.Value["src"]);
                                }
                                else if (buttonState.Key.ToLower().Contains(".inactif") || buttonState.Key.ToLower().Contains(".disabled"))
                                {
                                    buttonSpriteState.disabledSprite = ConvertToSprite(buttonState.Value["src"]);
                                }
                            }

                            buttonComponent.spriteState = buttonSpriteState;
                        }
                    }
                    else
                    {
                        Debug.Log("Cannot create button with name " + lPair.Key);
                    }
                }
                else
                {
                    // Search if exist a GameObject with this name
                    GameObject lParentGameObject = FindObject(new List<Transform>(parents), lPair.Key, false, lObjectFullPath);
                    if (lParentGameObject == null)
                    {
                        // Create a ParentGameObject in the hierarchy (without any components on it)
                        lParentGameObject = new GameObject(lPair.Key);
                        lParentGameObject.transform.SetParent(parents[0]);
                    }

                    this.SetOriginalPath_Util(lParentGameObject, lObjectFullPath);

                    RectTransform rectTransform = lParentGameObject.transform as RectTransform;
                    if (rectTransform == null)
                    {
                        rectTransform = lParentGameObject.AddComponent<RectTransform>();
                    }

                    rectTransform.sizeDelta = new Vector2(0, 0);

                    lParentGameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                    lParentGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

                    // Recursively call CreateObjsFromJson, but send as parents new hierarchy which includes new ParentGameObject
                    parents.Insert(0, lParentGameObject.transform);
                    CreateObjsFromJson(parents, lPair.Value, lObjectFullPath);
                    parents.RemoveAt(0);
                }
            }
        }
    }

    /// <summary>
    /// Add full path info on the object by creating a CreoImportTool_originalData if needed
    /// </summary>
    /// <param name="pObject">Game object to apply full path</param>
    /// <param name="pFullPath">Full path from json, like Images.background.sky</param>
	private void SetOriginalPath_Util(GameObject pObject, string pFullPath)
    {
        // Add original data info if needed
        CreoImportTool_originalData lCreoImportTool_originalData = pObject.GetComponent<CreoImportTool_originalData>();
        if (lCreoImportTool_originalData == null)
        {
            lCreoImportTool_originalData = pObject.AddComponent<CreoImportTool_originalData>();
            lCreoImportTool_originalData.originalImportPath = pFullPath;
        }
    }//SetOriginalPath_Util

    /// <summary>
    /// Create the GameObject if not exists and add 'Image' Component to it
    /// </summary>
    /// <param name="parents">the list of hierarchy where the GameObject need to pe created</param>
    /// <param name="objName">The name of the new GameObject</param>
    /// <param name="attributes">Contains all attributs of 'Image' Component</param>
    private GameObject CreateObject(List<Transform> parents, string objName, JSONNode attributes, string pFullPath)
    {
        if (attributes["idx"] != null)
        {
            objName += "." + attributes["idx"];
        }

        // Create the visual GameObject
        string lPath = attributes["src"];
        float lX = attributes["x"].AsFloat;
        float lY = attributes["y"].AsFloat;
        int lWidth = attributes["width"].AsInt;
        int lHeight = attributes["height"].AsInt;

        // Move the pivot of the image in the center of the GameObject
        lX += lWidth / 2;
        lY += lHeight / 2;
        lY *= -1;

        lX -= screenWidth / 2;
        lY += screenHeight / 2;

        // Search if exist a GameObject with an image with this name
        GameObject lGameObject = FindObject(new List<Transform>(parents), objName, true, pFullPath);
        if (lGameObject == null)
        {
            // Create new GameObject and sets the transform
            lGameObject = new GameObject(objName);
            lGameObject.transform.SetParent(parents[0]);
        }

        RectTransform rectTransform = lGameObject.transform as RectTransform;
        if (rectTransform == null)
        {
            rectTransform = lGameObject.AddComponent<RectTransform>();
        }

        lGameObject.transform.localPosition = new Vector3(lX, lY, 0f);
        lGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        rectTransform.sizeDelta = new Vector2(lWidth, lHeight);

        // Add Image component to the new GameObject
        Image pictureInScene = lGameObject.GetComponent<Image>();
        if (pictureInScene == null)
        {
            pictureInScene = lGameObject.AddComponent<Image>();
        }

        pictureInScene.sprite = ConvertToSprite(lPath);
        pictureInScene.sprite.name = objName;

        this.SetOriginalPath_Util(lGameObject, pFullPath);

        return lGameObject;
    }

    /// <summary>
    /// Convert an image from a given path to a 2D Sprite
    /// </summary>
    /// <param name="imagePath">Path of the image to be imprted to Unity</param>
    private Sprite ConvertToSprite(string imagePath)
    {
#if UNITY_EDITOR

        imagePath = "Assets/" + imagePath;
        TextureImporter tImporter = AssetImporter.GetAtPath(imagePath) as TextureImporter;

        if (tImporter.textureType != TextureImporterType.Sprite)
        {
            tImporter.textureType = TextureImporterType.Sprite;
            tImporter.textureCompression = TextureImporterCompression.Compressed;
            tImporter.SaveAndReimport();
            AssetDatabase.Refresh();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(imagePath);
#else
        return null;
#endif

    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(CreoImportTool))]
[CanEditMultipleObjects]
[ExecuteInEditMode]
class CreoImportToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(20);
        if (GUILayout.Button("Import/Reimport images from json"))
        {
            ImportToRootParent();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Clean root parent"))
        {
            CleanRootParent();
        }
    }

    private void CleanRootParent()
    {
        Debug.Log("CleanRootParent !!!!");
        CreoImportTool[] lJsonArray = Resources.FindObjectsOfTypeAll<CreoImportTool>();
        foreach (CreoImportTool lJson in lJsonArray)
        {
            for (var i = lJson.rootParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(lJson.rootParent.GetChild(i).gameObject, true);
            }
        }
    }

    private void ImportToRootParent()
    {
        Debug.Log("ImportToRootParent");
        CreoImportTool[] lJsonArray = Resources.FindObjectsOfTypeAll<CreoImportTool>();
        foreach (CreoImportTool lJson in lJsonArray)
        {
            lJson.ImportJson();
        }
    }
}
#endif