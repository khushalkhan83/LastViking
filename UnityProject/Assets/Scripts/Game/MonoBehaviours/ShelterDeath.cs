using Game.Models;
using System.Linq;
using UltimateSurvival;
using UnityEngine;


public class ShelterDeath : MonoBehaviour
{

    private IHealth _health;
    private IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

    private SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;

    private void OnEnable()
    {
        Health.OnDeath += OnDeathShelterHandler;
    }

    private void OnDisable()
    {
        Health.OnDeath -= OnDeathShelterHandler;
    }

    private void OnDeathShelterHandler()
    {
        SheltersModel.ShelterModel.Death();
    }
}
