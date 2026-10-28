// 本分片只測 DictCfgAccessor.TrySetNoSave: 建中間節點、型別衝突不覆蓋、空路徑。
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsCfg.Test.Domains.DictCfg;

public partial class TestDictCfg{
	public void RegisterTrySet(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestDictCfg),
			[typeof(DictCfgAccessor)],
			[nameof(DictCfgAccessor.TrySetNoSave)],
			"TrySet"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("缺中間節點時自動建", async (o) => {
			var Cfg = new DictCfgAccessor();
			// 值模型只管存, 存進去是甚麼型別讀出來就是甚麼型別; 這裡刻意用 i64 以便精確斷言
			T(Cfg.TrySetNoSave(["A", "B", "C"], (i64)7), "應該寫成功");
			T(Cfg.TryGet(["A", "B", "C"], out var Got, out _), "建完應該讀得回來");
			T(Got is i64 L && L == 7, "值應該是 i64 7");
			return null;
		});

		R("中間節點是標量時回 false 且不改動", async (o) => {
			var Cfg = new DictCfgAccessor();
			Cfg.TrySetNoSave(["Ui"], (i64)1);

			T(!Cfg.TrySetNoSave(["Ui", "Lang"], "zh-CN"), "中間是標量應該失敗");

			// 失敗之後原本的標量必須原封不動
			T(Cfg.TryGet(["Ui"], out var Got, out _), "Ui 應該還在");
			T(Got is i64 L && L == 1, "Ui 仍然是原本的 1");
			return null;
		});

		R("空路徑回 false", async (o) => {
			var Cfg = new DictCfgAccessor();
			T(!Cfg.TrySetNoSave([], 1), "空路徑不該寫成功");
			return null;
		});
	}
}
