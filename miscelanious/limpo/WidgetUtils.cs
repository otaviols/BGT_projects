using Hearthstone.DataModels;
using Hearthstone.UI;
using UnityEngine;

public static class WidgetUtils
{
  public static EventDataModel GetEventDataModel(VisualController vc)
  {
    if ((Object) vc == (Object) null)
      return (EventDataModel) null;
    Widget component = vc.GetComponent<Widget>();
    return (Object) component == (Object) null ? (EventDataModel) null : component.GetDataModel<EventDataModel>();
  }

  public static void BindorCreateDataModel<T>(Widget owner, int modelId, ref T dataModel) where T : class, IDataModel, new()
  {
    if ((object) dataModel == null)
    {
      if (owner.GetDataModel(modelId, out IDataModel _))
        return;
      IDataModel dataModel1 = (IDataModel) new T();
      owner.BindDataModel(dataModel1);
      dataModel = dataModel1 as T;
    }
    else
      owner.BindDataModel((IDataModel) dataModel);
  }
}
