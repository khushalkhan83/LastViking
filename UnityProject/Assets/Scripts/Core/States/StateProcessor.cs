using Game.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Core.StateMachine
{
    public class StateProcessor : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private bool __loggining;

        [SerializeField] private State _firstState;

#pragma warning restore 0649
        #endregion

        public State First => _firstState;

        public State State { get; private set; }

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        private void OnEnable()
        {
             BeginState(First); 
             GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            ForceEndBehaviour();
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        public void OnUpdate()
        {
            if (State == null)
            {
                return;
            }

            if (TryIterrupts(State.Interrupts, out var interrupt))
            {
                ForceInterrupt(interrupt);
            }
            else if (State.Behaviour == null)
            {
                if (TryIterrupts(State.Next, out interrupt))
                {
                    Interrupt(interrupt);
                }
            }

            State.Behaviour?.Refresh();
        }

        private void ForceInterrupt(Interrupt interrupt)
        {
            ForceEndBehaviour();
            ApplyEffects(interrupt.Effects);
            BeginState(interrupt.State);
        }

        private void Interrupt(Interrupt interrupt)
        {
            ApplyEffects(interrupt.Effects);
            BeginState(interrupt.State);
        }

        private void ApplyEffects(EffectBase[] effects)
        {
            if (effects != null)
            {
                foreach (var effect in effects)
                {
                    effect.Apply();
                }
            }
        }

        private void ContinueState(State state)
        {
            State = state;

            if (state?.Behaviour)
            {
                state.Behaviour.OnEnd += OnEndBehaviour;
                if (__loggining)
                    state.Behaviour.Log(x => $"<color=#ff3f3fff><b>Continue>>></b></Color> <color=#0f7f2fff>{x.transform.parent.name}</Color>.<color=#7fff7fff><b>{x.name}</b></Color>");
            }
        }

        private void BeginState(State state)
        {
            State = state;

            if (state?.Behaviour)
            {
                state.Behaviour.OnEnd += OnEndBehaviour;
                if (__loggining)
                {
                    state.Behaviour.Log(x => $"<color=#3f3fffff><b>Run>>></b></Color> <color=#0f7f2fff>{x.transform.parent.name}</Color>.<color=#7fff7fff><b>{x.name}</b></Color>");
                }
                state.Behaviour.Begin();
            }
        }

        private void ForceEndBehaviour()
        {
            if (State?.Behaviour)
            {
                State.Behaviour.OnEnd -= OnEndBehaviour;
                State.Behaviour.ForceEnd();
            }
        }

        public void RestoreState(State state)
        {
            ForceEndBehaviour();
            ContinueState(state);
        }

        public void TransitionTo(State state)
        {
            ForceEndBehaviour();
            BeginState(state);
        }

        private void OnEndBehaviour(BehaviourBase effect)
        {
            if (State.Behaviour)
            {
                if (__loggining)
                {
                    State.Behaviour.Log(x => $"<color=#3f3fffff><b> <<<End </b> </Color> <color=#0f7f2fff>{x.transform.parent.name}</Color>.<color=#7fff7fff><b>{x.name}</b></Color>");
                }

                State.Behaviour.OnEnd -= OnEndBehaviour;

            }

            if (TryIterrupts(State.Next, out var interrupt))
            {
                Interrupt(interrupt);
            }
        }

        List<Interrupt> __interruptsResult = new List<Interrupt>();

        private bool TryIterrupts(Interrupt[] interrupts, out Interrupt result)
        {
            result = null;

            bool isCondition;
            byte interruptId;
            byte conditionId;
            ConditionBase condition;
            Interrupt interrupt;

            for (interruptId = 0; interruptId < interrupts.Length; interruptId++)
            {
                interrupt = interrupts[interruptId];

                isCondition = true;

                if (interrupt.Conditions != null)
                {
                    for (conditionId = 0; conditionId < interrupt.Conditions.Length; conditionId++)
                    {
                        condition = interrupt.Conditions[conditionId];

                        if (!condition.IsTrue)
                        {
                            isCondition = false;
                            break;
                        }
                    }
                }

                //caching all interrputs with high priority
                if (isCondition)
                {
                    if (__interruptsResult.Count == 0)
                    {
                        __interruptsResult.Add(interrupt);
                    }
                    else if (interrupt.Priority > __interruptsResult[0].Priority)
                    {
                        __interruptsResult.Clear();
                        __interruptsResult.Add(interrupt);
                    }
                    else if (interrupt.Priority == __interruptsResult[0].Priority)
                    {
                        __interruptsResult.Add(interrupt);
                    }
                }
            }

            if (__interruptsResult.Count > 0)
            {
                result = __interruptsResult[Random.Range(0, __interruptsResult.Count)];
                __interruptsResult.Clear();
            }

            {//[LOG]
                if (result != null)
                {
                    if (__loggining)
                    {
                        result.Log(x => $"[<color=#3f3fffff><b>>>>Transition</b></Color> <color=#8f8fffff><b>{x?.Priority}</b></Color>] > [<color=#7f7f7fff>{x?.State?.transform.parent.name}</Color>.<color=#6fff6fff><b>{x?.State?.name}</b></Color>]");
                    }
                }

                return result != null;
            }
        }
    }
}