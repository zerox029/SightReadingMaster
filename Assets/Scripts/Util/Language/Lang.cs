//#define TEXDrawPlugin

using System.Collections;using System.Collections.Generic;using UnityEngine; using UnityEngine.UI;
using System.IO;using SimpleJSON;

#if TEXDrawPlugin
	using  TexDrawLib;
#endif

#if UNITY_EDITOR
	using UnityEditor;
	[AddComponentMenu("Language/Lang")]
	[ExecuteInEditMode]
#endif
// Use Lang.Get( "My_Text_Id" ) , will return string for the current language, will return empty string if not found
// Use Lang.Get( "My_Text_Id", "en" )   , will return string for english language

// Use Lang.GetDebug( "My_Text_Id"  ), will return string "id[lang] is empty" if not found ( to display something... )

// Use Lang.Exist( "My_Text_Id" ) to know if this id is defined in the current language
// Use Lang.Exist( "My_Text_Id", "en" ) to know if this id is defined in englsih

// Use Lang.Toggle( pUser = "Default"  ) to switch from french to english and vice versa
// Use Lang.GetLang( pUser = "Default" ) to get the current language
// Use Lang.SetLang("fr", pUser = "Default") to get the current language to french

public class Lang : MonoBehaviour{

	public string textId;
	public string forceLang;
	public bool forceCapitilize;

	public string user; // If text is for a specific user

	static public string GetNbsp(){ return "\u00A0"; }
	static private string ApplyFilter( string pString, string pLanguage ){

		pString = pString.Replace( " ?", "?" );
		pString = pString.Replace( " !", "!" );
		pString = pString.Replace( " :", ":" );
		pString = pString.Replace( ": ", ":" );
		pString = pString.Replace( "« ", "«" );
		pString = pString.Replace( " »", "»" );



		if( pLanguage == "fr" ){
			pString = pString.Replace( "?", GetNbsp()+"?" );
			pString = pString.Replace( "!", GetNbsp()+"!" );
			pString = pString.Replace( ":", GetNbsp()+":"+GetNbsp() );
			pString = pString.Replace( "«", "«"+GetNbsp() );
			pString = pString.Replace( "»", GetNbsp()+"»" );
		}

		pString = pString.Replace( "%urlSeparator%", "://" );

		return pString;
	}

	public void SetTextId( string pId){
		this.textId = pId;
		this.UpdateUiString(pUseEmptyString: true);
	}//Get
	static public string Get( string pId, string pLanguage = "" ){
		if( pLanguage == "" ){ pLanguage = LangManager.Instance.GetCurrentLanguage(); }
		JSONNode lNode = GetNode( pId, pLanguage );
		if( lNode != null ){ return ApplyFilter( lNode.Value, pLanguage ); }
		return "";
	}//Get

	static public string GetDebug( string pId, string pLanguage = "" ){
		if( pLanguage == "" ){ pLanguage = LangManager.Instance.GetCurrentLanguage(); }
		JSONNode lNode = GetNode( pId, pLanguage );
		if( lNode != null ){ if( lNode.Value.Length > 0 ){return lNode.Value;} }
		if( pLanguage == "fr"){ return pId+"["+pLanguage+"] est vide" ; }
		return pId+"["+pLanguage+"] is empty" ;
	}//GetDebug
	static public bool Exist( string pId, string pLanguage = "" ){
		if( pLanguage == "" ){ pLanguage = LangManager.Instance.GetCurrentLanguage(); }
		JSONNode lNode = GetNode( pId, pLanguage );
		if( lNode != null ){ return true; }
		return false;
	}//Exist

	static public void Toggle( string pUser = "Default" ){ LangManager.Instance.Toggle( pUser );}//Toggle

	static public string  GetLang( string pUser = "Default" ){ return LangManager.Instance.GetCurrentLanguage( pUser ); }
	static public void  SetLang( string pLang, string pUser = "Default" ){ LangManager.Instance.SetCurrentLanguage( pLang, pUser ); }

	static private JSONNode GetNode( string pId, string pLanguage = "" ){
		JSONNode lNode = LangManager.Instance.GetNode( pId, pLanguage );
		return lNode;
	}//Get

	void OnEnable() {
        UpdateUiString();
    }

	public void UpdateUiString( bool pInPlayMode = true, string pLang = "", bool pUseEmptyString = false ){

		if( (textId != null) && ( textId.Length > 0 ) || (pUseEmptyString)) {

			if( ( pLang == null ) || ( pLang.Length == 0 )){
				pLang = LangManager.Instance.GetCurrentLanguage( user );
			}

			if( forceLang.Length > 0 ){ pLang = forceLang; }
			string lNewText = Get( textId, pLang );

			string lFontSizeString = Get( textId, "fontSize" );
			string lFontFamily = Get( textId, "fontFamily" );
			#if TEXDrawPlugin
				string lFontFamilyBold = Get( textId, "fontFamilyBold" );
				string lFontFamilyItalic = Get( textId, "fontFamilyItalic" );
			#endif

			int lFontSize = 0;
			if( lFontSizeString.Length > 0 ){ lFontSize = int.Parse( lFontSizeString ); }

			if( forceCapitilize ){ lNewText = lNewText.ToUpper(); }


			Text lText = GetComponent<Text>();
			if( lText != null ){
				if( lText.text != lNewText ){lText.text = lNewText;}
				if( ( lFontSize > 0 ) && ( lFontSize != lText.fontSize ) ){ lText.fontSize = lFontSize;	}

				if( lFontFamily.Length > 0 ){
					Font lFont = LangManager.Instance.GetFontByShortName( lFontFamily );
					if( (lFont != null) && (lText.font != lFont) ){	lText.font = lFont;	}
				}// Have a font

			}
			else{
				#if TEXDrawPlugin
				TEXDraw lTexDraw = GetComponent<TEXDraw>();
				if( lTexDraw != null ){
					lNewText = GetPluginTxt( lNewText, lFontFamily, lFontFamilyBold, lFontFamilyItalic );
					if( lTexDraw.text != lNewText ){ lTexDraw.text = lNewText; }
					if( ( lFontSize > 0 ) && ( lFontSize != lTexDraw.size ) ){ lTexDraw.size = lFontSize; }
				}// support texDraw plugin
				#endif
			}

			//\gothambold{petit}

			#if UNITY_EDITOR
				if( !pInPlayMode ){ EditorUtility.SetDirty( this ); }
			#endif
		}
	}

	public string GetPluginTxt( string pString, string pDefaultFont, string pBoldFont, string pItalicFont ){

		if( pString.Length == 0 ){ return ""; }

		ParseHtml.ParseReturn lParseArray = ParseHtml.Parse( pString );

		string lDefaultFontTag = "\\"+ pDefaultFont +" ";
		string lReturnString = lDefaultFontTag;

		for( int lIndex = 0; lIndex < lParseArray.textArray.Length; lIndex ++ ){
				string lTag = lParseArray.tagArray[ lIndex ];
				string lText = lParseArray.textArray[ lIndex ];

				if( ( lTag == "<b>") || ( lTag == "<B>") ) {
					lText= lText.Replace( "\n", "\n"+ "\\"+pBoldFont+" " );
					lReturnString +=  "\\"+pBoldFont+"{"+lText+"}";
				}
				else if( ( lTag == "<i>") || ( lTag == "<I>") ) {
					lText= lText.Replace( "\n", "\n"+ "\\"+pItalicFont+" " );
					lReturnString +=  "\\"+pItalicFont+"{"+lText+"}";
				}
				else { lReturnString += lText.Replace( "\n", "\n"+lDefaultFontTag ); }
		}

		return lReturnString;
	}//GetPluginTxt

}//Lang

#if UNITY_EDITOR

	[CustomEditor(typeof(Lang))]
	[CanEditMultipleObjects]
	class LangEditor : Editor {


		private void UpdateUIString( ){
				LangManager.Instance.LoadFont();
				LangManager.Instance.ReloadJson();
				LangManager.Instance.UpdateUiString( false );
		}
		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			if(GUILayout.Button("Set text in editor from json")){
				UpdateUIString();
			}

			#if TEXDrawPlugin

				if(GUILayout.Button("Convert text to TEXDraw, can't undo !!")){
					LangManager.Instance.ForAllLang(
							( Lang pLang )=>{

								Text lText = pLang.GetComponent<Text>();
								if( lText != null ){
									string lTxtContent = lText.text;
									int lFontSize = lText.fontSize;
									Color lColor = lText.color;
									bool lRayCast = lText.raycastTarget;
									TextAnchor lAlign = lText.alignment;
									Vector2 lVectorAlign = Text.GetTextAnchorPivot( lAlign );

									GameObject lGameObject = pLang.GetComponent<Component>().gameObject;

									DestroyImmediate( lText );

									lGameObject.AddComponent<TEXDraw>();
									TEXDraw lTexDraw = pLang.GetComponent<TEXDraw>();

									lTexDraw.text = lTxtContent;
									lTexDraw.size = lFontSize;
									lTexDraw.color = lColor;
									lTexDraw.alignment = lVectorAlign;
									lTexDraw.autoFit = TexDrawLib.Fitting.Off;
									lTexDraw.autoWrap = TexDrawLib.Wrapping.WordWrap;
									lTexDraw.raycastTarget = lRayCast;

									/*Debug.Log( pLang.name + " font size : " + lFontSize + " color : ( "+ lColor.r+","+ lColor.g +","+ lColor.b +","+ lColor.a +")" );
									Debug.Log( lTxtContent );*/
									//Debug.Log( lAlign );
									//Debug.Log( lVectorAlign );
								}else{
									//TEXDraw lTexDraw = pLang.GetComponent<TEXDraw>();

									//Debug.Log( lTexDraw.alignment );

								}
							}
					  );
					UpdateUIString();
				}
			#endif
		}
	}
#endif


class ParseHtml{


	public struct ParseReturn{
		public string [] textArray;
		public string [] tagArray;
	 }
	static public ParseReturn  Parse( string pString ){
		//let lReturn = cOriginalText( cPropsWithStyle( this.props, { fontWeight:'bold' } ) );

			string lText = pString;

			List<string> lTextUiArray = new List<string>();
			List<string> lTagArray = new List<string>();

			//= new string[3 ]{ "Small", "MaisSmall", "PlantMaisFinalSmall" }

			string [] lBeginTagArray = new string[] { "<b>", "<B>", "<i>", "<I>" };
			string [] lEndTagArray = new string[]{ "</b>", "</B>", "</i>", "</I>" };
			//let lTagCreatorArray = [ cStrong, cItalic, cA ];
			//string [] lTagAddParamArray = new string[]{ "", "", "", "" };

			int [] lNextBeginIndexTagArray = new int[] { -1, -1, -1, -1 };
			int [] lNextEndIndexTagArray = new int [] { -1, -1, -1, -1 };

			//int lKey = 0;

			int lCurrentPosition = 0;

			// Compute all next index
			for( int lTagIndex = 0;  lTagIndex < lBeginTagArray.Length; lTagIndex++ ){
				lNextBeginIndexTagArray[ lTagIndex ] = lText.IndexOf( lBeginTagArray[ lTagIndex ], lCurrentPosition );
				lNextEndIndexTagArray[ lTagIndex ] = lText.IndexOf( lEndTagArray[ lTagIndex ], lCurrentPosition );
			}

			while( lCurrentPosition < lText.Length ){
				int lNextTagIndex = -1;
				int lMinTagPosition = lText.Length;

				for( int lTagIndex = 0; lTagIndex < lBeginTagArray.Length; lTagIndex++ ){
					if( ( lNextBeginIndexTagArray[ lTagIndex ] >= 0 ) && ( lNextBeginIndexTagArray[ lTagIndex ] < lMinTagPosition )  ){
						lMinTagPosition  = lNextBeginIndexTagArray[ lTagIndex ];
						lNextTagIndex = lTagIndex;
					}
				}// Find next closest tag

				string lBeforeTagText = lText.Substring( lCurrentPosition, lMinTagPosition - lCurrentPosition );

				if( lBeforeTagText.Length > 0 ){
					lTextUiArray.Add( lBeforeTagText );
					lTagArray.Add("");
					lCurrentPosition = lMinTagPosition;
				}

				 if( lNextTagIndex >=0 ){ // have a tag

					/*let lTagParam = {};

					if( isObject( lTagAddParamArray[ lNextTagIndex] ) ){
						lTagParam = cProps( lTagParam, lTagAddParamArray[ lNextTagIndex] );
					}*/

					int lBeginTextIndex = lNextBeginIndexTagArray[lNextTagIndex] + lBeginTagArray[ lNextTagIndex ].Length;
					int lEndTextIndex = lText.Length;
					int lNextCurrentPosition = lEndTextIndex;

					/*
					if( lBeginTagArray[ lNextTagIndex ].slice(-1) !== ">" ){
						let lCloseTagIndex = lText.indexOf( ">", lBeginTextIndex );
						if( lCloseTagIndex >= 0 ){ // find tag closure

							let lTagParamTxt = lText.substr( lBeginTextIndex, lCloseTagIndex - lBeginTextIndex );

							let lParamArray = lTagParamTxt.split(' ');

							for( let lParamIndex = 0; lParamIndex < lParamArray.length; lParamIndex++ ){
								let lOneParamFullLine = lParamArray[ lParamIndex ];
								let lParamNameEndIndex = lOneParamFullLine.indexOf('=');
								if( lParamNameEndIndex > 0 ){
									let lParamName = lOneParamFullLine.substr( 0, lParamNameEndIndex ).trim();
									if( lParamName.length > 0 ){
										let lParamContent = lOneParamFullLine.substr( lParamNameEndIndex + 1 ).trim() ; // remove =
										lParamContent = lParamContent.substr( 1, lParamContent.length - 2 ); // remove begin and last "
										if( lParamContent.length > 0 ){
											lTagParam[ lParamName ] = lParamContent;
										}
									}
								}
							}

							lBeginTextIndex = lCloseTagIndex + 1;
						}
					}// It's a open tag like <a,  must find the end of the tag and prepare the parameter
					 // tag inside parameter, like href="http://google.com"  in <a href="http://google.com">google</a>
					*/

					if( lNextEndIndexTagArray[ lNextTagIndex ] >= 0 ){
						lEndTextIndex = lNextEndIndexTagArray[ lNextTagIndex ];
						 lNextCurrentPosition = lEndTextIndex + lEndTagArray[ lNextTagIndex ].Length;;
					}

					string lInTagText = lText.Substring( lBeginTextIndex, lEndTextIndex - lBeginTextIndex );

					lTextUiArray.Add( /*lTagCreatorArray[ lNextTagIndex ]( cProps( lTagParam, { key: ++lKey  } ),*/ lInTagText  );
					lTagArray.Add( lBeginTagArray[ lNextTagIndex ] );

					lCurrentPosition = lNextCurrentPosition;

					lNextBeginIndexTagArray[ lNextTagIndex ] = lText.IndexOf( lBeginTagArray[ lNextTagIndex ], lCurrentPosition );
					lNextEndIndexTagArray[ lNextTagIndex ] = lText.IndexOf( lEndTagArray[ lNextTagIndex ], lCurrentPosition );
					}

			}// Keep searching for tag

			//lReturn = cText( cProps( this.props, {} ) /*{ style: this.props.style}*/ , lTextUiArray ); // just to fix warning this.props.style

		/*}// Have a text html
		else{
			lReturn = cText( this.props, this.props.children );
		}

		return lReturn;*/


		ParseReturn lReturn;
		lReturn.textArray = lTextUiArray.ToArray();
		lReturn.tagArray = lTagArray.ToArray();

		return lReturn;
	}//ParseHtml
}// ParseHtml
