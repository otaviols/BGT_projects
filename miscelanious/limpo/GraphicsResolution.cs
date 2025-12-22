using Hearthstone;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphicsResolution : IComparable
{
  private const int MIN_ASPECT_RATIO_WIDTH = 4;
  private const int MIN_ASPECT_RATIO_HEIGHT = 3;
  private const int MAX_ASPECT_RATIO_WIDTH = 16;
  private const int MAX_ASPECT_RATIO_HEIGHT = 9;
  private const int MIN_WINDOW_WIDTH = 400;
  private const int MIN_WINDOW_HEIGHT = 400;
  private const int TASKBAR_HEIGHT = 63;
  private const float ASPECT_RATIO_ERROR_ALLOWANCE = 0.051f;
  public static readonly List<GraphicsResolution> resolutions_ = new List<GraphicsResolution>();

  private GraphicsResolution()
  {
  }

  private GraphicsResolution(int width, int height)
  {
    this.x = width;
    this.y = height;
    this.aspectRatio = (float) this.x / (float) this.y;
  }

  public static GraphicsResolution create(Resolution res) => new GraphicsResolution(res.width, res.height);

  public static GraphicsResolution create(int width, int height) => new GraphicsResolution(width, height);

  private static bool add(int width, int height)
  {
    GraphicsResolution graphicsResolution = new GraphicsResolution(width, height);
    if (GraphicsResolution.resolutions_.BinarySearch(graphicsResolution) >= 0)
      return false;
    GraphicsResolution.resolutions_.Add(graphicsResolution);
    GraphicsResolution.resolutions_.Sort();
    return true;
  }

  public static List<GraphicsResolution> list
  {
    get
    {
      if (GraphicsResolution.resolutions_.Count == 0)
      {
        lock (GraphicsResolution.resolutions_)
        {
          foreach (Resolution resolution in Screen.resolutions)
          {
            if (GraphicsResolution.IsAspectRatioWithinLimit(resolution.width, resolution.height, false))
              GraphicsResolution.add(resolution.width, resolution.height);
          }
          GraphicsResolution.resolutions_.Reverse();
        }
      }
      return GraphicsResolution.resolutions_;
    }
  }

  public static GraphicsResolution current => GraphicsResolution.create(Screen.currentResolution);

  public int x { get; private set; }

  public int y { get; private set; }

  public float aspectRatio { get; private set; }

  public int CompareTo(object obj)
  {
    if (!(obj is GraphicsResolution graphicsResolution))
      return 1;
    if (this.x < graphicsResolution.x)
      return -1;
    if (this.x > graphicsResolution.x)
      return 1;
    if (this.y < graphicsResolution.y)
      return -1;
    return this.y > graphicsResolution.y ? 1 : 0;
  }

  public override bool Equals(object obj) => obj != null && obj is GraphicsResolution graphicsResolution && this.x == graphicsResolution.x && this.y == graphicsResolution.y;

  public override int GetHashCode() => (23 * 17 + this.x.GetHashCode()) * 17 + this.y.GetHashCode();

  public static GraphicsResolution GetLargestResolution() => GraphicsResolution.list.First<GraphicsResolution>();

  public static bool IsAspectRatioWithinLimit(int width, int height, bool isWindowedMode)
  {
    if (HearthstoneApplication.IsInternal())
      return true;
    if (isWindowedMode)
      height += 63;
    return width >= 400 && height >= 400 && GraphicsResolution.CompareAspectRatio(16, 9, width, height) >= 0 && GraphicsResolution.CompareAspectRatio(width, height, 4, 3) >= 0;
  }

  public static int CompareAspectRatio(int lWidth, int lHeight, int rWidth, int rHeight)
  {
    float num1 = (float) lWidth / (float) lHeight;
    float num2 = (float) rWidth / (float) rHeight;
    if ((double) Mathf.Abs(num1 - num2) < 0.0509999990463257)
      return 0;
    return (double) num1 > (double) num2 ? 1 : -1;
  }

  public static int[] CalcAspectRatioLimit(int x, int y)
  {
    int lWidth = x;
    if (lWidth < 400)
      lWidth = 400;
    int lHeight = y;
    if (lHeight < 400)
      lHeight = 400;
    if (GraphicsResolution.CompareAspectRatio(lWidth, lHeight, 16, 9) > 0)
      lWidth = (int) ((double) lHeight * 16.0 / 9.0);
    else if (GraphicsResolution.CompareAspectRatio(lWidth, lHeight, 16, 9) < 0)
      lWidth = (int) ((double) lHeight * 4.0 / 3.0);
    return new int[2]{ lWidth, lHeight };
  }
}
