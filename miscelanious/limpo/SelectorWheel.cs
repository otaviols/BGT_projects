using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectorWheel : MonoBehaviour
{
  [SerializeField]
  protected PegUIElement m_dragRegion;
  [SerializeField]
  protected PegUIElement m_cruiseUpRegion;
  [SerializeField]
  protected PegUIElement m_cruiseDownRegion;
  [SerializeField]
  protected GameObject m_tileBase;
  [SerializeField]
  protected float m_radius = 2f;
  [SerializeField]
  protected float m_flyBraking = 4f;
  [SerializeField]
  protected float m_flyDamping = 1f;
  [SerializeField]
  protected float m_snapDamping = 8f;
  [SerializeField]
  protected float m_snapForce = 40f;
  [SerializeField]
  protected float m_tileSpacing = 1f;
  [SerializeField]
  protected int m_maxTiles = 7;
  [SerializeField]
  protected bool m_globalScrolling = true;
  [SerializeField]
  protected bool m_invertScrolling = true;
  [SerializeField]
  protected float m_scrollingSpeed = 15f;
  [SerializeField]
  protected int m_numTiles = 1;
  [SerializeField]
  protected bool m_topToBottom = true;
  [SerializeField]
  protected float m_cruiseStartSpeed = 4f;
  [SerializeField]
  protected float m_cruiseEndSpeed = 24f;
  [SerializeField]
  protected float m_exponentialCruising = 0.5f;
  private SelectorWheel.Tile[] m_tiles;
  private float m_velocity;
  private float m_snapInfluence = 1f;
  private float m_wheelPosition;
  private int m_lastSelection;
  private Vector3? m_lastDragPos;
  private float? m_scrollTargetPosition;
  private int m_cruiseDir;
  private List<IDataModel> m_dataModels = new List<IDataModel>();

  private void Start()
  {
    this.m_tiles = new SelectorWheel.Tile[this.m_maxTiles];
    for (int index = 0; index < this.m_maxTiles; ++index)
    {
      this.m_tiles[index] = new SelectorWheel.Tile();
      this.m_tiles[index].indexOffset = index;
      this.m_tiles[index].gameObject = index != 0 ? UnityEngine.Object.Instantiate<GameObject>(this.m_tileBase, this.m_tileBase.transform.parent) : this.m_tileBase;
    }
    if ((UnityEngine.Object) this.m_dragRegion != (UnityEngine.Object) null)
      this.m_dragRegion.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e =>
      {
        this.m_lastDragPos = new Vector3?(this.GetLocalMousePos());
        PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
      }));
    this.InitializeCruiseClicker(this.m_cruiseUpRegion, 1);
    this.InitializeCruiseClicker(this.m_cruiseDownRegion, -1);
    this.SetIndex(0);
  }

  private void Update()
  {
    float deltaTime = Time.deltaTime;
    this.UpdateInput(deltaTime);
    float wheelPosition = this.m_wheelPosition;
    this.SimulateWheel(deltaTime);
    if ((double) Mathf.Abs(this.m_wheelPosition - wheelPosition) < 1.0 / 1000.0)
      return;
    this.UpdateSelection();
    this.LayoutTiles();
  }

  public event Action OnSelectionChanged;

  public int TileCount
  {
    get => this.m_numTiles;
    protected set
    {
      this.m_numTiles = value;
      if ((double) this.m_wheelPosition >= (double) this.m_numTiles)
        this.SetIndex(this.m_numTiles > 0 ? this.m_numTiles - 1 : 0);
      this.LayoutTiles();
    }
  }

  public void SetIndex(int index, bool instant = true)
  {
    float positionFromIndex = this.GetPositionFromIndex(index);
    if (instant)
    {
      this.m_wheelPosition = positionFromIndex;
      this.m_velocity = 0.0f;
      this.LayoutTiles();
    }
    else
      this.m_scrollTargetPosition = new float?(positionFromIndex);
  }

  public int GetSelectedIndex() => this.GetIndexFromPosition(Mathf.RoundToInt(this.m_wheelPosition));

  public void SetDataModels(List<IDataModel> dataModels)
  {
    this.m_dataModels = dataModels;
    this.TileCount = this.m_dataModels.Count;
  }

  public IDataModel GetDataModel(int index) => index < 0 || index >= this.m_dataModels.Count ? (IDataModel) null : this.m_dataModels[index];

  private void UpdateInput(float deltaTime)
  {
    Camera camera = this.GetCamera();
    bool flag = false;
    if (this.m_globalScrolling)
      flag = true;
    else if (UniversalInputManager.Get() != null && (UnityEngine.Object) camera != (UnityEngine.Object) null)
      flag = UniversalInputManager.Get().ForcedInputIsOver(camera, this.m_dragRegion.gameObject);
    if (flag)
    {
      float axis = Input.GetAxis("Mouse ScrollWheel");
      if ((double) axis != 0.0)
      {
        float num1 = Mathf.Sign(axis);
        if (this.m_invertScrolling)
          num1 = -num1;
        if (!this.m_scrollTargetPosition.HasValue)
          this.m_scrollTargetPosition = new float?(Mathf.Round(this.m_wheelPosition));
        float num2 = 1f;
        float? scrollTargetPosition = this.m_scrollTargetPosition;
        float num3 = num1 * num2;
        this.m_scrollTargetPosition = scrollTargetPosition.HasValue ? new float?(scrollTargetPosition.GetValueOrDefault() + num3) : new float?();
        this.m_scrollTargetPosition = new float?(Mathf.Clamp(this.m_scrollTargetPosition.Value, 0.0f, (float) (this.m_numTiles - 1)));
        this.m_snapInfluence = 1f;
        this.AbortCruise();
      }
    }
    if (this.m_lastDragPos.HasValue)
    {
      if (InputCollection.GetMouseButtonUp(0) || UniversalInputManager.Get().WasTouchCanceled())
      {
        this.m_lastDragPos = new Vector3?();
        PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
      }
      else
      {
        Vector3 localMousePos = this.GetLocalMousePos();
        Vector3 vector3 = localMousePos - this.m_lastDragPos.Value;
        this.m_lastDragPos = new Vector3?(localMousePos);
        this.m_velocity = -vector3.y / this.m_tileSpacing / deltaTime;
        this.m_snapInfluence = 0.0f;
      }
    }
    if (this.m_cruiseDir == 0)
      return;
    if (InputCollection.GetMouseButtonUp(0) || UniversalInputManager.Get().WasTouchCanceled())
      this.AbortCruise();
    if ((double) Mathf.Sign(this.m_velocity) == (double) Mathf.Sign((float) this.m_cruiseDir))
      this.m_velocity = (float) this.m_cruiseDir * Mathf.Clamp(Mathf.Abs(this.m_velocity) * Mathf.Exp(this.m_exponentialCruising * deltaTime), this.m_cruiseStartSpeed, this.m_cruiseEndSpeed);
    this.m_snapInfluence = 1f;
  }

  private void SimulateWheel(float deltaTime)
  {
    float num1 = 0.0f;
    float num2 = (float) (this.m_numTiles - 1);
    bool flag = this.m_lastDragPos.HasValue || this.m_cruiseDir != 0;
    if (this.m_scrollTargetPosition.HasValue)
    {
      float f = this.m_scrollTargetPosition.Value - this.m_wheelPosition;
      float num3 = Mathf.Abs(f);
      if ((double) num3 >= 0.5)
      {
        float max = (float) (0.5 + (double) num3 / 2.0);
        this.m_velocity = Mathf.Clamp(f, -max, max) * this.m_scrollingSpeed;
      }
      else
      {
        num2 = num1 = this.m_scrollTargetPosition.Value;
        float num4 = (float) (0.5 * (0.5 - (double) num3));
        if ((double) Mathf.Sign(this.m_velocity) != (double) Mathf.Sign(f))
          num4 = (float) (4.0 / (0.509999990463257 - (double) num3));
        this.m_velocity *= Mathf.Exp(-deltaTime * num4);
        if ((double) Mathf.Abs(this.m_velocity) < 0.100000001490116)
          this.m_scrollTargetPosition = new float?();
      }
    }
    else
      this.m_snapInfluence = Mathf.Lerp(this.m_snapInfluence, Mathf.Max(this.m_snapInfluence, (float) (1.0 / (1.0 + 4.0 * (double) Mathf.Abs(this.m_velocity)))), flag ? 1f : 1f - Mathf.Exp(-deltaTime * this.m_flyBraking));
    if (!flag)
    {
      double f = ((double) this.m_wheelPosition % 1.0 + 1.5) % 1.0 - 0.5;
      float num5 = Mathf.Abs((float) f);
      float num6 = (float) -Math.Sign((float) f);
      this.m_velocity += ((double) num5 > 0.449999988079071 ? num6 * (float) (1.0 - ((double) num5 - 0.449999988079071) / 0.050000011920929) : num6 * (num5 / 0.45f)) * deltaTime * this.m_snapForce * this.m_snapInfluence;
      this.m_velocity *= Mathf.Exp(-Mathf.Lerp(this.m_flyDamping, this.m_snapDamping, this.m_snapInfluence) * deltaTime);
    }
    this.m_wheelPosition += this.m_velocity * deltaTime;
    float min = num1 - 0.49f;
    float max1 = num2 + 0.49f;
    if (((double) this.m_wheelPosition > (double) min || (double) this.m_velocity >= 0.0) && ((double) this.m_wheelPosition < (double) max1 || (double) this.m_velocity <= 0.0))
      return;
    this.m_wheelPosition = Mathf.Clamp(this.m_wheelPosition, min, max1);
    this.m_velocity = 0.0f;
    this.AbortCruise();
  }

  private void UpdateSelection()
  {
    int num = Mathf.RoundToInt(this.m_wheelPosition);
    if (this.m_lastSelection == num)
      return;
    this.m_lastSelection = num;
    if (this.OnSelectionChanged == null)
      return;
    this.OnSelectionChanged();
  }

  private void LayoutTiles()
  {
    foreach (SelectorWheel.Tile tile in this.m_tiles)
    {
      int num = Mathf.FloorToInt((float) (((double) this.m_wheelPosition - (double) tile.indexOffset) / (double) this.m_maxTiles + 0.5));
      int position = tile.indexOffset + num * this.m_maxTiles;
      if (position < 0 || position >= this.m_numTiles)
      {
        tile.gameObject.SetActive(false);
      }
      else
      {
        tile.gameObject.SetActive(true);
        float f = ((float) position - this.m_wheelPosition) * this.m_tileSpacing / this.m_radius;
        Vector3 localPosition = tile.gameObject.transform.localPosition with
        {
          y = this.m_radius * Mathf.Sin(f),
          z = this.m_radius * (1f - Mathf.Cos(f))
        };
        tile.gameObject.transform.localPosition = localPosition;
        tile.gameObject.transform.localEulerAngles = new Vector3((float) ((double) f * 180.0 / 3.14159274101257), 0.0f, 0.0f);
        int indexFromPosition = this.GetIndexFromPosition(position);
        this.AssignIndexToTile(tile.gameObject, indexFromPosition);
      }
    }
  }

  protected void AssignIndexToTile(GameObject tile, int index)
  {
    Widget componentInChildren = tile.GetComponentInChildren<Widget>();
    IDataModel dataModel = this.GetDataModel(index);
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null) || dataModel == null)
      return;
    componentInChildren.BindDataModel(dataModel);
  }

  private Camera GetCamera() => CameraUtils.FindFirstByLayer(this.gameObject.layer);

  private Vector3 GetLocalMousePos()
  {
    Camera camera = this.GetCamera();
    Vector3 min = this.m_dragRegion.GetComponent<BoxCollider>().bounds.min;
    Plane plane = new Plane(-camera.transform.forward, min);
    Ray ray = camera.ScreenPointToRay(InputCollection.GetMousePosition());
    float enter;
    return plane.Raycast(ray, out enter) ? this.transform.InverseTransformPoint(ray.GetPoint(enter)) : Vector3.zero;
  }

  private void InitializeCruiseClicker(PegUIElement cruiser, int dir)
  {
    if ((UnityEngine.Object) cruiser == (UnityEngine.Object) null)
      return;
    cruiser.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e =>
    {
      this.m_cruiseDir = this.m_invertScrolling ? -dir : dir;
      this.m_velocity = (float) this.m_cruiseDir * Mathf.Max((double) Mathf.Sign(this.m_velocity) == (double) Mathf.Sign((float) this.m_cruiseDir) ? Mathf.Abs(this.m_velocity) : 0.0f, this.m_cruiseStartSpeed);
      this.m_scrollTargetPosition = new float?();
    }));
    cruiser.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
    {
      if (this.m_cruiseDir == 0)
        return;
      this.m_scrollTargetPosition = new float?(Mathf.Clamp(Mathf.Round(this.m_wheelPosition + (float) this.m_cruiseDir * 0.51f), 0.0f, (float) (this.m_numTiles - 1)));
      this.m_cruiseDir = 0;
    }));
    PegCursor.Mode mode = dir > 0 ? PegCursor.Mode.UPARROW : PegCursor.Mode.DOWNARROW;
    cruiser.SetCursorOver(mode);
    cruiser.SetCursorDown(mode);
  }

  private int GetIndexFromPosition(int position) => !this.m_topToBottom ? position : this.m_numTiles - 1 - position;

  private float GetPositionFromIndex(int index) => this.m_topToBottom ? (float) (this.m_numTiles - 1 - index) : (float) index;

  private void AbortCruise() => this.m_cruiseDir = 0;

  private struct Tile
  {
    public GameObject gameObject;
    public int indexOffset;
  }
}
