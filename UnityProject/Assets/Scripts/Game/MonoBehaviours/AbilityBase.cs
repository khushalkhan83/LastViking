using Game.Models;
using System;
using UnityEngine;

[Serializable]
public class AbilityBase
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private LocalizationKeyID _localizationKeyID;
    [SerializeField] private Sprite _sprite;

#pragma warning restore 0649
    #endregion

    public LocalizationKeyID LocalizationKeyID => _localizationKeyID;
    public Sprite Sprite => _sprite;
}
