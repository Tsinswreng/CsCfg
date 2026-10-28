using Tsinswreng.CsSh;
using static Tsinswreng.CsSh.ShGlobal;

namespace Tsinswreng.CsCfg.Scripts;

/// TestAot 命令的流程實現; NativeAOT 開關由 Directory.Build.props 統一提供。
internal static partial class TestAot{
	internal static async partial Task Main(ISCtx Ctx, CT Ct){
		const str Rid = "win-x64";
		var TestProject = Ctx.RootDir/"proj/Tsinswreng.CsCfg.Test/Tsinswreng.CsCfg.Test.csproj";

		// step 1: 以 Release 配置發佈 NativeAOT 測試 executable
		await Exe("dotnet", ["publish", TestProject, "-c", "Release", "-r", Rid], Ct);

		// step 2: 運行 publish 目錄中的原生測試程式; 目錄結構固定為 bin/Release/{Tfm}/{Rid}/publish
		var PublishExe = Ctx.RootDir/"proj/Tsinswreng.CsCfg.Test/bin/Release"/Ctx.Tfm/Rid/"publish/Tsinswreng.CsCfg.Test.exe";
		await Exe(PublishExe, [], Ct);
	}
}
