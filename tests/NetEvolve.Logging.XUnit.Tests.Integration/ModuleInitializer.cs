namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.AutoVerify();

        Verifier.DerivePathInfo(
            (sourceFile, projectDirectory, method, type) =>
            {
                var snapshots = Path.Combine(projectDirectory, "_snapshots");
                _ = Directory.CreateDirectory(snapshots);
                return new(snapshots, type.Name, method.Name);
            }
        );
    }
}
