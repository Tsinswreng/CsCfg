// 本分片只測 LayeredCfgAccessor 的取值與寫入路由: 前優先、null 遮蔽、來源回報、無可寫源。
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsCfg.Test.Domains.LayeredCfg;

public partial class TestLayeredCfg{
	public void RegisterResolve(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestLayeredCfg),
			[typeof(LayeredCfgAccessor)],
			[nameof(LayeredCfgAccessor.TryGet)],
			"Resolve"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("同一個鍵取第一個命中的源", async (o) => {
			var Top = new DictCfgAccessor{ Name = "top" };
			var Bottom = new DictCfgAccessor{ Name = "bottom" };
			Top.TrySetNoSave(["Ui", "Lang"], "zh-CN");
			Bottom.TrySetNoSave(["Ui", "Lang"], "en-US");

			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(Top);
			Layered.AddSrc(Bottom);

			T(Layered.TryGet(["Ui", "Lang"], out var Got, out var Src), "應該命中");
			T((str?)Got == "zh-CN", "應該取到前一個源的值");
			T(Src is not null && Src.Name == "top", "來源應該是最前面的 top");
			return null;
		});

		R("不同源可以各供一部分子鍵", async (o) => {
			var Top = new DictCfgAccessor{ Name = "top" };
			var Bottom = new DictCfgAccessor{ Name = "bottom" };
			Top.TrySetNoSave(["Ui", "Lang"], "zh-CN");
			Bottom.TrySetNoSave(["Ui", "Theme"], "dark");

			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(Top);
			Layered.AddSrc(Bottom);

			// 這條路徑兩邊都沒有, 但子鍵是鍵級別疊加的
			T(Layered.TryGet(["Ui", "Lang"], out var Lang, out _), "Lang 應該取得到");
			T((str?)Lang == "zh-CN", "Lang 來自 top");
			T(Layered.TryGet(["Ui", "Theme"], out var Theme, out _), "Theme 應該取得到");
			T((str?)Theme == "dark", "Theme 來自 bottom");
			return null;
		});

		R("上層的 null 會遮蔽下層的值", async (o) => {
			var Top = new DictCfgAccessor{ Name = "top" };
			var Bottom = new DictCfgAccessor{ Name = "bottom" };
			Top.TrySetNoSave(["Ui", "Lang"], null);
			Bottom.TrySetNoSave(["Ui", "Lang"], "en-US");

			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(Top);
			Layered.AddSrc(Bottom);

			T(Layered.TryGet(["Ui", "Lang"], out var Got, out var Src), "null 也是值, 應該命中");
			T(Got is null, "取到的應該是那個 null");
			T(Src is not null && Src.Name == "top", "來源應該是寫了 null 的 top");
			return null;
		});

		R("全都沒有時回 false 且來源為 null", async (o) => {
			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(new DictCfgAccessor{ Name = "a" });

			T(!Layered.TryGet(["No", "Such"], out var Got, out var Src), "不該命中");
			T(Got is null, "值應該是 null");
			T(Src is null, "來源應該是 null");
			return null;
		});

		R("沒有源時回 false", async (o) => {
			var Layered = new LayeredCfgAccessor();
			T(!Layered.TryGet(["K"], out _, out _), "空源清單不該命中");
			return null;
		});

		R("寫入只落可寫源", async (o) => {
			var Top = new DictCfgAccessor{ Name = "top" };
			var Bottom = new DictCfgAccessor{ Name = "bottom" };

			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(Top);
			Layered.AddSrc(Bottom);
			Layered.Writable = Bottom;

			T(Layered.TrySetNoSave(["K"], (i64)1), "應該寫成功");
			T(Bottom.TryGet(["K"], out var Got, out _), "值應該落在 bottom");
			T(Got is i64 L && L == 1, "值應該是 1");
			T(!Top.TryGet(["K"], out _, out _), "top 不該被寫入");
			return null;
		});

		R("沒有可寫源時寫入與刪除都回 false", async (o) => {
			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(new DictCfgAccessor());

			T(!Layered.TrySetNoSave(["K"], 1), "沒有可寫源就寫不進去");
			T(!Layered.RmPathNoSave(["K"]), "沒有可寫源就刪不掉");
			// 沒有可寫源時 Save 什麼都不做, 不拋
			Layered.Save();
			return null;
		});
	}
}
