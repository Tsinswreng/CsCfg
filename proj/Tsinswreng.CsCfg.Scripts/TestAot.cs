namespace Tsinswreng.CsCfg.Scripts;

/// 以 win-x64 NativeAOT 發佈 Tsinswreng.CsCfg.Test 並運行發佈物,
/// 驗證剪裁與 AOT 之後測試仍全部通過。
internal static partial class TestAot{
	/// 執行: publish -c Release -r win-x64 → 運行 publish 目錄下的原生測試 exe。
	internal static partial Task Main(ISCtx Ctx, CT Ct);
}
