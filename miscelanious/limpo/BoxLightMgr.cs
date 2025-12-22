using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxLightMgr : MonoBehaviour
{
  public List<BoxLightState> m_States;
  private BoxLightStateType m_activeStateType = BoxLightStateType.DEFAULT;

  private void Start() => this.UpdateState();

  public BoxLightStateType GetActiveState() => this.m_activeStateType;

  public void ChangeState(BoxLightStateType stateType)
  {
    if (stateType == BoxLightStateType.INVALID || this.m_activeStateType == stateType)
      return;
    this.ChangeStateImpl(stateType);
  }

  public void SetState(BoxLightStateType stateType)
  {
    if (this.m_activeStateType == stateType)
      return;
    this.m_activeStateType = stateType;
    this.UpdateState();
  }

  public void UpdateState()
  {
    BoxLightState state = this.FindState(this.m_activeStateType);
    if (state == null)
      return;
    state.m_Spell.ActivateState(SpellStateType.ACTION);
    iTween.Stop(this.gameObject);
    RenderSettings.ambientLight = state.m_AmbientColor;
    if (state.m_LightInfos == null)
      return;
    foreach (BoxLightInfo lightInfo in state.m_LightInfos)
    {
      iTween.Stop(lightInfo.m_Light.gameObject);
      lightInfo.m_Light.color = lightInfo.m_Color;
      lightInfo.m_Light.intensity = lightInfo.m_Intensity;
      LightType type = lightInfo.m_Light.type;
      switch (type)
      {
        case LightType.Spot:
        case LightType.Point:
          lightInfo.m_Light.range = lightInfo.m_Range;
          if (type == LightType.Spot)
          {
            lightInfo.m_Light.spotAngle = lightInfo.m_SpotAngle;
            continue;
          }
          continue;
        default:
          continue;
      }
    }
  }

  private BoxLightState FindState(BoxLightStateType stateType)
  {
    foreach (BoxLightState state in this.m_States)
    {
      if (state.m_Type == stateType)
        return state;
    }
    return (BoxLightState) null;
  }

  private void ChangeStateImpl(BoxLightStateType stateType)
  {
    this.m_activeStateType = stateType;
    BoxLightState state = this.FindState(stateType);
    if (state == null)
      return;
    iTween.Stop(this.gameObject);
    state.m_Spell.ActivateState(SpellStateType.BIRTH);
    this.ChangeAmbient(state);
    if (state.m_LightInfos == null)
      return;
    foreach (BoxLightInfo lightInfo in state.m_LightInfos)
      this.ChangeLight(state, lightInfo);
  }

  private void ChangeAmbient(BoxLightState state)
  {
    Color prevAmbientColor = RenderSettings.ambientLight;
    Action<object> action = (Action<object>) (amount => RenderSettings.ambientLight = Color.Lerp(prevAmbientColor, state.m_AmbientColor, (float) amount));
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "delay", (object) state.m_DelaySec, (object) "time", (object) state.m_TransitionSec, (object) "easetype", (object) state.m_TransitionEaseType, (object) "onupdate", (object) action));
  }

  private void ChangeLight(BoxLightState state, BoxLightInfo lightInfo)
  {
    iTween.Stop(lightInfo.m_Light.gameObject);
    Hashtable args1 = iTween.Hash((object) "color", (object) lightInfo.m_Color, (object) "delay", (object) state.m_DelaySec, (object) "time", (object) state.m_TransitionSec, (object) "easetype", (object) state.m_TransitionEaseType);
    iTween.ColorTo(lightInfo.m_Light.gameObject, args1);
    float intensity = lightInfo.m_Light.intensity;
    Action<object> action1 = (Action<object>) (amount => lightInfo.m_Light.intensity = (float) amount);
    Hashtable args2 = iTween.Hash((object) "from", (object) intensity, (object) "to", (object) lightInfo.m_Intensity, (object) "delay", (object) state.m_DelaySec, (object) "time", (object) state.m_TransitionSec, (object) "easetype", (object) state.m_TransitionEaseType, (object) "onupdate", (object) action1);
    iTween.ValueTo(lightInfo.m_Light.gameObject, args2);
    LightType type = lightInfo.m_Light.type;
    switch (type)
    {
      case LightType.Spot:
      case LightType.Point:
        float range = lightInfo.m_Light.range;
        Action<object> action2 = (Action<object>) (amount => lightInfo.m_Light.range = (float) amount);
        Hashtable args3 = iTween.Hash((object) "from", (object) range, (object) "to", (object) lightInfo.m_Range, (object) "delay", (object) state.m_DelaySec, (object) "time", (object) state.m_TransitionSec, (object) "easetype", (object) state.m_TransitionEaseType, (object) "onupdate", (object) action2);
        iTween.ValueTo(lightInfo.m_Light.gameObject, args3);
        if (type != LightType.Spot)
          break;
        float spotAngle = lightInfo.m_Light.spotAngle;
        Action<object> action3 = (Action<object>) (amount => lightInfo.m_Light.spotAngle = (float) amount);
        Hashtable args4 = iTween.Hash((object) "from", (object) spotAngle, (object) "to", (object) lightInfo.m_SpotAngle, (object) "delay", (object) state.m_DelaySec, (object) "time", (object) state.m_TransitionSec, (object) "easetype", (object) state.m_TransitionEaseType, (object) "onupdate", (object) action3);
        iTween.ValueTo(lightInfo.m_Light.gameObject, args4);
        break;
    }
  }
}
