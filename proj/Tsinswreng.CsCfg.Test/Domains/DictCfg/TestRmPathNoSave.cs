// 本分片只測 DictCfgAccessor.RmPathNoSave: 刪鍵、由內而外清空父節點、根不清理。
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsCfg.Test.Domains.DictCfg;

public partial class TestDictCfg{
	public void RegisterRm(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestDictCfg),
			[typeof(DictCfgAccessor)],
			[nameof(DictCfgAccessor.RmPathNoSave)],
			"Rm"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("刪鍵之後連空的父節點一起清掉", async (o) => {
			var Cfg = new DictCfgAccessor();
			Cfg.TrySetNoSave(["A", "B", "C"], 1);

			T(Cfg.RmPathNoSave(["A", "B", "C"]), "應該刪成功");
			T(!Cfg.TryGet(["A", "B", "C"], out _, out _), "葉子應該沒了");
			// A 底下只剩 B, B 空掉之後 A 也該被摘掉
			T(!Cfg.TryGet(["A"], out _, out _), "空的父節點 A 也應該被清掉");
			return null;
		});

		R("兄弟節點還在時不清父節點", async (o) => {
			var Cfg = new DictCfgAccessor();
			Cfg.TrySetNoSave(["A", "B"], (i64)1);
			Cfg.TrySetNoSave(["A", "C"], (i64)2);

			T(Cfg.RmPathNoSave(["A", "B"]), "應該刪成功");
			T(!Cfg.TryGet(["A", "B"], out _, out _), "B 應該沒了");
			T(Cfg.TryGet(["A", "C"], out var Got, out _), "A 還有一個兄弟, 不該被清");
			T(Got is i64 L && L == 2, "C 仍然是 2");
			return null;
		});

		R("根不清理", async (o) => {
			var Cfg = new DictCfgAccessor();
			Cfg.TrySetNoSave(["Only"], 1);

			T(Cfg.RmPathNoSave(["Only"]), "應該刪成功");
			T(Cfg.Dict.Count == 0, "根應該空了");
			T(Cfg.Dict is not null, "但根本身不該被清掉");
			return null;
		});

		R("缺鍵回 false", async (o) => {
			var Cfg = new DictCfgAccessor();
			T(!Cfg.RmPathNoSave(["NoSuchKey"]), "刪不存在的鍵不該成功");
			return null;
		});
	}
}
