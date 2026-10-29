// 本分片只測 SetRoRw: 只讀優先、寫入只落可寫源、同一物件與 null 的拒絕。
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsCfg;

namespace Tsinswreng.CsCfg.Test.Domains.RoRw;

public partial class TestRoRw{
	public void RegisterSetRoRw(ITestNode Node){
		var register = Node.MkTestFnRegister(
			typeof(TestRoRw),
			[typeof(ILayeredCfgAccessorExtn)],
			[nameof(ILayeredCfgAccessorExtn.SetRoRw)],
			"SetRoRw"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("兩邊都有同一個鍵時, 只讀那份贏", async (o) => {
			var Ro = MkRo(("Lang", "ro"), ("OnlyRo", 1L));
			var Rw = MkRw(("Lang", "rw"));
			var F = new LayeredCfgAccessor();

			F.SetRoRw([Ro], Rw);

			T(F.TryGet(["Lang"], out var Lang, out var Src), "應該取得到 Lang");
			T((str?)Lang == "ro", "只讀那份的值應該贏");
			// 命中的源要能被指出來: 設定頁靠它顯示「這一項由誰接管」
			T(ReferenceEquals(Src, Ro), "命中源應該是只讀源");
			T(F.TryGet(["OnlyRo"], out var Only, out _), "只有只讀那份有的鍵也取得到");
			T(Only is i64 N && N == 1, "OnlyRo 應該是 i64 1");
			return null;
		});

		R("讀源有多個時, 依清單順序決定優先級", async (o) => {
			var Env = MkRo(("Lang", "env"), ("Theme", "dark"));
			var File = MkRo(("Lang", "file"), ("Size", 14L));
			var Rw = MkRw(("Lang", "rw"), ("Size", 99L), ("OnlyRw", "x"));
			var F = new LayeredCfgAccessor();

			F.SetRoRw([Env, File], Rw);

			T(F.TryGet(["Lang"], out var Lang, out var LangSrc), "應該取得到 Lang");
			T((str?)Lang == "env", "三個源都有時, 第一個讀源贏");
			T(ReferenceEquals(LangSrc, Env), "命中源應該是第一個讀源");
			T(F.TryGet(["Theme"], out var Theme, out _), "只有第一個讀源有的鍵也取得到");
			T((str?)Theme == "dark", "Theme 應該來自第一個讀源");
			T(F.TryGet(["Size"], out var Size, out var SizeSrc), "第二個讀源有的鍵取得到");
			T(Size is i64 S && S == 14, "Size 應該來自第二個讀源, 不是可寫源");
			T(ReferenceEquals(SizeSrc, File), "命中源應該是第二個讀源");
			T(F.TryGet(["OnlyRw"], out var OnlyRw, out var RwSrc), "只有可寫源有的鍵也取得到");
			T((str?)OnlyRw == "x", "OnlyRw 應該來自可寫源");
			T(ReferenceEquals(RwSrc, Rw), "命中源應該是可寫源");
			return null;
		});

		R("寫入只落可寫源, 只讀那份不動", async (o) => {
			var Ro = MkRo(("Lang", "ro"));
			var Rw = MkRw(("Lang", "rw"));
			var F = new LayeredCfgAccessor();
			F.SetRoRw([Ro], Rw);

			T(F.TrySetNoSave(["Lang"], "new"), "應該寫得進去");
			T(F.TryGet(["Lang"], out var Lang, out _), "寫完仍取得到");
			// 只讀源在前, 所以讀到的還是它: 這正是「只讀優先」的語義
			T((str?)Lang == "ro", "讀到的仍該是只讀那份的值");
			// 但寫入確實落在可寫源上, 拿掉只讀源之後就看得出來
			T((str?)Rw.Dict["Lang"] == "new", "寫入應該落在可寫源");
			T((str?)Ro.Dict["Lang"] == "ro", "只讀源不該被改");
			return null;
		});

		R("同一個物件當兩個角色會拋", async (o) => {
			var One = MkRw(("K", 1));
			var F = new LayeredCfgAccessor();

			var Threw = false;
			try{
				F.SetRoRw([One], One);
			}catch(ArgumentException){
				Threw = true;
			}
			T(Threw, "同一個源既是只讀又是可寫, 應該直接拒絕");
			return null;
		});

		R("任一個源是 null 會拋", async (o) => {
			var Rw = MkRw();
			var F = new LayeredCfgAccessor();

			var Threw = false;
			try{
				F.SetRoRw(null!, Rw);
			}catch(ArgumentNullException){
				Threw = true;
			}
			T(Threw, "缺只讀源應該拋");
			return null;
		});
	}
}
