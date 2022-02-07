using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Game.Models
{
    public class ResourceMessagesViewModel : MonoBehaviour
    {
        #region Data
		#pragma warning disable 0649

        [SerializeField] private ObscuredFloat _timeShowMessage;

	    [SerializeField] private int _acceleratedShowTimeAfterMessages = 4;
	    
	    [SerializeField] private int _mergedMessageShowTime = 3;
		
		#pragma warning restore 0649
        #endregion

        public ObscuredFloat TimeShowMessage => _timeShowMessage;

	    public int AcceleratedShowTimeAfterMessages => _acceleratedShowTimeAfterMessages;

	    public float MergedMessageShowTime => _mergedMessageShowTime;
    }
}
