// 本分片只測 LayeredCfgAccessor 的源管理: AddSrc 去重、可寫源校驗、移除後退訂、SetSrcs 保序。
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsCfg.Test.Domains.LayeredCfg;

public partial class TestLayeredCfg{
	public void RegisterSrcManagement(ITestNode Node){
		// 這片測的是三個方法, 名字一次列全
		var Names = nameof(LayeredCfgAccessor.AddSrc)
			+ ", " + nameof(LayeredCfgAccessor.RemoveSrc)
			+ ", " + nameof(LayeredCfgAccessor.SetSrcs);
		var register = Node.MkTestFnRegister(
			typeof(TestLayeredCfg),
			[typeof(LayeredCfgAccessor)],
			[Names],
			"SrcMgmt"
		);
		var R = register.Register;
		var T = Assert.IsTrue;

		R("同一個源重複加入只留一份", async (o) => {
			var A = new DictCfgAccessor{ Name = "a" };
			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(A);
			Layered.AddSrc(A);

			T(Layered.Srcs.Count == 1, "重複加入應該被忽略");
			return null;
		});

		R("加入 null 或自己會拋", async (o) => {
			var Layered = new LayeredCfgAccessor();

			var NullThrew = false;
			try{
				Layered.AddSrc(null!);
			}catch(ArgumentNullException){
				NullThrew = true;
			}
			T(NullThrew, "加入 null 應該拋 ArgumentNullException");

			var SelfThrew = false;
			try{
				Layered.AddSrc(Layered);
			}catch(ArgumentException){
				SelfThrew = true;
			}
			T(SelfThrew, "把自己當源應該拋 ArgumentException");
			return null;
		});

		R("可寫源必須先在源清單裡", async (o) => {
			var A = new DictCfgAccessor{ Name = "a" };
			var Layered = new LayeredCfgAccessor();

			var Threw = false;
			try{
				Layered.Writable = A;
			}catch(ArgumentException){
				Threw = true;
			}
			T(Threw, "不在清單裡的源不該被設成可寫源");

			// 先加進去就允許了
			Layered.AddSrc(A);
			Layered.Writable = A;
			T(ReferenceEquals(Layered.Writable, A), "加進清單之後應該設得上");
			return null;
		});

		R("移除可寫源時一併清掉 Writable", async (o) => {
			var A = new DictCfgAccessor{ Name = "a" };
			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(A);
			Layered.Writable = A;

			Layered.RemoveSrc(A);

			T(Layered.Srcs.Count == 0, "源應該被移除");
			T(Layered.Writable is null, "可寫源被移除後應該清成 null");
			return null;
		});

		R("移除之後不再轉發它的事件", async (o) => {
			var A = new DictCfgAccessor{ Name = "a" };
			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(A);

			var Count = 0;
			Layered.AfterSetByPath += (S, E) => Count++;

			A.TrySetNoSave(["K"], 1);
			T(Count == 1, "還在清單裡時應該轉發");

			Layered.RemoveSrc(A);
			A.TrySetNoSave(["K"], 2);
			T(Count == 1, "移除之後不該再轉發");
			return null;
		});

		R("轉發時 sender 是聚合層, 負載裡的源是真正的源", async (o) => {
			var A = new DictCfgAccessor{ Name = "a" };
			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(A);

			obj? Sender = null;
			ICfgReader? Src = null;
			Layered.AfterSetByPath += (S, E)=>{
				Sender = S;
				Src = E.Src;
			};

			A.TrySetNoSave(["K"], 1);

			T(ReferenceEquals(Sender, Layered), "sender 應該是聚合層自己");
			T(ReferenceEquals(Src, A), "事件負載裡的源應該是真正變動的源");
			return null;
		});

		R("SetSrcs 換清單並保住仍在清單裡的可寫源", async (o) => {
			var A = new DictCfgAccessor{ Name = "a" };
			var B = new DictCfgAccessor{ Name = "b" };
			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(A);
			Layered.Writable = A;

			// 把 A 換到後面, B 排前面
			Layered.SetSrcs([B, A]);

			T(Layered.Srcs.Count == 2, "應該有兩個源");
			T(ReferenceEquals(Layered.Srcs[0], B), "B 應該在前面");
			T(ReferenceEquals(Layered.Writable, A), "A 還在清單裡, 應該保住可寫源");

			// A 不在新清單裡時就該清掉
			Layered.SetSrcs([B]);
			T(Layered.Writable is null, "可寫源不在新清單裡就該清成 null");
			return null;
		});

		R("SetSrcs 傳入自己的 Srcs 不會邊清邊讀", async (o) => {
			var A = new DictCfgAccessor{ Name = "a" };
			var B = new DictCfgAccessor{ Name = "b" };
			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(A);
			Layered.AddSrc(B);

			// 這是最容易寫錯的調用方式: 進來的清單就是自己的內部清單
			Layered.SetSrcs(Layered.Srcs);

			T(Layered.Srcs.Count == 2, "應該還是兩個源");
			T(ReferenceEquals(Layered.Srcs[0], A), "順序不該變");
			T(ReferenceEquals(Layered.Srcs[1], B), "順序不該變");
			return null;
		});

		R("Reload 會依序重載每個源", async (o) => {
			var A = new DictCfgAccessor{ Name = "a" };
			var B = new DictCfgAccessor{ Name = "b" };
			var Order = new List<str>();
			A.FnReload = ()=>{
				Order.Add("a");
				return new OrderedDictionary<str, obj?>();
			};
			B.FnReload = ()=>{
				Order.Add("b");
				return new OrderedDictionary<str, obj?>();
			};

			var Layered = new LayeredCfgAccessor();
			Layered.AddSrc(A);
			Layered.AddSrc(B);
			Layered.Reload();

			T(Order.Count == 2, "兩個源都該被重載");
			T(Order[0] == "a" && Order[1] == "b", "應該按照源順序");
			return null;
		});
	}
}
