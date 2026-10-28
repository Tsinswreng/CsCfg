// 本分片只測 DictCfgAccessor.TryGet: 命中、顯式 null、缺鍵、中途是標量。
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsCfg.Test.Domains.DictCfg;

public partial class TestDictCfg{
	public void RegisterTryGet(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestDictCfg),
			[typeof(DictCfgAccessor)],
			[nameof(DictCfgAccessor.TryGet)],
			"TryGet"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("命中時給值與來源", async (o) => {
			var Cfg = new DictCfgAccessor{ Name = "dict" };
			Cfg.TrySetNoSave(["Ui", "Lang"], "zh-CN");

			var Hit = Cfg.TryGet(["Ui", "Lang"], out var Got, out var Src);

			T(Hit, "應該命中");
			T((str?)Got == "zh-CN", "值應該是 zh-CN");
			T(Src is not null && Src.Name == "dict", "來源應該是這個 accessor 自己");
			return null;
		});

		R("顯式 null 與缺鍵分得開", async (o) => {
			var Cfg = new DictCfgAccessor();
			Cfg.TrySetNoSave(["Ui", "Lang"], null);

			// 鍵存在但值是 null: 回 true
			T(Cfg.TryGet(["Ui", "Lang"], out var Got, out _), "顯式 null 應該算命中");
			T(Got is null, "值應該是 null");

			// 鍵不存在: 回 false, 且來源為 null
			T(!Cfg.TryGet(["Ui", "Missing"], out _, out var NoSrc), "缺鍵應該回 false");
			T(NoSrc is null, "缺鍵時來源應該為 null");
			return null;
		});

		R("中途是標量時回 false", async (o) => {
			var Cfg = new DictCfgAccessor();
			Cfg.TrySetNoSave(["Ui"], 1);

			T(!Cfg.TryGet(["Ui", "Lang"], out _, out _), "Ui 是標量, 往下鑽應該失敗");
			return null;
		});

		R("空路徑回 false", async (o) => {
			var Cfg = new DictCfgAccessor();
			T(!Cfg.TryGet([], out _, out _), "空路徑不是任何鍵");
			return null;
		});
	}
}
