// 本分片只測事件: Before 伴隨失敗觸發、After 只在成功時觸發、Path 與 Src 的內容。
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsCfg.Test.Domains.DictCfg;

public partial class TestDictCfg{
	public void RegisterEvents(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestDictCfg),
			[typeof(DictCfgAccessor)],
			[nameof(DictCfgAccessor.TrySetNoSave)],
			"Events"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("寫值成功時 Before 與 After 都帶路徑與源", async (o) => {
			var Cfg = new DictCfgAccessor{ Name = "dict" };
			// 兩個事件記到不同變數, 事後可分開檢查
			ICfgChange? Before = null;
			ICfgChange? After = null;
			Cfg.BeforeSetByPath += (S, E) => Before = E;
			Cfg.AfterSetByPath += (S, E) => After = E;

			T(Cfg.TrySetNoSave(["Ui", "Lang"], "zh-CN"), "應該寫成功");

			T(Before is not null, "Before 應該觸發");
			T(After is not null, "After 應該觸發");
			T(Before!.Path is not null && Before.Path.Count == 2, "Before 的路徑應該有兩段");
			T(After!.Path is not null && After.Path[1] == "Lang", "After 的路徑末段應該是 Lang");
			T(ReferenceEquals(After.Src, Cfg), "After 的來源應該是這個 accessor");
			return null;
		});

		R("寫值失敗時只觸發 Before", async (o) => {
			var Cfg = new DictCfgAccessor();
			Cfg.TrySetNoSave(["Ui"], 1);
			var BeforeCount = 0;
			var AfterCount = 0;
			Cfg.BeforeSetByPath += (S, E) => BeforeCount++;
			Cfg.AfterSetByPath += (S, E) => AfterCount++;

			// 中間節點是標量, 這次寫入必然失敗
			T(!Cfg.TrySetNoSave(["Ui", "Lang"], "zh-CN"), "應該寫失敗");

			T(BeforeCount == 1, "確實嘗試過了, Before 應該觸發一次");
			T(AfterCount == 0, "失敗不該觸發 After");
			return null;
		});

		R("刪鍵失敗時只觸發 Before", async (o) => {
			var Cfg = new DictCfgAccessor();
			var BeforeCount = 0;
			var AfterCount = 0;
			Cfg.BeforeRmPath += (S, E) => BeforeCount++;
			Cfg.AfterRmPath += (S, E) => AfterCount++;

			T(!Cfg.RmPathNoSave(["NoSuchKey"]), "刪不存在的鍵應該失敗");

			T(BeforeCount == 1, "Before 應該觸發一次");
			T(AfterCount == 0, "沒刪到東西就不算變更, 不該觸發 After");
			return null;
		});

		R("重讀時 Path 為 null 表示整體替換", async (o) => {
			var Cfg = new DictCfgAccessor();
			Cfg.FnReload = ()=>{
				var D = new OrderedDictionary<str, obj?>();
				D["Ui"] = "reloaded";
				return D;
			};
			ICfgChange? Before = null;
			ICfgChange? After = null;
			Cfg.BeforeReload += (S, E) => Before = E;
			Cfg.AfterReload += (S, E) => After = E;

			Cfg.Reload();

			T(Before is not null && Before.Path is null, "Before 的 Path 應該是 null");
			T(After is not null && After.Path is null, "After 的 Path 應該是 null");
			T(Cfg.TryGet(["Ui"], out var Got, out _), "重讀之後應該讀得到新值");
			T((str?)Got == "reloaded", "值應該來自 FnReload");
			return null;
		});

		R("落盤失敗時只觸發 Before", async (o) => {
			var Cfg = new DictCfgAccessor();
			Cfg.FnSave = (D)=> throw new IOException("disk full");
			var BeforeCount = 0;
			var AfterCount = 0;
			Cfg.BeforeSave += (S, E) => BeforeCount++;
			Cfg.AfterSave += (S, E) => AfterCount++;

			// 這裏就是要它拋, 所以自己接住
			var Threw = false;
			try{
				Cfg.Save();
			}catch(IOException){
				Threw = true;
			}

			T(Threw, "FnSave 拋的話 Save 應該跟着拋");
			T(BeforeCount == 1, "Before 應該觸發一次");
			T(AfterCount == 0, "沒寫成就沒有 After");
			return null;
		});

		R("未設 FnSave 時直接拋且不觸發事件", async (o) => {
			var Cfg = new DictCfgAccessor();
			var BeforeCount = 0;
			Cfg.BeforeSave += (S, E) => BeforeCount++;

			var Threw = false;
			try{
				Cfg.Save();
			}catch(InvalidOperationException){
				Threw = true;
			}

			T(Threw, "FnSave 沒設就該拋");
			T(BeforeCount == 0, "連嘗試都沒開始, 不該觸發 Before");
			return null;
		});

		R("訂閱者收到的是同一個源的實例", async (o) => {
			var Cfg = new DictCfgAccessor{ Name = "the-src" };
			ICfgReader? Got = null;
			Cfg.AfterSetByPath += (S, E) => Got = E.Src;

			Cfg.TrySetNoSave(["K"], 1);

			T(Got is not null && Got.Name == "the-src", "事件裡的源應該就是寫入的那個源");
			return null;
		});
	}
}
