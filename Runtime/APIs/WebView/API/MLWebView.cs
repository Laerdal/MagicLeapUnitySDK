// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebView.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_MAGICLEAP || UNITY_ANDROID

namespace UnityEngine.XR.MagicLeap
{
    using System;

    /// <summary>
    /// API for MLWebView that allows an application to instantiate a hardware
    /// accelerated WebView and interact with it(via "mouse" and "keyboard" events).
    /// </summary>
    public partial class MLWebView : MLAPIBase
    {
        /// <summary>
        /// Create a MLWebView. 
        /// The MLWebView will be ready to use once this function returns with MLResult_OK.
        /// </summary>
        /// <param name="width">Width of the WebView in pixels.</param>
        /// <param name="height">Height of the WebView in pixels.</param>
        /// <returns>MLWebView instance if creation was successful, null otherwise.</returns>
        public static MLWebView Create(uint width, uint height)
        {
            MLWebView webView = new MLWebView();
            return webView.CreateInternal(width, height) == MLResult.Code.Ok ? webView : null;

        }

        /// <summary>
        /// Destroy a MLWebView. The MLWebView will be terminated by this function call and the  shall no longer be valid.
        /// </summary>
        /// <returns>MLResult.Code.Ok if was destroyed successfully.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if an error occurred destroying the MLWebView.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult Destroy() => MLResult.Create(DestroyInternal());

        /// <summary>
        /// Retrieves the assigned web view handle.
        /// </summary>
        /// <returns>The assigned web view handle, MagicLeapNativeBindings.InvalidHandle if it has not been created.</returns>
        public ulong WebViewHandle
        {
            get => Handle;
        }

        /// <summary>
        /// Flag to indicate if urls issuing certificate errors should be loaded or not
        /// </summary>
        public bool IgnoreCertificateError 
        {
            get; set;
        } = false;

        /// <summary>
        /// Go to a URL with the specified MLWebView.  Note that success with this call only indicates that a load will be
        /// attempted.  Caller can be notified about issues loading the URL via the event r on_load_error.
        /// </summary>
        /// <param name="url">URL that will be loaded.</param>
        /// <returns>MLResult.Code.Ok if WebView is attempting to load the specified URL.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult GoTo(string url) => MLResult.Create(GoToInternal(url));

        /// <summary>
        /// Trigger a "Reload" action in the MLWebView.
        /// </summary>
        /// <returns>MLResult.Code.Ok if WebView Reload action was initiated.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult Reload() => MLResult.Create(ReloadInternal());

        /// <summary>
        /// Trigger a "Back" action in the MLWebView.  Query #MLWebViewCanGoBack before calling this method.  If there is no valid
        /// page to go back to, this method will be no-op.
        /// </summary>
        /// <returns>MLResult.Code.Ok if WebView Back action was initiated or cannot go back any further.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult GoBack() => MLResult.Create(GoBackInternal());

        /// <summary>
        /// Trigger a "Forward" action in the MLWebView.  Query MLWebViewCanGoForward before calling this method.  If there is no
        /// valid page to go forward to, this method will be no-op.
        /// </summary>
        /// <returns>MLResult.Code.Ok if WebView Forward action was initiated or cannot go forward any further.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult GoForward() => MLResult.Create(GoForwardInternal());

        /// <summary>
        /// Checks if the "Back" action is currently valid.
        /// </summary>
        /// <returns>True if can use the "Back" action.</returns>
        public bool CanGoBack() => CanGoBackInternal();

        /// <summary>
        /// Checks if the "Forward" action is currently valid.
        /// </summary>
        /// <returns>True if can use the "Forward" action.</returns>
        public bool CanGoForward() => CanGoForwardInternal();

        /// <summary>
        /// Get the current URL.
        /// </summary>
        /// <returns>Current URL.</returns>
        public string GetURL() => GetURLInternal();

        /// <summary>
        /// Moves the WebView mouse.
        /// </summary>
        /// <param name="xPosition">Horizontal position of the cursor.</param>
        /// <param name="yPosition">Vertical position of the cursor. </param>
        /// <param name="modifiers">Should be one or combination of EventFlags.</param>
        /// <returns>MLResult.Code.Ok if internal mouse was moved.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectMouseMove(uint xPosition, uint yPosition, EventFlags modifiers) => MLResult.Create(InjectMouseMoveInternal(xPosition, yPosition, modifiers));

        /// <summary>
        /// Sends a mouse button down/pressed event on a specific location on screen. 
        /// </summary>
        /// <param name="xPosition">Horizontal position of the cursor.</param>
        /// <param name="yPosition">Vertical position of the cursor.</param>
        /// <param name="modifiers">Should be one or combination of EventFlags.</param>
        /// <param name="buttonType">The mouse button being pressed.</param>
        /// <returns>MLResult.Code.Ok if successful.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectMouseButtonDown(uint xPosition, uint yPosition, EventFlags modifiers, MouseButtonType buttonType) => MLResult.Create(InjectMouseButtonDownInternal(xPosition, yPosition, modifiers, buttonType));

        /// <summary>
        /// Sends a mouse button up/released event on a specific location on screen.
        /// </summary>
        /// <param name="xPosition">Horizontal position of the cursor.</param>
        /// <param name="yPosition">Vertical position of the cursor.</param>
        /// <param name="modifiers">Should be one or combination of EventFlags.</param>
        /// <param name="buttonType">The mouse button being pressed.</param>
        /// <returns>MLResult.Code.Ok if successful.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectMouseButtonUp(uint xPosition, uint yPosition, EventFlags modifiers, MouseButtonType buttonType) => MLResult.Create(InjectMouseButtonUpInternal(xPosition, yPosition, modifiers, buttonType));

        /// <summary>
        /// Sends a printable char keyboard event to MLWebView.
        /// </summary>
        /// <param name="charUtf32">printable char utf code</param>
        /// <returns>MLResult.Code.Ok if key event was injected.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectChar(char charUtf) => MLResult.Create(InjectCharInternal(charUtf));

        /// <summary>
        /// Sends a key down/pressed event to MLWebView.
        /// </summary>
        /// <param name="keyCode">MLWebView.KeyCode.</param>
        /// <param name="modifierMask">Should be one or combination of MLWebView.EventFlags.</param>
        /// <returns>MLResult.Code.Ok if key event was injected.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectKeyDown(MLWebView.KeyCode keyCode, uint modifierMask) => MLResult.Create(InjectKeyDownInternal(keyCode, modifierMask));

        /// <summary>
        /// Sends a key up/release event to MLWebView.
        /// </summary>
        /// <param name="keyCode">MLWebView.KeyCode.</param>
        /// <param name="modifierMask">Should be one or combination of MLWebView.EventFlags.</param>
        /// <returns>MLResult.Code.Ok if key event was injected.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult InjectKeyUp(MLWebView.KeyCode keyCode, uint modifierMask) => MLResult.Create(InjectKeyUpInternal(keyCode, modifierMask));

        /// <summary>
        /// Triggers a mouse "Scroll" event.
        /// </summary>
        /// <param name="xPixels">The number of pixels to scroll on the x axis.</param>
        /// <param name="yPixels">The number of pixels to scroll on the y axis.</param>
        /// <returns>MLResult.Code.Ok if MLWebView was scrolled.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        public MLResult ScrollBy(uint xPixels, uint yPixels) => MLResult.Create(ScrollByInternal(xPixels, yPixels));

        /// <summary>
        /// Get the entire scrollable size of the webview. 
        /// This should be typically called afer OnLoadEnd to determine the scollable size
        /// of the main frame of the loaded page.Some pages might dynamically resize and this should be called
        /// before each frame draw to correctly determine the scrollable size of the webview.
        /// </summary>
        /// <returns>Vector2Int representing the entire width and height of the webview, in pixels.</returns>
        public Vector2Int GetScrollSize() => GetScrollSizeInternal();

        /// <summary>
        /// Get the scroll offset of the webview.
        /// </summary>
        /// <returns>Vector2Int representing the horizontal and vertical offset of the webview, in pixels.</returns>
        public Vector2Int GetScrollOffset() => GetScrollOffsetInternal();

        /// <summary>
        /// Reset zoom level to 1.0.
        /// </summary>
        /// <returns>MLResult.Code.Ok if MLWebView zoom was reset.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
        public MLResult ResetZoom() => MLResult.Create(ResetZoomInternal());

        /// <summary>
        /// Zoom in one level.
        /// </summary>
        /// <returns>MLResult.Code.Ok if MLWebView zoomed in.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        /// <returns>MLResult.Code.WebViewResultZoomLimitReached if cannot zoom in any further.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
        public MLResult ZoomIn() => MLResult.Create(ZoomInInternal());

        /// <summary>
        /// Zoom out one level.
        /// </summary>
        /// <returns>MLResult.Code.Ok if MLWebView zoomed out.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        /// <returns>MLResult.Code.WebViewResultZoomLimitReached if cannot zoom out any further.</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
        public MLResult ZoomOut() => MLResult.Create(ZoomOutInternal());

        /// <summary>
        /// Get the current zoom factor.  The default zoom factor is 1.0.
        /// </summary>
        /// <returns>Current numeric value for zoom factor.</returns>
        public double GetZoomFactor() => GetZoomFactorInternal();

        /// <summary>
        /// Clear the webview cache.
        /// </summary>
        /// <returns>MLResult.Code.Ok if cache cleared successfully</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if clearing cache failed due to an internal error.</returns>
        public MLResult ClearCache() => MLResult.Create(ClearCacheInternal());

        /// <summary>
        /// Remove all webview cookies.
        /// </summary>
        /// <returns>MLResult.Code.Ok if all cookies removed successfully.</returns>
        /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView .</returns>
        /// <returns>MLResult.Code.UnspecifiedFailure if removing all cookies failed due to an internal error.</returns>
        public MLResult RemoveAllCookies() => MLResult.Create(RemoveAllCookiesInternal());

        /// <summary>
        /// Struct containing data about clicked input field in WebView.
        /// </summary>
        public struct InputFieldData
        {
            /// <summary>
            /// Horizontal position of the input field.
            /// </summary>
            public int X;

            /// <summary>
            /// Vertical position of the input field.
            /// </summary>
            public int Y;

            /// <summary>
            /// Width of the input field.
            /// </summary>
            public int Width;

            /// <summary>
            /// Height of the input field.
            /// </summary>
            public int Height;

            /// <summary>
            /// One or combination of TextInputFlags.
            /// </summary>
            public TextInputFlags TextInputFlags;

            /// <summary>
            /// One of TextInputType.
            /// </summary>
            public TextInputType TextInputType;
        }
    }
}

#endif