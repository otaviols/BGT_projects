using UnityEngine;

public static class UberMath
{
  private static readonly int[,] grad3 = new int[12, 3]
  {
    {
      1,
      1,
      0
    },
    {
      -1,
      1,
      0
    },
    {
      1,
      -1,
      0
    },
    {
      -1,
      -1,
      0
    },
    {
      1,
      0,
      1
    },
    {
      -1,
      0,
      1
    },
    {
      1,
      0,
      -1
    },
    {
      -1,
      0,
      -1
    },
    {
      0,
      1,
      1
    },
    {
      0,
      -1,
      1
    },
    {
      0,
      1,
      -1
    },
    {
      0,
      -1,
      -1
    }
  };
  private static readonly int[,] grad4 = new int[32, 4]
  {
    {
      0,
      1,
      1,
      1
    },
    {
      0,
      1,
      1,
      -1
    },
    {
      0,
      1,
      -1,
      1
    },
    {
      0,
      1,
      -1,
      -1
    },
    {
      0,
      -1,
      1,
      1
    },
    {
      0,
      -1,
      1,
      -1
    },
    {
      0,
      -1,
      -1,
      1
    },
    {
      0,
      -1,
      -1,
      -1
    },
    {
      1,
      0,
      1,
      1
    },
    {
      1,
      0,
      1,
      -1
    },
    {
      1,
      0,
      -1,
      1
    },
    {
      1,
      0,
      -1,
      -1
    },
    {
      -1,
      0,
      1,
      1
    },
    {
      -1,
      0,
      1,
      -1
    },
    {
      -1,
      0,
      -1,
      1
    },
    {
      -1,
      0,
      -1,
      -1
    },
    {
      1,
      1,
      0,
      1
    },
    {
      1,
      1,
      0,
      -1
    },
    {
      1,
      -1,
      0,
      1
    },
    {
      1,
      -1,
      0,
      -1
    },
    {
      -1,
      1,
      0,
      1
    },
    {
      -1,
      1,
      0,
      -1
    },
    {
      -1,
      -1,
      0,
      1
    },
    {
      -1,
      -1,
      0,
      -1
    },
    {
      1,
      1,
      1,
      0
    },
    {
      1,
      1,
      -1,
      0
    },
    {
      1,
      -1,
      1,
      0
    },
    {
      1,
      -1,
      -1,
      0
    },
    {
      -1,
      1,
      1,
      0
    },
    {
      -1,
      1,
      -1,
      0
    },
    {
      -1,
      -1,
      1,
      0
    },
    {
      -1,
      -1,
      -1,
      0
    }
  };
  private static readonly int[,] simplex = new int[64, 4]
  {
    {
      0,
      1,
      2,
      3
    },
    {
      0,
      1,
      3,
      2
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      2,
      3,
      1
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      1,
      2,
      3,
      0
    },
    {
      0,
      2,
      1,
      3
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      3,
      1,
      2
    },
    {
      0,
      3,
      2,
      1
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      1,
      3,
      2,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      1,
      2,
      0,
      3
    },
    {
      0,
      0,
      0,
      0
    },
    {
      1,
      3,
      0,
      2
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      2,
      3,
      0,
      1
    },
    {
      2,
      3,
      1,
      0
    },
    {
      1,
      0,
      2,
      3
    },
    {
      1,
      0,
      3,
      2
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      2,
      0,
      3,
      1
    },
    {
      0,
      0,
      0,
      0
    },
    {
      2,
      1,
      3,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      2,
      0,
      1,
      3
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      3,
      0,
      1,
      2
    },
    {
      3,
      0,
      2,
      1
    },
    {
      0,
      0,
      0,
      0
    },
    {
      3,
      1,
      2,
      0
    },
    {
      2,
      1,
      0,
      3
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      0,
      0,
      0,
      0
    },
    {
      3,
      1,
      0,
      2
    },
    {
      0,
      0,
      0,
      0
    },
    {
      3,
      2,
      0,
      1
    },
    {
      3,
      2,
      1,
      0
    }
  };
  private static int[] perm = new int[512];

  static UberMath()
  {
    for (int index = 0; index < 512; ++index)
      UberMath.perm[index] = Random.Range(5, 250);
  }

  private static int floor(float x) => (double) x <= 0.0 ? (int) x - 1 : (int) x;

  private static float dot(int gx, int gy, float x, float y) => (float) ((double) gx * (double) x + (double) gy * (double) y);

  private static float dot(int gx, int gy, int gz, float x, float y, float z) => (float) ((double) gx * (double) x + (double) gy * (double) y + (double) gz * (double) z);

  public static float SimplexNoise(float xin, float yin)
  {
    float num1 = 0.3660254f;
    float num2 = (xin + yin) * num1;
    int num3 = UberMath.floor(xin + num2);
    int num4 = UberMath.floor(yin + num2);
    float num5 = 0.2113249f;
    float num6 = (float) (num3 + num4) * num5;
    float num7 = (float) num4 - num6;
    float num8 = (float) num3 - num6;
    float y1 = yin - num7;
    float x1 = xin - num8;
    int num9;
    int num10;
    if ((double) x1 > (double) y1)
    {
      num9 = 1;
      num10 = 0;
    }
    else
    {
      num9 = 0;
      num10 = 1;
    }
    float x2 = x1 - (float) num9 + num5;
    float y2 = y1 - (float) num10 + num5;
    float x3 = (float) ((double) x1 - 1.0 + 2.0 * (double) num5);
    float y3 = (float) ((double) y1 - 1.0 + 2.0 * (double) num5);
    int num11 = num3 & (int) byte.MaxValue;
    int index1 = num4 & (int) byte.MaxValue;
    int index2 = UberMath.perm[num11 + UberMath.perm[index1]] % 12;
    int index3 = UberMath.perm[num11 + num9 + UberMath.perm[index1 + num10]] % 12;
    int index4 = UberMath.perm[num11 + 1 + UberMath.perm[index1 + 1]] % 12;
    float num12 = (float) (0.5 - (double) x1 * (double) x1 - (double) y1 * (double) y1);
    float num13;
    if ((double) num12 < 0.0)
    {
      num13 = 0.0f;
    }
    else
    {
      float num14 = num12 * num12;
      num13 = num14 * num14 * UberMath.dot(UberMath.grad3[index2, 0], UberMath.grad3[index2, 1], x1, y1);
    }
    float num15 = (float) (0.5 - (double) x2 * (double) x2 - (double) y2 * (double) y2);
    float num16;
    if ((double) num15 < 0.0)
    {
      num16 = 0.0f;
    }
    else
    {
      float num17 = num15 * num15;
      num16 = num17 * num17 * UberMath.dot(UberMath.grad3[index3, 0], UberMath.grad3[index3, 1], x2, y2);
    }
    float num18 = (float) (0.5 - (double) x3 * (double) x3 - (double) y3 * (double) y3);
    float num19;
    if ((double) num18 < 0.0)
    {
      num19 = 0.0f;
    }
    else
    {
      float num20 = num18 * num18;
      num19 = num20 * num20 * UberMath.dot(UberMath.grad3[index4, 0], UberMath.grad3[index4, 1], x3, y3);
    }
    return (float) (70.0 * ((double) num13 + (double) num16 + (double) num19));
  }

  public static float SimplexNoise(float xin, float yin, float zin)
  {
    float num1 = (float) (((double) xin + (double) yin + (double) zin) * 0.333333343267441);
    int num2 = UberMath.floor(xin + num1);
    int num3 = UberMath.floor(yin + num1);
    int num4 = UberMath.floor(zin + num1);
    float num5 = (float) (num2 + num3 + num4) * 0.1666667f;
    float num6 = (float) num2 - num5;
    float num7 = (float) num3 - num5;
    float num8 = (float) num4 - num5;
    float x1 = xin - num6;
    float y1 = yin - num7;
    float z1 = zin - num8;
    int num9;
    int num10;
    int num11;
    int num12;
    int num13;
    int num14;
    if ((double) x1 >= (double) y1)
    {
      if ((double) y1 >= (double) z1)
      {
        num9 = 1;
        num10 = 0;
        num11 = 0;
        num12 = 1;
        num13 = 1;
        num14 = 0;
      }
      else if ((double) x1 >= (double) z1)
      {
        num9 = 1;
        num10 = 0;
        num11 = 0;
        num12 = 1;
        num13 = 0;
        num14 = 1;
      }
      else
      {
        num9 = 0;
        num10 = 0;
        num11 = 1;
        num12 = 1;
        num13 = 0;
        num14 = 1;
      }
    }
    else if ((double) y1 < (double) z1)
    {
      num9 = 0;
      num10 = 0;
      num11 = 1;
      num12 = 0;
      num13 = 1;
      num14 = 1;
    }
    else if ((double) x1 < (double) z1)
    {
      num9 = 0;
      num10 = 1;
      num11 = 0;
      num12 = 0;
      num13 = 1;
      num14 = 1;
    }
    else
    {
      num9 = 0;
      num10 = 1;
      num11 = 0;
      num12 = 1;
      num13 = 1;
      num14 = 0;
    }
    float x2 = (float) ((double) x1 - (double) num9 + 0.16666667163372);
    float y2 = (float) ((double) y1 - (double) num10 + 0.16666667163372);
    float z2 = (float) ((double) z1 - (double) num11 + 0.16666667163372);
    float x3 = (float) ((double) x1 - (double) num12 + 0.333333343267441);
    float y3 = (float) ((double) y1 - (double) num13 + 0.333333343267441);
    float z3 = (float) ((double) z1 - (double) num14 + 0.333333343267441);
    float x4 = (float) ((double) x1 - 1.0 + 0.5);
    float y4 = (float) ((double) y1 - 1.0 + 0.5);
    float z4 = (float) ((double) z1 - 1.0 + 0.5);
    int num15 = num2 & (int) byte.MaxValue;
    int num16 = num3 & (int) byte.MaxValue;
    int index1 = num4 & (int) byte.MaxValue;
    int index2 = UberMath.perm[num15 + UberMath.perm[num16 + UberMath.perm[index1]]] % 12;
    int index3 = UberMath.perm[num15 + num9 + UberMath.perm[num16 + num10 + UberMath.perm[index1 + num11]]] % 12;
    int index4 = UberMath.perm[num15 + num12 + UberMath.perm[num16 + num13 + UberMath.perm[index1 + num14]]] % 12;
    int index5 = UberMath.perm[num15 + 1 + UberMath.perm[num16 + 1 + UberMath.perm[index1 + 1]]] % 12;
    float num17 = (float) (0.600000023841858 - (double) x1 * (double) x1 - (double) y1 * (double) y1 - (double) z1 * (double) z1);
    float num18;
    if ((double) num17 < 0.0)
    {
      num18 = 0.0f;
    }
    else
    {
      float num19 = num17 * num17;
      num18 = num19 * num19 * UberMath.dot(UberMath.grad3[index2, 0], UberMath.grad3[index2, 1], UberMath.grad3[index2, 2], x1, y1, z1);
    }
    float num20 = (float) (0.600000023841858 - (double) x2 * (double) x2 - (double) y2 * (double) y2 - (double) z2 * (double) z2);
    float num21;
    if ((double) num20 < 0.0)
    {
      num21 = 0.0f;
    }
    else
    {
      float num22 = num20 * num20;
      num21 = num22 * num22 * UberMath.dot(UberMath.grad3[index3, 0], UberMath.grad3[index3, 1], UberMath.grad3[index3, 2], x2, y2, z2);
    }
    float num23 = (float) (0.600000023841858 - (double) x3 * (double) x3 - (double) y3 * (double) y3 - (double) z3 * (double) z3);
    float num24;
    if ((double) num23 < 0.0)
    {
      num24 = 0.0f;
    }
    else
    {
      float num25 = num23 * num23;
      num24 = num25 * num25 * UberMath.dot(UberMath.grad3[index4, 0], UberMath.grad3[index4, 1], UberMath.grad3[index4, 2], x3, y3, z3);
    }
    float num26 = (float) (0.600000023841858 - (double) x4 * (double) x4 - (double) y4 * (double) y4 - (double) z4 * (double) z4);
    float num27;
    if ((double) num26 < 0.0)
    {
      num27 = 0.0f;
    }
    else
    {
      float num28 = num26 * num26;
      num27 = num28 * num28 * UberMath.dot(UberMath.grad3[index5, 0], UberMath.grad3[index5, 1], UberMath.grad3[index5, 2], x4, y4, z4);
    }
    return (float) (32.0 * ((double) num18 + (double) num21 + (double) num24 + (double) num27));
  }
}
