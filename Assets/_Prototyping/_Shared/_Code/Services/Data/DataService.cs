using System;
using System.Collections;
using System.Collections.Generic;
using BeauData;
using BeauPools;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Tags;
using BeauUtil.Variants;
using ProtoAqua.Profile;
using UnityEngine;

namespace ProtoAqua
{
    public partial class DataService : ServiceBehaviour
    {
        static private readonly string DebugUserDataPrefsKey = "_debugUserProfile";

        #region Inspector

        [SerializeField] private string m_DefaultPlayerDisplayName = "Unknown Player";

        [Header("-- DEBUG --")]
        [SerializeField] private string m_DebugUrl = string.Empty;

        #endregion // Inspector

        [NonSerialized] private SaveData m_CurrentSaveData = new SaveData();
        [NonSerialized] private QueryParams m_QueryParams;

        [NonSerialized] private CustomVariantResolver m_VariableResolver;

        #region Query Params

        public QueryParams PeekQueryParams()
        {
            return m_QueryParams;
        }

        public QueryParams PopQueryParams()
        {
            QueryParams stored = m_QueryParams;
            m_QueryParams = null;
            return stored;
        }

        private void RetrieveQueryParams()
        {
            string url;
            #if UNITY_EDITOR
            url = m_DebugUrl;
            #else
            url = Application.absoluteURL;
            #endif // UNITY_EDITOR

            m_QueryParams = new QueryParams();
            m_QueryParams.TryParse(url);
        }

        #endregion // Query Params

        #region Save Data

        public SaveData Profile
        {
            get { return m_CurrentSaveData; }
        }

        public string CurrentCharacterName()
        {
            return m_CurrentSaveData?.Character?.DisplayName ?? m_DefaultPlayerDisplayName;
        }

        public Pronouns CurrentCharacterPronouns()
        {
            CharacterProfile profile = m_CurrentSaveData?.Character;
            return profile != null ? profile.Pronouns : Pronouns.Neutral;
        }

        #endregion // Save Data

        #region IService

        protected override void OnRegisterService()
        {
            InitVariableResolver();

            SaveData fromPrefs = null;
            if (Serializer.ReadPrefs(ref fromPrefs, DebugUserDataPrefsKey))
            {
                m_CurrentSaveData = fromPrefs;
            }
            RetrieveQueryParams();

            HookSaveDataToVariableResolver(m_CurrentSaveData);
        }

        protected override void OnDeregisterService()
        {
            Serializer.WritePrefs(m_CurrentSaveData, DebugUserDataPrefsKey, OutputOptions.None, Serializer.Format.JSON);
            PlayerPrefs.Save();
            m_QueryParams = null;
        }

        public override FourCC ServiceId()
        {
            return ServiceIds.Data;
        }

        #endregion // IService
    }
}