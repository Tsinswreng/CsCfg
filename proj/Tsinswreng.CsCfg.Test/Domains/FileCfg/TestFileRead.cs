// 本分片只測 檔案 → 配置樹: 缺檔、缺目錄、含註釋、路徑正規化、空路徑。
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsCfg.Json;

namespace Tsinswreng.CsCfg.Test.Domains.FileCfg;

public partial class TestFileCfg{
	public void RegisterFileRead(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestFileCfg),
			[typeof(JsonFileCfgReader)],
			[nameof(JsonFileCfgReader.Load)],
			"FileRead"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("檔案不存在時是空源, 重讀也不拋", async (o) => {
			var Dir = MkTempDir();
			try{
				var Ro = new JsonFileCfgReader(System.IO.Path.Combine(Dir, "absent.jsonc"), "file");

				T(Ro.Dict.Count == 0, "缺檔應該是空源");
				T(System.IO.Path.IsPathRooted(Ro.Path), "Path 應該被正規化成絕對路徑");

				// 重讀同樣是「沒有內容」而不是錯誤(首次啟動就是這種情形)
				Ro.Reload();
				T(Ro.Dict.Count == 0, "重讀缺檔仍是空源");
				return null;
			}
			finally{
				RmTempDir(Dir);
			}
		});

		R("所在目錄整個不存在時也是空源", async (o) => {
			var Dir = MkTempDir();
			try{
				// 連目錄都沒建: FileStream 會拋 DirectoryNotFoundException, 而那是「首次啟動」的正常情形
				var Ro = new JsonFileCfgReader(
					System.IO.Path.Combine(Dir, "no-such-sub-dir", "absent.jsonc")
					,"file"
				);
				T(Ro.Dict.Count == 0, "缺目錄不該拋, 也不該有內容");
				return null;
			}
			finally{
				RmTempDir(Dir);
			}
		});

		R("讀得到既有檔案, 且允許註釋與尾逗號", async (o) => {
			var Dir = MkTempDir();
			try{
				var FilePath = System.IO.Path.Combine(Dir, "cfg.jsonc");
				MkFile(FilePath, """
				{
					// 人手寫的那份
					"lang": "zh-CN",
					"nested": { "size": 14 },
				}
				""");

				var Ro = new JsonFileCfgReader(FilePath, "file");

				T(Ro.TryGet(["lang"], out var Lang, out _), "應該讀得到 lang");
				T((str?)Lang == "zh-CN", "lang 應該是 zh-CN");
				T(Ro.TryGet(["nested", "size"], out var Size, out _), "應該讀得到嵌套鍵");
				T(Size is i64 L && L == 14, "size 應該是 i64 14");
				return null;
			}
			finally{
				RmTempDir(Dir);
			}
		});

		R("空路徑在建構時就拋", async (o) => {
			var Threw = false;
			try{
				_ = new JsonFileCfgReader("   ", "file");
			}catch(ArgumentException){
				Threw = true;
			}
			T(Threw, "路徑是空串應該是宣告違約, 直接拋");
			return null;
		});

		R("吃得下帶 BOM 的檔案", async (o) => {
			var Dir = MkTempDir();
			try{
				var FilePath = System.IO.Path.Combine(Dir, "cfg.jsonc");
				// 有些編輯器存 UTF-8 時會加 BOM; 那是編輯器的習慣, 不該變成使用者的錯誤
				MkFileWithBom(FilePath, """{"lang": "en"}""");

				var Ro = new JsonFileCfgReader(FilePath, "file");
				T(Ro.TryGet(["lang"], out var Lang, out _), "帶 BOM 的檔案應該照樣讀得進來");
				T((str?)Lang == "en", "lang 應該是 en");
				return null;
			}
			finally{
				RmTempDir(Dir);
			}
		});
	}
}
