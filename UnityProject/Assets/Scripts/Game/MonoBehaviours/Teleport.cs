using System;
using System.Collections;
using System.Collections.Generic;
using Core.Storage;
using Game.Interactables;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

public class Teleport : ItemsRelatedInteractableBase
{
    [Serializable]
    public class Data : DataBase
    {
        public bool itemsPlaced;          

        public void SeItemsPlaced(bool value) 
        {
            itemsPlaced = value;
            ChangeData();
        }         
    }

    [SerializeField] private Data data = default;
    [SerializeField] private SavableItem[] requiredItems = default;
    [SerializeField] private Collider interactionCollider = default;
    [SerializeField] private Collider teleportCollider = default;
    [SerializeField] private ColliderTriggerModel colliderTriggerModel = default;
    [SerializeField] private GameObject activeTeleportObject = default;
    [SerializeField] private Teleport reletedTeleport = default;
    [SerializeField] private bool dependOnReletedTeleport = false;
    [SerializeField] private Transform playerSpawnPosition = default;


    private bool processed = false;

    public event Action OnItemsPlaced;

    public override SavableItem[] RequiredItems => requiredItems;
    private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
    private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
    private ViewsSystem ViewsSystem => ViewsSystem.Instance;
    private PlayerHealthModel PlayerHealthModel =>  ModelsSystem.Instance._playerHealthModel;


    public bool ItemsPlaced
    {
        get{return data.itemsPlaced;}
        private set{data.SeItemsPlaced(value);}
    }

    public bool Active
    {
        get
        {
            if(dependOnReletedTeleport)
            {
                return reletedTeleport != null && reletedTeleport.ItemsPlaced;
            }
            else
            {
                return ItemsPlaced;
            }
        }
    }
    public Transform PlayerSpawnPosition => playerSpawnPosition;

    private void OnEnable() 
    {
        if(dependOnReletedTeleport)
        {
            reletedTeleport.OnItemsPlaced += OnRelatedTeleportItemsPlaced;
        }
        else
        {
            if(!processed)
            {
                processed = StorageModel.TryProcessing(data);
                if(ItemsPlaced) OnItemsPlaced?.Invoke();
            }
        }
        colliderTriggerModel.OnEnteredTrigger += OnTeleportTriggerEnter;
        SetupTeleportObject();
    }

    private void OnDisable() 
    {
        reletedTeleport.OnItemsPlaced -= OnRelatedTeleportItemsPlaced;
        colliderTriggerModel.OnEnteredTrigger -= OnTeleportTriggerEnter;
    }

    public override bool CanUse() => !ItemsPlaced;
    public override void Use()
    {
        ItemsPlaced = true;
        SetupTeleportObject();
        OnItemsPlaced?.Invoke();
    }

    private void SetupTeleportObject()
    {
        interactionCollider.enabled = !Active && !dependOnReletedTeleport;
        teleportCollider.enabled = Active;
        activeTeleportObject.SetActive(Active);
    }

    private void OnRelatedTeleportItemsPlaced()
    {
        SetupTeleportObject();
    }

    private void OnTeleportTriggerEnter(Collider other) 
    {
        if(Active && other.gameObject.TryGetComponent<PlayerEventHandler>(out var component))
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        if(PlayerHealthModel.IsDead)
            return;

        var characterController = PlayerEventHandler.GetComponent<CharacterController>();

        if(characterController == null) 
            return;

        StartCoroutine(TeleportPlayerCoroutine(characterController));
    }

    private IEnumerator TeleportPlayerCoroutine(CharacterController characterController)
    {
        ViewsSystem.Show(ViewConfigID.TeleportEffectConfig);

        yield return new WaitForSeconds(0.2f);

        if(!PlayerHealthModel.IsDead)
        {
            PlayerEventHandler.transform.position = reletedTeleport.PlayerSpawnPosition.position;
            PlayerEventHandler.Velocity.Set(Vector3.zero);
            characterController.Move(Vector3.zero);
        }
    }
}
