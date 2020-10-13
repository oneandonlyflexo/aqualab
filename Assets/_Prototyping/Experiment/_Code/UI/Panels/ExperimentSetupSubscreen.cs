using UnityEngine;
using BeauRoutine.Extensions;

namespace ProtoAqua.Experiment
{
    public class ExperimentSetupSubscreen : BasePanel
    {
        #region Inspector

        [Header("Subscreen")]
        [SerializeField] private string m_ScreenId = null;

        #endregion // Inspector

        #region BasePanel

        protected override void OnShow(bool inbInstant)
        {
            if (!WasShowing())
            {
                Services.Data.SetVariable(ExperimentVars.SetupPanelScreen, m_ScreenId);
            }
        }

        #endregion // BasePanel
    }
}