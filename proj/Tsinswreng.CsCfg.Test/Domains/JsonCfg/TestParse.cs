// 本分片只測 JSON → 配置樹: 註釋與尾逗號、各種標量型別、數組、根不是對象。
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsCfg.Json;

namespace Tsinswreng.CsCfg.Test.Domains.JsonCfg;

public partial class TestJsonCfg{
	public void RegisterParse(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestJsonCfg),
			[typeof(JsonCfgCodec), typeof(JsonCfgReader)],
			[nameof(JsonCfgCodec.ParseObject)],
			"Parse"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("允許註釋與尾逗號", async (o) => {
			// 出廠配置是人手寫的, 所以默認要吃得下這兩種寫法
			var Text = """
			{
				// 界面語言
				"lang": "zh-CN",
				"theme": "dark",
			}
			""";
			var Rdr = new JsonCfgReader(MkStream(Text), "jsonc");

			T(Rdr.TryGet(["lang"], out var Lang, out _), "應該讀得到 lang");
			T((str?)Lang == "zh-CN", "lang 應該是 zh-CN");
			T(Rdr.TryGet(["theme"], out var Theme, out _), "應該讀得到 theme");
			T((str?)Theme == "dark", "theme 應該是 dark");
			return null;
		});

		R("數字先當 i64, 裝不下再當 f64", async (o) => {
			var Rdr = new JsonCfgReader(MkStream("""{"num": 42, "real": 1.5}"""), "json");

			T(Rdr.TryGet(["num"], out var Num, out _), "應該讀得到 num");
			T(Num is i64 L && L == 42, "整數應該是 i64 42");
			T(Rdr.TryGet(["real"], out var Real, out _), "應該讀得到 real");
			T(Real is f64 D && D == 1.5, "小數應該是 f64 1.5");
			return null;
		});

		R("布爾、字符串、null 各歸各位", async (o) => {
			var Rdr = new JsonCfgReader(
				MkStream("""{"on": true, "off": false, "name": "ngan", "none": null}""")
				,"json"
			);

			T(Rdr.TryGet(["on"], out var On, out _), "應該讀得到 on");
			T(On is bool B1 && B1, "on 應該是 true");
			T(Rdr.TryGet(["off"], out var Off, out _), "應該讀得到 off");
			T(Off is bool B2 && !B2, "off 應該是 false");
			T(Rdr.TryGet(["name"], out var Name, out _), "應該讀得到 name");
			T((str?)Name == "ngan", "name 應該是 ngan");
			// null 也是值: 命中但值為 null
			T(Rdr.TryGet(["none"], out var None, out _), "顯式 null 應該算命中");
			T(None is null, "none 應該是 null");
			return null;
		});

		R("數組解析成 IList, 元素同樣走值模型", async (o) => {
			var Rdr = new JsonCfgReader(
				MkStream("""{"list": [1, "two", false, null, {"k": 3}]}""")
				,"json"
			);

			T(Rdr.TryGet(["list"], out var Got, out _), "應該讀得到 list");
			var List = Got as IList<obj?>;
			T(List is not null, "list 應該是 IList<obj?>");
			T(List!.Count == 5, "應該有五個元素");
			T(List[0] is i64 I && I == 1, "第一個是 i64 1");
			T((str?)List[1] == "two", "第二個是字符串 two");
			T(List[2] is bool B && !B, "第三個是 false");
			T(List[3] is null, "第四個是 null");
			T(List[4] is IDictionary<str, obj?>, "第五個是嵌套對象");
			// 上一行只是斷言, 編譯器不會因此認為型別已收窄, 所以這裡再轉一次
			var Inner = (IDictionary<str, obj?>)List[4]!;
			T(Inner["k"] is i64 K && K == 3, "嵌套對象的 k 是 i64 3");
			return null;
		});

		R("嵌套對象可以按路徑下鑽", async (o) => {
			var Rdr = new JsonCfgReader(
				MkStream("""{"Ui": {"Font": {"Family": "Mono", "Size": 14}}}""")
				,"json"
			);

			T(Rdr.TryGet(["Ui", "Font", "Family"], out var Family, out _), "應該讀得到深層的鍵");
			T((str?)Family == "Mono", "Family 應該是 Mono");
			// 中間段是對象而不是鍵, 所以這條路徑不存在
			T(!Rdr.TryGet(["Ui", "Font", "Family", "Deeper"], out _, out _), "鑽到標量以下應該失敗");
			return null;
		});

		R("根不是對象時拋 InvalidDataException", async (o) => {
			var Threw = false;
			try{
				// 數組沒辦法用路徑尋址, 直接當成配置是錯的
				_ = new JsonCfgReader(MkStream("[1, 2, 3]"), "json");
			}catch(InvalidDataException){
				Threw = true;
			}
			T(Threw, "根為數組應該拋 InvalidDataException");
			return null;
		});

		R("重讀會回到流開頭再解析一次", async (o) => {
			var S = MkStream("""{"v": 1}""");
			var Rdr = new JsonCfgReader(S, "json");
			T(Rdr.TryGet(["v"], out var First, out _), "第一次應該讀到 v");
			T(First is i64 A && A == 1, "v 應該是 1");

			// 換掉流的內容, 模擬檔案被人改過
			S.SetLength(0);
			var Bytes = System.Text.Encoding.UTF8.GetBytes("""{"v": 2}""");
			S.Write(Bytes, 0, Bytes.Length);
			S.Position = 0;

			Rdr.Reload();

			T(Rdr.TryGet(["v"], out var Second, out _), "重讀之後應該還有 v");
			T(Second is i64 B && B == 2, "v 應該變成 2");
			return null;
		});

		R("沒設流就載入會拋", async (o) => {
			var Rdr = new JsonCfgReader(null, "json");
			var Threw = false;
			try{
				Rdr.Load();
			}catch(InvalidOperationException){
				Threw = true;
			}
			T(Threw, "InputStream 沒設就該拋");
			return null;
		});
	}
}
