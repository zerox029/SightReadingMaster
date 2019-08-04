
using System.Collections;using System.Collections.Generic;using UnityEngine; using UnityEngine.UI;
using System.IO;

using SimpleJSON;



// check Lang.cs

// Use Lang.Get( "My_Text_Id" )   , will return string for the current language, will return empty string if not found
// Use Lang.Get( "My_Text_Id", "en" )   , will return string for english language

// Use Lang.Exist( "My_Text_Id" ) to know if this id is defined in the current language
// Use Lang.Exist( "My_Text_Id", "en" ) to know if this id is defined in englsih

// Use Lang.Toggle( pUser = "Default"  ) to switch from french to english and vice versa
// Use Lang.GetLang( pUser = "Default" ) to get the current language
// Use Lang.SetLang("fr", pUser = "Default") to get the current language to french

#if UNITY_EDITOR
	using UnityEditor;
	[ExecuteInEditMode]
#endif

public delegate void LangManagerLoopCallBack( Lang pLang );

public class LangManager : Singleton<LangManager> {

	public TextAsset jsonFile;
	public string defaultLanguage = "fr";

	Dictionary<string,string> langByUserDictionary = new Dictionary<string,string>();

	private JSONNode mJsonData;

	private bool isInit = false;

	bool useFont = true;
	Font [] fontArray = null;
	Dictionary<string,int> indexByFontNameArray = new Dictionary<string,int>();

	public void ReloadJson( ){ isInit = false; Start(); }

	public string GetIsInit( ){ if( isInit ){ return mJsonData.Count.ToString() + "Ii"; } return "Is not init" ;} //Debug Stuff
	public JSONNode GetNode( string pId, string pLanguage = "" ){
		if( !isInit ){ Start(); }
		JSONNode lNode = null;
		if( mJsonData != null ){
			lNode = LangManager.Instance.mJsonData[ pId ];
			if( lNode != null ){ lNode = lNode[ pLanguage ];}
		}
		return lNode;
	}//GetNode

	//public string jsonString;
	void Start(){
		if( !isInit ){
			isInit = true;

		 	//string lJsonString = File.ReadAllText("Assets/Language/lang.json" );
			mJsonData = JSON.Parse( jsonFile.text );
			LoadFont();
		 	UpdateUiString();
		}
	}

	public string GetCurrentLanguage( string pUser = "Default" ){
		if( (pUser == null) || (pUser.Length == 0) ){ pUser = "Default"; }
		string lLanguage;

		bool lIsPresent = langByUserDictionary.TryGetValue( pUser, out lLanguage );
		if( lIsPresent ){ return lLanguage; }

		return defaultLanguage;
	}//GetCurrentLanguage

	public void SetLangEnglish( string pUser = "Default" ){ SetCurrentLanguage( "en", pUser ); }
	public void SetLangFrench( string pUser = "Default" ){ SetCurrentLanguage( "fr", pUser ); }

	public void SetCurrentLanguage( string pLang ){ SetCurrentLanguage( pLang, "Default" ); } // Back compatibilty fix
	public void SetCurrentLanguage( string pLang, string pUser = "Default" ){
		if( (pUser == null) || (pUser.Length == 0) ){ pUser = "Default"; }

		string lLanguage;
		bool lIsPresent = langByUserDictionary.TryGetValue( pUser, out lLanguage );

		if( lIsPresent ){
			langByUserDictionary[ pUser ] = pLang;
		}else{
			langByUserDictionary.Add( pUser, pLang );
		}
		UpdateUiString();
	}
	public void Toggle( string pUser = "Default" ){
		if( GetCurrentLanguage( pUser ) == "en" ){SetCurrentLanguage("fr", pUser );}else{SetCurrentLanguage("en", pUser );}
	}//Toggle

	public void UpdateUiString( bool pInPlayMode = true ){
		Lang[] lLangArray = Resources.FindObjectsOfTypeAll <Lang>( );
		foreach( Lang lLang in lLangArray ){
			lLang.UpdateUiString( pInPlayMode );
		}
	}//UpdateUiString

	public void ForAllLang( LangManagerLoopCallBack pCallBack ){
		Lang[] lLangArray = Resources.FindObjectsOfTypeAll<Lang>( );
		foreach( Lang lLang in lLangArray ){
			pCallBack( lLang );
		}
	}//ForAllLang

	public void LoadFont( ){
		if( !useFont ){ return; }

		//Debug.Log( "Load font !" );
		#if UNITY_EDITOR
			// force reload in editor
			fontArray = null;
			indexByFontNameArray.Clear();
		#endif

		if( fontArray == null ){
			List<Font> lFontList = new List<Font>();
			//List<string> lFontStringList = new List<string>();
			Object[] lObjectArray = Resources.LoadAll("Fonts", typeof( Font ));
			//Debug.Log( lObjectArray.Length );

			if( ( lObjectArray != null ) && ( lObjectArray.Length > 0) ){
				for( int lIndex = 0; lIndex < lObjectArray.Length; lIndex++ ){
					Font lFont = lObjectArray[ lIndex ] as Font;
					if(  lFont != null ){
						lFontList.Add( lFont );
						string lShortName = lFont.name.ToLower().Replace( "-","" );
						indexByFontNameArray.Add( lShortName, lFontList.Count - 1 );
						//Debug.Log( lShortName );
					}
				}
			}

			fontArray = lFontList.ToArray();
			//Debug.Log( "Loaded font : " + fontArray.Length );
		}
	}// LoadFont

	public Font GetFontByShortName( string pFontShortName ){
		if( !useFont ){ return null; }
		Font lFontReturn = null;
		int lFontIndex = -1;
		bool lIsPresent = indexByFontNameArray.TryGetValue( pFontShortName, out lFontIndex );
		if( lIsPresent && (lFontIndex >= 0 ) && ( lFontIndex < fontArray.Length ) ){
			lFontReturn = fontArray[ lFontIndex ];
		}
		return lFontReturn;
	}//GetFontByShortName

}//LangManager

#if UNITY_EDITOR

[CustomEditor(typeof(LangManager))]
 class LangManagerEditor : Editor {
   public override void OnInspectorGUI() {
        	DrawDefaultInspector();
			if(GUILayout.Button("Set text in editor from json")){
				//if( LangManager.Instance == null ){ LangManager lCreated =new LangManager(); }

				//Debug.Log( LangManager.Instance );
				LangManager.Instance.ReloadJson();
				LangManager.Instance.UpdateUiString( false );
			}
   }
 }
 #endif
