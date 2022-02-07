using NaughtyAttributes;
using UnityEngine;


namespace Game.AI.Behaviours.Kraken
{
    [CreateAssetMenu(fileName = "SO_config_krakenAI_default", menuName = "Config/AI/Kraken", order = 0)]
    public class KrakenConfig : ValueProviderScriptable, IDamageValueProvider
    {
        [Header("Health")]
        [SerializeField] private float healthOnStart = 400;
        [SerializeField] private float maxHealth = 800;

        [Header("Combat: general")]
        [SerializeField] private float throwForce = 50;
        [SerializeField] private float shakeScale = 2;
        [SerializeField] private float damage = 30;

        [Header("Combat: on start")]
        [SerializeField] private bool useWarningSmash = true;
        [ShowIf("useWarningSmash")] [SerializeField] private int warningSmashesCount = 1;

        [Header("Combat: smash")]
        [SerializeField] private float holdLegOnGroundTime = 15;
        [SerializeField] private float damageToCancelHoldLegOnGround = 25;

        [Header("Run Away")]
        [SerializeField] private bool runAway;
        [ShowIf("runAway")] [Range(0,1)] [SerializeField] private float healthPercentageToRunAway = 0.3f;


        public float HealthOnStart => healthOnStart;
        public float MaxHealth => maxHealth;
        public float ThrowForce => throwForce;
        public float ShakeScale => shakeScale;
        public float Damage => damage;
        public bool UseWarningSmash => useWarningSmash;
        public int WarningSmashesCount => warningSmashesCount;
        public float HoldLegOnGroundTime => holdLegOnGroundTime;
        public float DamageToCancelHoldLegOnGround => damageToCancelHoldLegOnGround;
        public float HealthPercentageToRun => healthPercentageToRunAway;
        public bool RunAway => runAway;
        public float HealthToRunAway => maxHealth * healthPercentageToRunAway;

        #region Interfaces Realization
        public float DamageValue => damage;
            
        #endregion

        #region Test Buttons
        [Button] void ShowInConsoleHealthToRunAway()
        {
            if(runAway)
                Debug.Log("health to run away: " + HealthToRunAway);
            else
                Debug.Log("Run away disabled");
        }
        #endregion

    }
}

public abstract class ValueProviderScriptable : ScriptableObject { }

public interface IDamageValueProvider
{
    float DamageValue {get;}
}