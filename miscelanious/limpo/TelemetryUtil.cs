using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.Telemetry.Standard.Network;
using Blizzard.Telemetry.WTCG.Client;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class TelemetryUtil
{
  public static Disconnect.Reason GetReasonFromBnetError(BattleNetErrors error)
  {
    switch (error)
    {
      case BattleNetErrors.ERROR_OK:
        return Disconnect.Reason.LOCAL;
      case BattleNetErrors.ERROR_TIMED_OUT:
      case BattleNetErrors.ERROR_LOGON_WEB_VERIFY_TIMEOUT:
      case BattleNetErrors.ERROR_RPC_REQUEST_TIMED_OUT:
        return Disconnect.Reason.TIMEOUT;
      default:
        return Disconnect.Reason.REMOTE;
    }
  }

  public static UnitySystemInfo GetUnitySystemInfo() => new UnitySystemInfo()
  {
    BatteryLevel = SystemInfo.batteryLevel,
    BatteryStatus = (UnitySystemInfo.BatteryStatusEnum) (SystemInfo.batteryStatus + 1),
    CopyTextureSupport = (UnitySystemInfo.CopyTextureSupportEnum) (SystemInfo.copyTextureSupport + 1),
    DeviceModel = SystemInfo.deviceModel,
    DeviceName = SystemInfo.deviceName,
    DeviceType = (UnitySystemInfo.DeviceTypeEnum) (SystemInfo.deviceType + 1),
    DeviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier,
    GraphicsDeviceID = (long) SystemInfo.graphicsDeviceID,
    GraphicsDeviceName = SystemInfo.graphicsDeviceName,
    GraphicsDeviceType = (UnitySystemInfo.GraphicsDeviceTypeEnum) (SystemInfo.graphicsDeviceType + 1),
    GraphicsDeviceVendor = SystemInfo.graphicsDeviceVendor,
    GraphicsDeviceVendorID = (long) SystemInfo.graphicsDeviceVendorID,
    GraphicsDeviceVersion = SystemInfo.graphicsDeviceVersion,
    GraphicsMemorySize = SystemInfo.graphicsMemorySize,
    GraphicsMultiThreaded = SystemInfo.graphicsMultiThreaded,
    GraphicsShaderLevel = SystemInfo.graphicsShaderLevel,
    GraphicsUVStartsAtTop = SystemInfo.graphicsUVStartsAtTop,
    HasDynamicUniformArrayIndexingInFragmentShaders = SystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders,
    HasHiddenSurfaceRemovalOnGPU = SystemInfo.hasHiddenSurfaceRemovalOnGPU,
    HasMipMaxLevel = SystemInfo.hasMipMaxLevel,
    MaxComputeBufferInputsCompute = SystemInfo.maxComputeBufferInputsCompute,
    MaxComputeBufferInputsDomain = SystemInfo.maxComputeBufferInputsDomain,
    MaxComputeBufferInputsFragment = SystemInfo.maxComputeBufferInputsFragment,
    MaxComputeBufferInputsGeometry = SystemInfo.maxComputeBufferInputsGeometry,
    MaxComputeBufferInputsHull = SystemInfo.maxComputeBufferInputsHull,
    MaxComputeBufferInputsVertex = SystemInfo.maxComputeBufferInputsVertex,
    MaxComputeWorkGroupSize = SystemInfo.maxComputeWorkGroupSize,
    MaxComputeWorkGroupSizeX = SystemInfo.maxComputeWorkGroupSizeX,
    MaxComputeWorkGroupSizeY = SystemInfo.maxComputeWorkGroupSizeY,
    MaxComputeWorkGroupSizeZ = SystemInfo.maxComputeWorkGroupSizeZ,
    MaxCubemapSize = SystemInfo.maxCubemapSize,
    MaxTextureSize = SystemInfo.maxTextureSize,
    NpotSupport = (UnitySystemInfo.NPOTSupportEnum) (SystemInfo.npotSupport + 1),
    OperatingSystem = SystemInfo.operatingSystem,
    OperatingSystemFamily = (UnitySystemInfo.OperatingSystemFamilyEnum) (SystemInfo.operatingSystemFamily + 1),
    ProcessorCount = SystemInfo.processorCount,
    ProcessorFrequency = SystemInfo.processorFrequency,
    ProcessorType = SystemInfo.processorType,
    RenderingThreadingMode = (UnitySystemInfo.RenderingThreadingModeEnum) (SystemInfo.renderingThreadingMode + 1),
    SupportedRandomWriteTargetCount = SystemInfo.supportedRandomWriteTargetCount,
    SupportedRenderTargetCount = SystemInfo.supportedRenderTargetCount,
    Supports2DArrayTextures = SystemInfo.supports2DArrayTextures,
    Supports32bitsIndexBuffer = SystemInfo.supports32bitsIndexBuffer,
    Supports3DRenderTextures = SystemInfo.supports3DRenderTextures,
    Supports3DTextures = SystemInfo.supports3DTextures,
    SupportsAccelerometer = SystemInfo.supportsAccelerometer,
    SupportsAsyncCompute = SystemInfo.supportsAsyncCompute,
    SupportsAsyncGPUReadback = SystemInfo.supportsAsyncGPUReadback,
    SupportsAudio = SystemInfo.supportsAudio,
    SupportsComputeShaders = SystemInfo.supportsComputeShaders,
    SupportsCubemapArrayTextures = SystemInfo.supportsCubemapArrayTextures,
    SupportsGeometryShaders = SystemInfo.supportsGeometryShaders,
    SupportsGraphicsFence = SystemInfo.supportsGraphicsFence,
    SupportsGyroscope = SystemInfo.supportsGyroscope,
    SupportsHardwareQuadTopology = SystemInfo.supportsHardwareQuadTopology,
    SupportsInstancing = SystemInfo.supportsInstancing,
    SupportsLocationService = SystemInfo.supportsLocationService,
    SupportsMipStreaming = SystemInfo.supportsMipStreaming,
    SupportsMotionVectors = SystemInfo.supportsMotionVectors,
    SupportsMultisampleAutoResolve = SystemInfo.supportsMultisampleAutoResolve,
    SupportsMultisampledTextures = SystemInfo.supportsMultisampledTextures == 1,
    SupportsRawShadowDepthSampling = SystemInfo.supportsRawShadowDepthSampling,
    SupportsRayTracing = SystemInfo.supportsRayTracing,
    SupportsSeparatedRenderTargetsBlend = SystemInfo.supportsSeparatedRenderTargetsBlend,
    SupportsSetConstantBuffer = SystemInfo.supportsSetConstantBuffer,
    SupportsShadows = SystemInfo.supportsShadows,
    SupportsSparseTextures = SystemInfo.supportsSparseTextures,
    SupportsStoreAndResolveAction = SystemInfo.supportsStoreAndResolveAction,
    SupportsTessellationShaders = SystemInfo.supportsTessellationShaders,
    SupportsTextureWrapMirrorOnce = SystemInfo.supportsTextureWrapMirrorOnce == 1,
    SupportsVibration = SystemInfo.supportsVibration,
    SystemMemorySize = SystemInfo.systemMemorySize,
    UnsupportedIdentifier = "n/a",
    UsesLoadStoreActions = SystemInfo.usesLoadStoreActions,
    UsesReversedZBuffer = SystemInfo.usesReversedZBuffer,
    GraphicsFormatForLDR = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR).ToString(),
    GraphicsFormatForHDR = SystemInfo.GetGraphicsFormat(DefaultFormat.HDR).ToString(),
    SupportedRenderTextureFormats = TelemetryUtil.GetSupportedTextureFormats<RenderTextureFormat>(new TelemetryUtil.CanSupport<RenderTextureFormat>(SystemInfo.SupportsRenderTextureFormat)),
    SupportedTextureFormats = TelemetryUtil.GetSupportedTextureFormats<TextureFormat>(new TelemetryUtil.CanSupport<TextureFormat>(SystemInfo.SupportsTextureFormat))
  };

  private static List<string> GetSupportedTextureFormats<T>(TelemetryUtil.CanSupport<T> func)
  {
    DateTime now = DateTime.Now;
    List<string> supportedTextureFormats = new List<string>();
    foreach (T obj in Enum.GetValues(typeof (T)))
    {
      try
      {
        if (func(obj))
          supportedTextureFormats.Add(obj.ToString());
      }
      catch (Exception ex)
      {
        Log.Telemetry.PrintWarning(string.Format("Failed to check the texture format('{0}'): {1}", (object) obj, (object) ex.Message));
      }
    }
    Log.Telemetry.PrintDebug("Elapsed time(ms) of checking '{0}': {1}", (object) typeof (T).FullName, (object) (long) DateTime.Now.Subtract(now).TotalMilliseconds);
    return supportedTextureFormats;
  }

  private delegate bool CanSupport<T>(T arg);
}
