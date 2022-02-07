using System.Collections;
using Game.Models;
using UnityEngine;

namespace Game.ThirdPerson
{
    public class PlayerMovementController : MonoBehaviour
    {
        // TODO: fix because of ui is not handled by viewSystem and can be visiable in 
        private const float ShowUIDelay = 2f;
        private PlayerScenesModel scenesModel;
        private PlayerDeathModel deathModel;
        private PlayerMovementFacade playerFacade;
        private HotBarModel hotBarModel;

        #region MonoBehaviour
        private void Awake()
        {
            playerFacade = GetComponent<PlayerMovementFacade>();
            deathModel = ModelsSystem.Instance._playerDeathModel;
            scenesModel = ModelsSystem.Instance._playerScenesModel;
            hotBarModel = ModelsSystem.Instance._hotBarModel;
            playerFacade.CanShowUI = false;
        }
        private void OnEnable()
        {
            deathModel.OnRevival += OnRevival;
            deathModel.OnRevivalPrelim += OnRevival;
            hotBarModel.OnChangeEquipCell += OnChangeEquipCell;

            StartCoroutine(ShowUIWithDelay());
        }

        private void OnDisable()
        {
            deathModel.OnRevival -= OnRevival;
            deathModel.OnRevivalPrelim -= OnRevival;
            hotBarModel.OnChangeEquipCell -= OnChangeEquipCell;
        }
        #endregion

        private void OnRevival() => playerFacade.Reset();

        private void OnChangeEquipCell() => playerFacade.Reset();

        private IEnumerator ShowUIWithDelay()
        {
            yield return new WaitForSeconds(ShowUIDelay);

            playerFacade.CanShowUI = true;
        }
    }
}