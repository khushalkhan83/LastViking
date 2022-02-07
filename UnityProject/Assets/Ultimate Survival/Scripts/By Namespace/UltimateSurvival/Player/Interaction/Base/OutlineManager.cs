using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace UltimateSurvival
{
    public class OutlineManager : MonoBehaviour
    {
        Dictionary<IOutlineTarget, Dictionary<Renderer,Outline>> outlineDicByTarget;

    void UpdateRendererList(IOutlineTarget obj)
    {
        //Debug.Log("update renderers " + obj.ToString());
        UnRegisterObject(obj);
        RegisterObject(obj);
    }

    void RegisterObject(IOutlineTarget target)
    {
        if (outlineDicByTarget == null)
            outlineDicByTarget = new Dictionary<IOutlineTarget, Dictionary<Renderer,Outline>>();
        if (!outlineDicByTarget.ContainsKey(target))
        {
            Dictionary<Renderer,Outline> dic = new Dictionary<Renderer, Outline>();
            outlineDicByTarget.Add(target, dic);

            foreach (Renderer rrr in target.GetRenderers())
            {
                if (rrr != null)
                {
                    var outl = rrr.GetComponent<Outline>();
                    if (outl == null)
                        outl = rrr.gameObject.AddComponent<Outline>();

                        (outl as Outline).color = target.GetColor();
                    outl.enabled = false;
                    dic.Add(rrr,outl);
                }
            }

            target.OnUpdateRendererList += UpdateRendererList;
        }
    }

    void UnRegisterObject(IOutlineTarget t)
    {
        if (outlineDicByTarget != null)
        {
            if (outlineDicByTarget.ContainsKey(t))
            {
                outlineDicByTarget.Remove(t);
                t.OnUpdateRendererList -= UpdateRendererList;
            }
        }
    }

        IOutlineTarget _wasTarget = null;

        public void SetOutline(IOutlineTarget target)
        {
            if (outlineDicByTarget == null)
            {
                outlineDicByTarget = new Dictionary<IOutlineTarget, Dictionary<Renderer, Outline>>();
            }

            if (outlineDicByTarget != null)
            {
                if (_wasTarget != target && _wasTarget != null && outlineDicByTarget.ContainsKey(_wasTarget))
                {
                    bool needToRefresh = false;
                    foreach (var keyValuePair in outlineDicByTarget[_wasTarget])
                    {
                        if (keyValuePair.Value == null)
                        {
                            needToRefresh = true;
                            break;
                        }
                        else
                            keyValuePair.Value.enabled = false;
                    }
                    if (needToRefresh)
                        UpdateRendererList(_wasTarget);
                }

                if (target != null)
                {

                    if (!outlineDicByTarget.ContainsKey(target))
                    {
                        RegisterObject(target);
                    }
                    foreach (var keyValuePair in outlineDicByTarget[target])
                    {
                        if(keyValuePair.Value != null)
                        {
                            bool rendererNotNull = keyValuePair.Key != null;
                            
                            keyValuePair.Value.enabled = rendererNotNull ? keyValuePair.Key.isVisible : true;
                        }
                    }
                }
            }
            _wasTarget = target;
        }
    }
}