// 本分片只測 配置樹 → 檔案: 首次存檔不蓋掉既有內容、存檔不留暫存檔、取消令牌的傳遞。
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsCfg;
using Tsinswreng.CsCfg.Json;

namespace Tsinswreng.CsCfg.Test.Domains.FileCfg;

public partial class TestFileCfg{
	public void RegisterFileWrite(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestFileCfg),
			[typeof(JsonFileCfgAccessor)],
			[nameof(JsonFileCfgAccessor.Save)],
			"FileWrite"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("建構時先讀入既有內容, 首次存檔不會蓋掉它", async (o) => {
			var Dir = MkTempDir();
			try{
				var FilePath = System.IO.Path.Combine(Dir, "cfg.rw.jsonc");
				MkFile(FilePath, """{"keep": 1}""");

				var Cfg = new JsonFileCfgAccessor(FilePath, "gui");
				T(Cfg.TryGet(["keep"], out var Keep, out _), "建構時就該讀到既有的鍵");
				T(Keep is i64 K && K == 1, "keep 應該是 i64 1");

				Cfg.TrySetNoSave(["add"], 2);
				Cfg.Save();

				// 只看內存不夠: 真正的風險是「檔案只剩剛寫的那個鍵」
				var Text = ReadFile(FilePath);
				T(Text.Contains("keep"), "存檔後, 原有的鍵不能消失");
				T(Text.Contains("add"), "存檔後, 新寫的鍵要在檔案裡");
				return null;
			}
			finally{
				RmTempDir(Dir);
			}
		});

		R("存檔之後不留暫存檔", async (o) => {
			var Dir = MkTempDir();
			try{
				var FilePath = System.IO.Path.Combine(Dir, "cfg.rw.jsonc");
				var Cfg = new JsonFileCfgAccessor(FilePath, "gui");
				Cfg.TrySetNoSave(["x"], 1);
				await Cfg.Save(default);

				T(File.Exists(FilePath), "正式檔應該存在");
				T(!File.Exists(FilePath + ".tmp"), "搬移完成後不該留下 .tmp");
				return null;
			}
			finally{
				RmTempDir(Dir);
			}
		});

		R("存檔後新開一個源讀得回同樣的值", async (o) => {
			var Dir = MkTempDir();
			try{
				var FilePath = System.IO.Path.Combine(Dir, "cfg.rw.jsonc");
				var Cfg = new JsonFileCfgAccessor(FilePath, "gui");
				Cfg.TrySetNoSave(["Lang"], "en");
				await Cfg.Save(default);

				var Again = new JsonFileCfgReader(FilePath, "file");
				T(Again.TryGet(["Lang"], out var Lang, out _), "應該讀得回來");
				T((str?)Lang == "en", "值應該不變");
				return null;
			}
			finally{
				RmTempDir(Dir);
			}
		});

		R("兩次讀取之間檔案被改掉, 重讀讀到新內容", async (o) => {
			var Dir = MkTempDir();
			try{
				var FilePath = System.IO.Path.Combine(Dir, "cfg.jsonc");
				MkFile(FilePath, """{"Lang": "en"}""");
				var Cfg = new JsonFileCfgAccessor(FilePath, "gui");
				T(Cfg.TryGet(["Lang"], out var First, out _), "第一次應該讀得到");
				T((str?)First == "en", "第一次是 en");

				// 模擬別的程式把這份檔案換掉
				MkFile(FilePath, """{"Lang": "zh-CN"}""");
				Cfg.Reload();

				T(Cfg.TryGet(["Lang"], out var Second, out _), "重讀之後應該還有 Lang");
				// 這一條測的是「每次讀取都重新開檔」; 若只握著建構時那條流, 這裡會還是 en
				T((str?)Second == "zh-CN", "重讀應該看到新內容");
				return null;
			}
			finally{
				RmTempDir(Dir);
			}
		});

		R("保存失敗時正式檔保持原樣", async (o) => {
			var Dir = MkTempDir();
			try{
				var FilePath = System.IO.Path.Combine(Dir, "cfg.rw.jsonc");
				MkFile(FilePath, """{"keep": 1}""");
				var Cfg = new JsonFileCfgAccessor(FilePath, "gui");

				// 用同名目錄佔住暫存檔路徑, 讓「開暫存檔」這一步失敗
				Directory.CreateDirectory(FilePath + ".tmp");

				var Threw = false;
				try{
					Cfg.Save();
				}catch(IOException){
					Threw = true;
				}catch(UnauthorizedAccessException){
					Threw = true;
				}

				T(Threw, "開不了暫存檔應該拋");
				// 原子寫的意義就在這裡: 失敗也不能動到正式檔
				T(ReadFile(FilePath).Contains("keep"), "正式檔應該保持原樣");
				return null;
			}
			finally{
				RmTempDir(Dir);
			}
		});

		R("Save(CT) 的取消令牌會傳到 FnSaveAsy", async (o) => {
			// 這一條測的是庫的契約, 不是檔案源: 先前 Save(CT) 把令牌丟掉, 取消對寫盤完全無效
			var Cfg = new DictCfgAccessor{ Name = "mem" };
			CT Seen = default;
			Cfg.FnSaveAsy = (Dict, Ct) => {
				Seen = Ct;
				return Task.FromResult<nil>(NIL);
			};

			using var Cts = new CancellationTokenSource();
			await Cfg.Save(Cts.Token);

			T(Seen == Cts.Token, "Save 收到的令牌應該原樣傳給 FnSaveAsy");
			return null;
		});
	}
}
