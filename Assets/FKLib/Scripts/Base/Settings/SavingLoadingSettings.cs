using System;
//============================================================
namespace FKLib
{
    public enum ENUM_SavingProvider
    {
        eSP_PlayerPrefs,
    }

    [Serializable]
    public class SavingLoadingSettings : ISettings
    {
        public override string Name
        {
            get { return "Saving & Loading"; }
        }

        public bool IsAutoSave = true;
        public string SavingKey = "Player";
        public float SavingRate = 60f;
        public ENUM_SavingProvider Provider;
    }
}
