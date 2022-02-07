//using UnityEngine;
//using UnityEngine.UI;

//namespace UltimateSurvival.GUISystem
//{
//    public class PlayerStatsGUI : GUIBehaviour
//    {

//        #region Data
//#pragma warning disable 0649

//        [SerializeField] private Text m_DefenseText;

//#pragma warning restore 0649
//        #endregion

//        private void Start()
//        {
//            Player.Defense.AddChangeListener(OnChanged_Defense);
//        }

//        private void OnChanged_Defense()
//        {
//            m_DefenseText.text = Player.Defense.Get() + "%";
//        }
//    }
//}
