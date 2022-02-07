//using UnityEngine;
//using UltimateSurvival.StandardAssets;

//namespace UltimateSurvival
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class PlayerPauseHandler : MonoBehaviour
//    {

//        #region Data
//#pragma warning disable 0649

//        [SerializeField] private DOF m_DOF;

//        [SerializeField] private ColorCorrection m_ColorCorrectionCurves;

//#pragma warning restore 0649
//        #endregion

//        private void Start()
//        {
//            //InventoryController.Instance.State.AddChangeListener(OnInventoryToggled);
//        }

//        private void OnInventoryToggled()
//        {
//            bool inventoryClosed = InventoryController.Instance.IsClosed;

//            if (m_DOF)
//                m_DOF.enabled = !inventoryClosed;

//            if (m_ColorCorrectionCurves)
//                m_ColorCorrectionCurves.enabled = !inventoryClosed;
//        }
//    }
//}
