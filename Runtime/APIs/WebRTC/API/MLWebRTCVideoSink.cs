// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCVideoSink.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Threading;
    using System.Collections.Generic;

#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a video sink used by the MLWebRTC API.
        /// Video sinks are fed data by media sources and produces frames to render.
        /// </summary>
        public partial class VideoSink : Sink
        {
            /// <summary>
            /// Buffer for the image planes array to use to hold the image plane data.
            /// </summary>
            private CircularBuffer<Frame.PlaneInfo[]> imagePlanesBuffer = CircularBuffer<Frame.PlaneInfo[]>.Create(new Frame.PlaneInfo[Frame.PlaneInfo.MaxImagePlanes], new Frame.PlaneInfo[Frame.PlaneInfo.MaxImagePlanes], new Frame.PlaneInfo[Frame.PlaneInfo.MaxImagePlanes]);

            /// <summary>
            /// The newest frame handle that the video sink knows of.
            /// </summary>
            private ulong newFrameHandle;

            private uint prevWidth = 0;
            private uint prevHeight = 0;

            private AutoResetEvent updateVideoEvent = new AutoResetEvent(true);

            public delegate void OnDestroySinkDelegate(VideoSink videoSink);

            public event OnDestroySinkDelegate OnDestroySink = delegate { };

            public delegate void OnFrameResolutionChangedDelegate(uint newWidth, uint newHeight);

            public event OnFrameResolutionChangedDelegate OnFrameResolutionChanged = delegate { };

            /// <summary>
            /// Initializes a new instance of the <see cref="VideoSink" /> class.
            /// </summary>
            internal VideoSink()
            {
                this.Type = MediaStream.Track.Type.Video;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="VideoSink" /> class.
            /// </summary>
            /// <param name="Handle">The Handle of the video sink.</param>
            internal VideoSink(ulong Handle) : base(Handle)
            {
                this.Type = MediaStream.Track.Type.Video;
            }

            /// <summary>
            /// Creates an initialized VideoSink object.
            /// </summary>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns> An initialized VideoSink object.</returns>
            public static VideoSink Create(out MLResult result)
            {
                VideoSink videoSink = null;
#if UNITY_MAGICLEAP || UNITY_ANDROID
                List<MLWebRTC.Sink> sinks = MLWebRTC.Instance.sinks;
                ulong Handle = MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkCreate(out Handle);
                if (!MLResult.DidNativeCallSucceed(resultCode, "MLWebRTCVideoSinkCreate()"))
                {
                    result = MLResult.Create(resultCode);
                    return videoSink;
                }

                videoSink = new VideoSink(Handle);
                if (MagicLeapNativeBindings.MLHandleIsValid(videoSink.Handle))
                {
                    sinks.Add(videoSink);
                }

                result = MLResult.Create(resultCode);
#else
                result = new MLResult();
#endif
                return videoSink;
            }

            public bool IsNewFrameAvailable()
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkIsNewFrameAvailable(this.Handle, out bool newFrameAvailable);
                MLResult.DidNativeCallSucceed(resultCode, "MLWebRTCVideoSinkIsNewFrameAvailable()");
                return newFrameAvailable;
#else
                return false;
#endif
            }

            public bool AcquireNextAvailableFrame(out Frame newFrame)
            {
                newFrame = new Frame();
#if UNITY_MAGICLEAP || UNITY_ANDROID
                ulong frameHandle = MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkAcquireNextAvailableFrame(this.Handle, out frameHandle);
                if (!MLResult.DidNativeCallSucceed(resultCode, "MLWebRTCVideoSinkAcquireNextAvailableFrame()"))
                {
                    return false;
                }

                Frame.NativeBindings.MLWebRTCFrame nativeFrame = Frame.NativeBindings.MLWebRTCFrame.Create(Frame.OutputFormat.YUV_420_888);
                resultCode = Frame.NativeBindings.MLWebRTCFrameGetData(frameHandle, ref nativeFrame);
                if (MLResult.DidNativeCallSucceed(resultCode, "MLWebRTCFrameGetData()"))
                {
                    newFrameHandle = frameHandle;
                    newFrame = Frame.Create(frameHandle, nativeFrame, imagePlanesBuffer.Get());

                    if (prevWidth != newFrame.NativeFrame.Width || prevHeight != newFrame.NativeFrame.Height)
                    {
                        MLThreadDispatch.Call(newFrame.NativeFrame.Width, newFrame.NativeFrame.Height, OnFrameResolutionChanged);
                    }
                    return true;
                }

                return false;
#else
                return false;
#endif
            }

            public void ReleaseFrame()
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                if (MagicLeapNativeBindings.MLHandleIsValid(newFrameHandle))
                {
                    MLResult.DidNativeCallSucceed(NativeBindings.MLWebRTCVideoSinkReleaseFrame(Handle, newFrameHandle), "MLWebRTCVideoSinkReleaseFrame()");
                    newFrameHandle = MagicLeapNativeBindings.InvalidHandle;
                }
#endif
            }

            /// <summary>
            /// Sets the track of the video sink.
            /// </summary>
            /// <param name="track">The track to use.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            protected override MLResult SetTrack(MediaStream.Track track)
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                ulong sourceHandle = track != null ? track.Handle : MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkSetSource(this.Handle, sourceHandle);
                MLResult.DidNativeCallSucceed(resultCode, "MLWebRTCVideoSinkSetSource()");
                return MLResult.Create(resultCode);
#else
                return new MLResult();
#endif
            }

            /// <summary>
            /// Sets the stream of the video sink sink.
            /// </summary>
            /// <param name="stream">The stream to use.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult SetStream(MediaStream stream)
            {
                if (Stream == stream)
                {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    return MLResult.Create(MLResult.Code.InvalidParam);
#endif
                }

                Stream = stream;
                if (Stream == null)
                {
                    return SetTrack(null);
                }

                return SetTrack(Stream.ActiveVideoTrack);
            }

            /// <summary>
            /// Destroys the video sink.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public override MLResult Destroy()
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                OnDestroySink(this);

                updateVideoEvent.WaitOne(250);
                this.SetStream(null);

                // TODO : synchronize with renderer
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkDestroy(this.Handle);
                MLResult.DidNativeCallSucceed(resultCode, "MLWebRTCVideoSinkDestroy()");
                this.InvalidateHandle();
                MLWebRTC.Instance.sinks.Remove(this);

                return MLResult.Create(resultCode);
#else
                return new MLResult();
#endif
            }
        }
    }
}