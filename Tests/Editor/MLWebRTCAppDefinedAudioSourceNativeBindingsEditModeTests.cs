using System.Reflection;
using NUnit.Framework;

namespace Tests.Editor
{
    public class MLWebRTCAppDefinedAudioSourceNativeBindingsEditModeTests : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLWebRTC.AppDefinedAudioSource);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }
        
        [Test]
        public void NativeBinding_MLWebRTCSourceAppDefinedAudioSourcePushData_Exists()
        {
            AssertThatMethodExists("MLWebRTCSourceAppDefinedAudioSourcePushData");
        }
    }
}