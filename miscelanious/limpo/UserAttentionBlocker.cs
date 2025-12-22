using System;

[Flags]
public enum UserAttentionBlocker
{
  NONE = 0,
  FATAL_ERROR_SCENE = 1,
  SET_ROTATION_INTRO = 2,
  SET_ROTATION_CM_TUTORIALS = 4,
  ALL = -1, // 0xFFFFFFFF
  ALL_EXCEPT_FATAL_ERROR_SCENE = -2, // 0xFFFFFFFE
}
