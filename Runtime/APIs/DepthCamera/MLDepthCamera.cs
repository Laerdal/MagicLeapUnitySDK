// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// APIs to access the depth camera data.
    /// <para>This is an experimental API which may be modified or removed without any prior notice.</para>
    /// <para>The API only supports reading data from the depth camera. Apps cannot modify the camera settings, support for the same may be added in a future release.</para>
    /// </summary>
    public partial class MLDepthCamera : MLAutoAPISingleton<MLDepthCamera>
    {
        /// <summary>
        /// Depth Camera modes<br/>
        /// Future release may add support to other modes.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Long range mode
            /// <para>Under normal operations long range mode has a maximum frequency of 5fps and a range of up to 5m, in some cases this can go as far 7.5m.</para>
            /// </summary>
            LongRange = 1 << 0
        }

        /// <summary>
        /// Depth Camera frame capture types
        /// </summary>
        public enum FrameType
        {
            /// <summary>
            /// Unknown or no frame type
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Frame captured using <see cref="Mode.LongRange"/> mode.
            /// </summary>
            LongRange = 1
        }

        /// <summary>
        /// Flags used to specify what kind of data to request from Depth Camera
        /// </summary>
        [Flags]
        public enum CaptureFlags
        {
            /// <summary>
            /// Enable DepthImage. See <see cref="Data.DepthImage"/> for more details.
            /// </summary>
            DepthImage = 1 << 0,

            /// <summary>
            /// Enable ConfidenceBuffer. See <see cref="Data.ConfidenceBuffer"/> for more details.
            /// </summary>
            Confidence = 1 << 1,

            /// <summary>
            /// Enable DepthFlagsBuffer. See <see cref="Data.DepthFlagsBuffer"/> for more details.
            /// </summary>
            DepthFlags = 1 << 2,

            /// <summary>
            /// Enable AmbientRawDepthImage. See <see cref="Data.AmbientRawDepthImage"/> for more details.
            /// </summary>
            AmbientRawDepthImage = 1 << 3
        }

        /// <summary>
        /// Flags to select data requested from depth camera.
        /// </summary>
        [Flags]
        public enum DepthFlags
        {
            /// <summary>
            /// Indicates that there is no additional flag data for this pixel.
            /// </summary>
            Valid = 0 << 0,

            /// <summary>
            /// This bit is set to one to indicate that one or more flags from below have
            /// been set. Depending on the use case the application can correlate the
            /// flag data and corresponding pixel data to determine how to handle the pixel
            /// </summary>
            Invalid = 1 << 0,

            /// <summary>
            /// The pixel intensity is either below the min or the max threshold value.
            /// </summary>
            Saturated = 1 << 1,

            /// <summary>
            /// Inconsistent data received when capturing frames. This can happen due to
            /// fast motion.
            /// </summary>
            Inconsistent = 1 << 2,

            /// <summary>
            /// Pixel has very low signal to noise ratio. One example of when this can
            /// happen is for pixels in far end of the range.
            /// </summary>
            LowSignal = 1 << 3,

            /// <summary>
            /// This typically happens when there is step jump in the distance of adjoining
            /// pixels in the scene. Example: When you open a door looking into the room the
            /// edges along the door's edges can cause flying pixels.
            /// </summary>
            FlyingPixel = 1 << 4,

            /// <summary>
            /// If this bit is on it indicates that the corresponding pixel may not be within
            /// the projector's illumination cone.
            /// </summary>
            Masked = 1 << 5,

            /// <summary>
            /// This bit will be set when there is high noise.
            /// </summary>
            SBI = 1 << 8,

            /// <summary>
            /// This could happen when there is another light source apart from the depth
            /// camera projector. This could also lead to <see cref="LowSignal"/>.
            /// </summary>
            StrayLight = 1 << 9,

            /// <summary>
            /// If a small group of <see cref="Valid"/> is sorrunded by a set of
            /// <see cref="Invalid"/> then this bit will be set to 1.
            /// </summary>
            ConnectedComponent = 1 << 10
        }

        /// <summary>
        /// Depth Camera Settings
        /// <para>API Level 22</para>
        /// </summary>
        public struct Settings
        {
            /// <summary>
            /// Flags to configure the depth data.
            /// </summary>
            public CaptureFlags Flags;

            /// <summary>
            /// Depth camera Mode.
            /// <para>See <see cref="Mode"/> for more details.</para>
            /// <para>NOTE: The system may not be able to service all the requested modes at any given time. This parameter is treated as a hint and data will be provided for the requested modes if available.</para>
            /// </summary>
            public Mode Mode;
        }

        /// <summary>
        /// Depth camera intrinsic parameters.
        /// <para>API Level 22</para>
        /// </summary>
        public struct Intrinsics
        {
            /// <summary>
            /// Camera Width
            /// </summary>
            public uint Width;

            /// <summary>
            /// Camera Height
            /// </summary>
            public uint Height;

            /// <summary>
            /// Camera Focal Length
            /// </summary>
            public Vector2 FocalLength;

            /// <summary>
            /// Camera Principal Point
            /// </summary>
            public Vector2 PrincipalPoint;

            /// <summary>
            /// Field of View in degrees
            /// </summary>
            public float FoV;

            /// <summary>
            /// Set of distortion coefficients.
            /// <para>The distortion coefficients are arranged in the following order: [k1, k2, p1, p2, k3]</para>
            /// </summary>
            public DistortionCoefficients Distortion;

            /// <summary>
            /// Convenience method to retrieve a list of the Distortion coefficient values in the correct order.
            /// </summary>
            /// <returns></returns>
            public List<double> GetDistortionList()
            {
                return new List<double>() { Distortion.K1, Distortion.K2, Distortion.P1, Distortion.P2, Distortion.K3 };
            }
        }

        /// <summary>
        /// The distortion coefficients are arranged in the following order: [k1, k2, p1, p2, k3]
        /// </summary>
        public readonly struct DistortionCoefficients
        {
            /// <summary>
            /// Distortion coefficient k1
            /// </summary>
            public readonly double K1;

            /// <summary>
            /// Distortion coefficient k2
            /// </summary>
            public readonly double K2;

            /// <summary>
            /// Distortion coefficient p1
            /// </summary>
            public readonly double P1;

            /// <summary>
            /// Distortion coefficient p2
            /// </summary>
            public readonly double P2;

            /// <summary>
            /// Distortion coefficient k3
            /// </summary>
            public readonly double K3;

            public DistortionCoefficients(double[] coefficients)
            {
                if (coefficients == null || coefficients.Length < 5)
                {
                    throw new ArgumentException("DistortionCoefficients constructor must receive an array of 5 values and the array cannot be null.");
                }
                K1 = coefficients[0];
                K2 = coefficients[1];
                P1 = coefficients[2];
                P2 = coefficients[3];
                K3 = coefficients[4];
            }
        }

        /// <summary>
        /// Per-plane info for each depth camera frame.
        /// <para>API Level 22</para>
        /// </summary>
        public struct FrameBuffer
        {
            /// <summary>
            /// Width of the buffer in pixels.
            /// </summary>
            public uint Width;

            /// <summary>
            /// Height of the buffer in pixels.
            /// </summary>
            public uint Height;

            /// <summary>
            /// Stride of the buffer in bytes.
            /// </summary>
            public uint Stride;

            /// <summary>
            /// Number of bytes used to represent a single value.
            /// </summary>
            public uint BytesPerUnit;

            /// <summary>
            /// Buffer data.
            /// </summary>
            public byte[] Data;

            public override string ToString()
            {
                return $"[FrameBuffer W: {Width}, H: {Height}, Stride: {Stride}, BPU: {BytesPerUnit}, Data: {(Data != null ? Data.Length + " bytes" : "null")}]";
            }
        }

        /// <summary>
        /// The settings the Depth Camera is currently configured with.
        /// </summary>
        public static Settings CurrentSettings { get; private set; }

        /// <summary>
        /// Sets the current settings of Depth Camera.
        /// </summary>
        /// <param name="settings"></param>
        public static void SetSettings(Settings settings) => CurrentSettings = settings;

        public static bool IsConnected { get; private set; }

        private bool connectionPaused;

        protected override MLResult.Code StartAPI() => MLResult.Code.Ok;

        protected override MLResult.Code StopAPI()
        {
            var result = MLResult.Code.Ok;

            if (IsConnected)
            {
                result = InternalDisconnect().Result;
            }

            return result;
        }

        protected override void OnApplicationPause(bool pauseStatus)
        {
            base.OnApplicationPause(pauseStatus);

            if (pauseStatus)
            {
                if (IsConnected)
                {
                    InternalDisconnect(true);
                }
            }
            else
            {
                if (connectionPaused)
                {
                    InternalConnect(CurrentSettings);
                }
            }
        }

        /// <summary>
        /// Connect to depth camera.
        /// <para>API Level 22</para>
        /// permissions com.magicleap.permission.DEPTH_CAMERA (protection level: dangerous)
        /// </summary>
        /// <returns>
        /// MLResult.Code.InvalidParam: One of the parameters is invalid.<br/>
        /// MLResult.Code.Ok: Connected to camera device(s) successfully.<br/>
        /// MLResult.Code.PermissionDenied: Necessary permission is missing.<br/>
        /// MLResult.Code.LicenseError: Necessary license is missing.<br/>
        /// MLResult.Code.UnspecifiedFailure: The operation failed with an unspecified error.
        /// </returns>
        public static MLResult Connect() => Instance.InternalConnect(CurrentSettings);

        /// <summary>
        /// Disconnect from depth camera.
        /// <para>API Level 22</para>
        /// permissions None
        /// </summary>
        /// <returns>
        /// MLResult.Code.InvalidParam: The camera's handle was invalid.<br/>
        /// MLResult.Code.Ok: Disconnected camera successfully.<br/>
        /// MLResult.Code.UnspecifiedFailure: Failed to disconnect camera for some unknown reason.
        /// </returns>
        public static MLResult Disconnect() => Instance.InternalDisconnect();

        /// <summary>
        /// Update the depth camera settings.
        /// <para>API Level 22</para>
        /// permissions None
        /// </summary>
        /// <param name="settings">New <see cref="Settings"/> for the depth camera.</param>
        /// <returns>
        /// MLResult.Code.InvalidParam: The camera's handle was invalid.<br/>
        /// MLResult.Code.Ok: Settings updated successfully.<br/>
        /// MLResult.Code.UnspecifiedFailure: Failed due to internal error.
        /// </returns>
        public static MLResult UpdateSettings(Settings settings) => Instance.InternalUpdateSettings(settings);

        /// <summary>
        /// Poll for Frames.
        /// <para>Returns a <see cref="Data"/> object referencing the latest available frame data, if any.</para>
        /// <para>This is a blocking call. API is not thread safe.</para>
        /// <para>If there are no new depth data frames for a given duration (duration determined by the system) then the API will return <see cref="MLResult.Code.Timeout"/>.</para>
        /// <para>API Level 22</para>
        /// permissions None
        /// </summary>
        /// <param name="timeoutMs">Timeout in milliseconds.</param>
        /// <param name="data">Depth camera data. Will be null if no valid data is available when called.</param>
        /// <returns>
        /// MLResult.Code.InvalidParam: The camera's handle was invalid.<br/>
        /// MLResult.Code.Ok: Depth camera data fetched successfully.<br/>
        /// MLResult.Code.Timeout: No frame available within time limit.<br/>
        /// MLResult.Code.UnspecifiedFailure: Failed due to internal error.
        /// </returns>
        public static MLResult GetLatestDepthData(ulong timeoutMs, out Data data) => Instance.InternalGetLatestDepthData(timeoutMs, out data);

        #region internal
        private MLResult InternalConnect(Settings settings)
        {
            if (!MLResult.DidNativeCallSucceed(MLPermissions.CheckPermission(MLPermission.DepthCamera).Result))
            {
                MLPluginLog.Error($"{nameof(MLDepthCamera)} requires missing permission {MLPermission.DepthCamera}");
                return MLResult.Create(MLResult.Code.PermissionDenied);
            }

            var camSettings = NativeBindings.MLDepthCameraSettings.Init();
            camSettings.Flags = (uint)settings.Flags;
            camSettings.Mode = (uint)settings.Mode;
            var resultCode = NativeBindings.MLDepthCameraConnect(in camSettings, out Handle);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLDepthCameraConnect)))
            {
                IsConnected = true;
            }
            return MLResult.Create(resultCode);
        }

        private MLResult InternalDisconnect(bool paused = false)
        {
            var resultCode = NativeBindings.MLDepthCameraDisconnect(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLDepthCameraDisconnect));
            connectionPaused = paused;
            if (!connectionPaused)
            {
                IsConnected = false;
            }
            return MLResult.Create(resultCode);
        }

        private MLResult InternalUpdateSettings(Settings settings)
        {
            var depthCamSettings = NativeBindings.MLDepthCameraSettings.Init();
            depthCamSettings.Flags = (uint)settings.Flags;
            depthCamSettings.Mode = (uint)settings.Mode;
            var resultCode = NativeBindings.MLDepthCameraUpdateSettings(Handle, in depthCamSettings);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLDepthCameraUpdateSettings)))
            {
                CurrentSettings = settings;
            }
            return MLResult.Create(resultCode);
        }

        private MLResult InternalGetLatestDepthData(ulong timeoutMs, out Data data)
        {
            var depthCamData = NativeBindings.MLDepthCameraData.Init();
            IntPtr dataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(depthCamData));
            Marshal.StructureToPtr(depthCamData, dataPtr, false);

            var resultCode = NativeBindings.MLDepthCameraGetLatestDepthData(Handle, timeoutMs, out dataPtr);

            // in this case, a Timeout is an acceptable result that we don't need to log as an error to the console.
            bool resultIsOk = (resultCode == MLResult.Code.Ok || resultCode == MLResult.Code.Timeout);
            if (!MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLDepthCameraGetLatestDepthData), showError: !resultIsOk) || dataPtr == IntPtr.Zero)
            {
                data = null;
            }
            else
            {
                depthCamData = (NativeBindings.MLDepthCameraData)Marshal.PtrToStructure(dataPtr, typeof(NativeBindings.MLDepthCameraData));

                FrameBuffer CreateFromPtr(IntPtr ptr)
                {
                    var result = new FrameBuffer();
                    if (ptr == IntPtr.Zero)
                    {
                        return result;
                    }
                    var plane = (NativeBindings.MLDepthCameraFrameBuffer)Marshal.PtrToStructure(ptr, typeof(NativeBindings.MLDepthCameraFrameBuffer));
                    byte[] bytes = null;
                    if (plane.Data != IntPtr.Zero)
                    {
                        bytes = new byte[plane.Size];
                        Marshal.Copy(plane.Data, bytes, 0, bytes.Length);
                    }
                    result = new FrameBuffer()
                    {
                        Width = plane.Width,
                        Height = plane.Height,
                        Stride = plane.Stride,
                        BytesPerUnit = plane.BytesPerUnit,
                        Data = bytes
                    };

                    return result;
                }

                var depthMap = CreateFromPtr(depthCamData.DepthImageFrameBufferPtr);
                var confidenceMap = CreateFromPtr(depthCamData.ConfidenceBufferFrameBufferPtr);
                var depthFlags = CreateFromPtr(depthCamData.DepthFlagsBufferFrameBufferPtr);
                var aiMap = CreateFromPtr(depthCamData.AmbientRawDepthImageFrameBufferPtr);

                data = new Data()
                {
                    FrameNumber = depthCamData.FrameNumber,
                    FrameTimestamp = depthCamData.FrameTimestamp,
                    FrameType = depthCamData.FrameType,
                    Position = Native.MLConvert.ToUnity(depthCamData.CameraPose.Position),
                    Rotation = Native.MLConvert.ToUnity(depthCamData.CameraPose.Rotation),
                    Intrinsics = NativeBindings.MLDepthCameraIntrinsics.ToManaged(depthCamData.Intrinsics),
                    DepthImage = (depthMap.Data != null) ? depthMap : null,
                    ConfidenceBuffer = (confidenceMap.Data != null) ? confidenceMap : null,
                    DepthFlagsBuffer = (depthFlags.Data != null) ? depthFlags : null,
                    AmbientRawDepthImage = (aiMap.Data != null) ? aiMap : null
                };

                // CAPI specifies that Release should be called exactly once for each successful call to GetLatest
                // Since we return a managed "copy" of the data in the out parameter, then we can immediately do this rather than require the application developer to do it themselves.
                var releaseResult = NativeBindings.MLDepthCameraReleaseDepthData(Handle, dataPtr);
                MLResult.DidNativeCallSucceed(releaseResult, nameof(NativeBindings.MLDepthCameraReleaseDepthData));
            }

            Marshal.FreeHGlobal(dataPtr);
            return MLResult.Create(resultCode);
        }
        #endregion
    }
}
