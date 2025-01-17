using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class IpApiData
{
    public string country_name;

    public static IpApiData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<IpApiData>(jsonString);
    }
}


public class SystemLanguageDetect : MonoBehaviour
{
    public static SystemLanguageDetect Instance;

    public string CurrentCountry;

    public void Awake() 
    {
        if(Instance == null)
            Instance = this;

        CurrentCountry = Get2LetterISOCodeFromSystemLanguage(); 
    }

	public string Get2LetterISOCodeFromSystemLanguage() {
		SystemLanguage lang = Application.systemLanguage;
		string res = "EN";
		switch (lang) {
			case SystemLanguage.Afrikaans: res = "AF"; break;
			case SystemLanguage.Arabic: res = "AR"; break;
			case SystemLanguage.Basque: res = "EU"; break;
			case SystemLanguage.Belarusian: res = "BY"; break;
			case SystemLanguage.Bulgarian: res = "BG"; break;
			case SystemLanguage.Catalan: res = "CA"; break;
			case SystemLanguage.Chinese: res = "ZH"; break;
			case SystemLanguage.Czech: res = "CS"; break;
			case SystemLanguage.Danish: res = "DA"; break;
			case SystemLanguage.Dutch: res = "NL"; break;
			case SystemLanguage.English: res = "EN"; break;
			case SystemLanguage.Estonian: res = "ET"; break;
			case SystemLanguage.Faroese: res = "FO"; break;
			case SystemLanguage.Finnish: res = "FI"; break;
			case SystemLanguage.French: res = "FR"; break;
			case SystemLanguage.German: res = "DE"; break;
			case SystemLanguage.Greek: res = "EL"; break;
			case SystemLanguage.Hebrew: res = "IW"; break;
			case SystemLanguage.Icelandic: res = "IS"; break;
			case SystemLanguage.Indonesian: res = "IN"; break;
			case SystemLanguage.Italian: res = "IT"; break;
			case SystemLanguage.Japanese: res = "JA"; break;
			case SystemLanguage.Korean: res = "KO"; break;
			case SystemLanguage.Latvian: res = "LV"; break;
			case SystemLanguage.Lithuanian: res = "LT"; break;
			case SystemLanguage.Norwegian: res = "NO"; break;
			case SystemLanguage.Polish: res = "PL"; break;
			case SystemLanguage.Portuguese: res = "PT"; break;
			case SystemLanguage.Romanian: res = "RO"; break;
			case SystemLanguage.Russian: res = "RU"; break;
			case SystemLanguage.SerboCroatian: res = "SH"; break;
			case SystemLanguage.Slovak: res = "SK"; break;
			case SystemLanguage.Slovenian: res = "SL"; break;
			case SystemLanguage.Spanish: res = "ES"; break;
			case SystemLanguage.Swedish: res = "SV"; break;
			case SystemLanguage.Thai: res = "TH"; break;
			case SystemLanguage.Turkish: res = "TR"; break;
			case SystemLanguage.Ukrainian: res = "UK"; break;
			case SystemLanguage.Unknown: res = "EN"; break;
			case SystemLanguage.Vietnamese: res = "VI"; break;
		}
		return res;
	
}
}

