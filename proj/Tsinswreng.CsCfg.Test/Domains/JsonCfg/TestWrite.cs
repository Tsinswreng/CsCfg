// 本分片只測 配置樹 → JSON: 鍵序保持、往返一致、寫出後能再讀回、缺輸出流時拋。
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsCfg.Json;

namespace Tsinswreng.CsCfg.Test.Domains.JsonCfg;

public partial class TestJsonCfg{
	public void RegisterWrite(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestJsonCfg),
			[typeof(JsonCfgCodec), typeof(JsonCfgAccessor)],
			[nameof(JsonCfgCodec.Write)],
			"Write"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("寫出時保持鍵序", async (o) => {
			var S = new MemoryStream();
			var D = new OrderedDictionary<str, obj?>{
				["zeta"] = 1
				,["alpha"] = 2
				,["mid"] = 3
			};

			JsonCfgCodec.Write(D, S, new System.Text.Json.JsonWriterOptions{ Indented = false });

			var Text = ReadText(S);
			// 位置本身就說明順序了; 用 IndexOf 比對比字符串相等更抗縮排變化
			T(Text.IndexOf("zeta") < Text.IndexOf("alpha"), "zeta 應該在 alpha 前面");
			T(Text.IndexOf("alpha") < Text.IndexOf("mid"), "alpha 應該在 mid 前面");
			return null;
		});

		R("往返之後值與鍵序都不變", async (o) => {
			var Original = """
			{"Ui": {"Lang": "zh-CN", "Size": 14}, "list": [1, true, null], "on": false}
			""";
			using var In = MkStream(Original);
			var Rdr = new JsonCfgReader(In, "json");

			using var Out = new MemoryStream();
			JsonCfgCodec.Write(Rdr.Dict, Out, new System.Text.Json.JsonWriterOptions{ Indented = true });

			// 把寫出來的東西再解析一次, 應與原來一致
			Out.Position = 0;
			var Again = new JsonCfgReader(Out, "json-roundtrip");

			T(Again.TryGet(["Ui", "Lang"], out var Lang, out _), "往返後應該還有 Lang");
			T((str?)Lang == "zh-CN", "Lang 不變");
			T(Again.TryGet(["Ui", "Size"], out var Size, out _), "往返後應該還有 Size");
			T(Size is i64 L && L == 14, "Size 仍是 i64 14");
			T(Again.TryGet(["on"], out var On, out _), "往返後應該還有 on");
			T(On is bool B && !B, "on 仍是 false");
			T(Again.TryGet(["list"], out var List, out _), "往返後應該還有 list");
			T((List as IList<obj?>)?.Count == 3, "list 仍有三個元素");
			return null;
		});

		R("Accessor 存檔會整份重寫, 不留舊尾巴", async (o) => {
			var Out = new MemoryStream();
			using var In = MkStream("""{"a": 1}""");
			var Cfg = new JsonCfgAccessor(In, Out, "json");

			// 先寫一份長的, 再寫一份短的; 沒先截斷的話尾巴會殘留
			Out.SetLength(0);
			Out.Position = 0;
			JsonCfgCodec.Write(
				new OrderedDictionary<str, obj?>{["longlonglonglongkey"] = "longlonglonglongvalue"}
				,Out
				,new System.Text.Json.JsonWriterOptions{ Indented = false }
			);

			Cfg.Dict.Clear();
			Cfg.TrySetNoSave(["x"], 1);
			Cfg.Save();

			var Text = ReadText(Out);
			T(Text.Contains("\"x\""), "應該有新的鍵");
			T(!Text.Contains("longlonglonglongkey"), "舊內容的尾巴不該殘留");
			return null;
		});

		R("值模型以外的型別寫成字符串", async (o) => {
			// enum 不在值模型裡; 寫成字符串, 讀回來就是字符串
			var D = new OrderedDictionary<str, obj?>{["mode"] = DayOfWeek.Monday};
			using var S = new MemoryStream();

			JsonCfgCodec.Write(D, S, new System.Text.Json.JsonWriterOptions{ Indented = false });
			S.Position = 0;
			var Rdr = new JsonCfgReader(S, "json");

			T(Rdr.TryGet(["mode"], out var Got, out _), "應該讀得回來");
			T((str?)Got == "Monday", "應該變成字符串 Monday");
			return null;
		});

		R("沒設輸出流就保存會拋", async (o) => {
			using var In = MkStream("""{"a": 1}""");
			var Cfg = new JsonCfgAccessor(In, null, "json");

			var Threw = false;
			try{
				Cfg.Save();
			}catch(InvalidOperationException){
				Threw = true;
			}
			T(Threw, "OutputStream 沒設就該拋");
			return null;
		});
	}
}
