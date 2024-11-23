using System.Linq;
using I2.Loc;
using UnityEngine;

public class NamesDatabase : ConfigurationAssetLoader
{
    public NamesConfiguration Configuration
    {
        get;
        private set;
    }

    public NamesDatabase() : base(GTAssetTypes.configuration_file, "NamesConfiguration")
    {
        this.Configuration = null;
    }

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (NamesConfiguration)scriptableObject;//JsonConverter.DeserializeObject<NamesConfiguration>(assetDataString);
    }

    public string GetName()
    {
        //if (this.Configuration.DisplayNames == null)
        //{
        //    return "unknown";
        //}
        //int count = this.Configuration.DisplayNames.Count;
        //if (count == 0)
        //{
        //    return "unknown";
        //}
        //string text = this.Configuration.DisplayNames[nameId % count];
        //if (this.Configuration.SecondNames != null)
        //{
        //    int count2 = this.Configuration.SecondNames.Count;
        //    if (count2 > 0)
        //    {
        //        int index = Random.Range(0, count2);
        //        text = text + " " + this.Configuration.SecondNames[index];
        //    }
        //}
        string text="unknown";
   
        if (LocalizationManager.CurrentLanguage.ToLower() == "english")
        {
            if (this.Configuration.EnglishNames == null || this.Configuration.EnglishNames.Count == 0)
            {
                text = "unknown";
            }
            else
            {
                text = Configuration.EnglishNames[Random.Range(0, this.Configuration.EnglishNames.Count)];
            }
        } else if (LocalizationManager.CurrentLanguage.ToLower() == "chinese")
        {
            if (this.Configuration.ChineseNames == null || this.Configuration.ChineseNames.Count == 0)
            {
                text = "-";
            }
            else
            {
                text = Configuration.ChineseNames[Random.Range(0, this.Configuration.ChineseNames.Count)];
            }
        }
        else
        {
            if (this.Configuration.PersianArabicNames == null || this.Configuration.PersianArabicNames.Count == 0)
            {
                text = "بی نام";
            }
            else
            {
                text = Configuration.PersianArabicNames[Random.Range(0, this.Configuration.PersianArabicNames.Count)];
            }
        }


        return text;
    }

    public string GetRandomName()
    {
        return this.GetName();
    }
}
