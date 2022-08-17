// %BANNER_BEGIN% 
// --------------------------------------------------------------------- 
// %COPYRIGHT_BEGIN%
// <copyright file="MLMarkerTracker.cs" company="Magic Leap">
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
    using System.Threading.Tasks;

    /// <summary>
    ///     This API can be used to scan markers. For QR codes it also provides 6DOF poses. The
    ///     scanner supports up to 16 markers. Identical markers will be treated as seperate
    ///     markers and reported individually. 
    ///     List of currently supported trackable markers (with pose info):
    ///     - QR codes of Model 1 and Model 2
    ///     - ArUco markers
    ///     List of currently supported detectable markers (without pose info):
    ///     - EAN-13 (experimental)
    ///     - UPC-A (experimental)
    /// </summary>
    public sealed partial class MLMarkerTracker : MLAutoAPISingleton<MLMarkerTracker>
    {
        /// <summary>
        ///     When any results are found from marker scanning, this event will be raised.
        /// </summary>
        public static event Action<MarkerData> OnMLMarkerTrackerResultsFound;

        /// <summary>
        ///     When any results are found from marker scanning, this event will be raised with an array of all markers found.
        /// </summary>
        public static event Action<MarkerData[]> OnMLMarkerTrackerResultsFoundArray;

        /// <summary>
        ///     A cache of the last requested settings value. Any new requested value will be
        ///     checked against this to verify that an update is needed.
        /// </summary>
        private static Settings futureSettingsValue;

        /// <summary>
        ///     Instance.settings setter.
        ///     If called with the same value while a settings update operation is in progress,
        ///     nothing will happen.
        /// </summary>
        public static async Task SetSettingsAsync(Settings value)
        {
            if (futureSettingsValue.Equals(value))
                return;

            futureSettingsValue = value;
            var resultCode = (await MLMarkerTrackerSettingsUpdate(value)).Result;
            if (MLResult.IsOK(resultCode))
                Instance.settings = value;
        }

        private static bool IsScanning => Instance.settings.EnableMarkerScanning;

        private MLMarkerTracker.Settings settings = Settings.Create(true, MarkerType.All);
        
        /// <summary>
        ///     Asynchronous utility method to enable marker scanning using the current <c> ScannerSettings </c>. 
        ///     Does nothing if scanning is already enabled.
        ///     Note that enabling scanning has a performance cost until scanning is disabled using 
        ///     <c> StopScanning </c> or by setting <c> ScannerSettings.enabled </c> to <c>false</c>.
        /// </summary>
        public static async Task StartScanningAsync(Settings? settings = null)
        {
            settings ??= Instance.settings;

            if (Instance.settings.EnableMarkerScanning == true)
                return;

            await SetSettingsAsync(Settings.Create(true, Instance.settings.MarkerTypes, Instance.settings.QRCodeSize, Instance.settings.ArucoDicitonary, Instance.settings.ArucoMarkerSize));
        }

        /// <summary>
        ///     Asynchronous method to disable marker scanning if previously activated. 
        ///     Otherwise, this does nothing.
        /// </summary>
        public async static Task StopScanningAsync()
        {
            // check future settings instead of current settings because this is asynchronous
            if (!IsStarted || futureSettingsValue.EnableMarkerScanning == false)
                return;

            await SetSettingsAsync(Settings.Create(false, Instance.settings.MarkerTypes, Instance.settings.QRCodeSize, Instance.settings.ArucoDicitonary, Instance.settings.ArucoMarkerSize));
        }

        protected override MLResult.Code StopAPI()
        {
            if (IsScanning)
                Task.Run(StopScanningAsync).Wait();

            return NativeBindings.MLMarkerTrackerDestroy(Handle);
        }

        protected override MLResult.Code StartAPI() => MLMarkerTrackerCreate(Instance.settings);

        /// <summary>
        ///     Runs once per Unity Update loop.
        /// </summary>
        protected override void Update()
        {
            if (IsScanning)
            {
                var results = MLMarkerTrackerGetResults();
                foreach(var result in results)
                    OnMLMarkerTrackerResultsFound?.Invoke(result);

                OnMLMarkerTrackerResultsFoundArray?.Invoke(results);
            }
        }    
    }
}

#endif