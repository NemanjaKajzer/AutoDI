using System.Runtime.CompilerServices;

namespace AutoDI.Testing
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() => VerifySourceGenerators.Initialize();
    }
}
