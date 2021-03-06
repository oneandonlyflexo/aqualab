using UnityEngine;
using UnityEngine.UI;
using BeauRoutine;
using BeauRoutine.Extensions;
using TMPro;
using System.Collections;
using System;
using BeauUtil.Tags;
using BeauUtil;
using BeauUtil.Variants;

namespace ProtoAqua
{
    public class PartnerButton : BasePanel
    {
        static public readonly TableKeyPair RequestCounter = TableKeyPair.Parse("kevin:help.requests");

        #region Inspector

        [SerializeField] private Button m_Button = null;

        #endregion // Inspector

        [NonSerialized] private Routine m_ResponseRoutine;
        [NonSerialized] private BaseInputLayer m_InputLayer;

        protected override void Awake()
        {
            Services.Events.Register(GameEvents.CutsceneStart, OnCutsceneStart, this)
                .Register(GameEvents.CutsceneEnd, OnCutsceneEnd, this);
            
            m_Button.onClick.AddListener(OnButtonClicked);

            m_InputLayer = BaseInputLayer.Find(this);
            m_InputLayer.Device.OnUpdate += CheckInput;
        }

        private void OnDestroy()
        {
            Services.Events?.DeregisterAll(this);
            m_InputLayer.Device.OnUpdate -= CheckInput;
        }

        #region Handlers

        private void OnButtonClicked()
        {
            m_ResponseRoutine.Replace(this, ExecuteSequence()).TryManuallyUpdate(0);
        }

        private void OnCutsceneStart()
        {
            Hide();
        }

        private void OnCutsceneEnd()
        {
            Show();
        }

        private void CheckInput(DeviceInput inDevice)
        {
            if (inDevice.KeyPressed(KeyCode.Q) && m_RootGroup.blocksRaycasts)
                OnButtonClicked();
        }

        #endregion // Handlers

        #region Sequences

        private IEnumerator ExecuteSequence()
        {
            Services.Data.AddVariable(RequestCounter, 1);
            var sequence = Services.Script.TriggerResponse(GameTriggers.RequestPartnerHelp, "kevin");
            SetInputState(false);
            m_RootGroup.alpha = 0.75f;
            yield return sequence.Wait();
            SetInputState(IsShowing());
            m_RootGroup.alpha = 1;
        }

        #endregion // Sequences
    }
}