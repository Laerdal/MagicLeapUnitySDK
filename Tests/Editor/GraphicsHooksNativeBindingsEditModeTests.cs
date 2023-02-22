using System.Reflection;
using NUnit.Framework;

namespace Tests.Editor
{
    public class GraphicsHooksNativeBindingsEditModeTests : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLGraphicsHooks);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLUnityGraphicsGetHandle_Exists()
        {
            AssertThatMethodExists("MLUnityGraphicsGetHandle");
        }

        [Test]
        public void NativeBinding_MLUnityGraphicsRegisterCallbacks_Exists()
        {
            AssertThatMethodExists("MLUnityGraphicsRegisterCallbacks");
        }
    }
}