// 一個 tester 對應一個被測成員: ILayeredCfgAccessorExtn.SetRoRw。
// 主文件以下劃線開頭, 只負責組裝與提供造源的輔助, 不在主文件裏寫用例。
using Tsinswreng.CsTreeTest;
using Tsinswreng.CsCfg;

namespace Tsinswreng.CsCfg.Test.Domains.RoRw;

public partial class TestRoRw : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		// 每個用例各建各的源, 彼此獨立, 不需要固定的執行順序
		Node.Ordered = false;
		Node.IsParallelRecursive = false;
		RegisterSetRoRw(Node);
		return Node;
	}

	/// 造一個只讀源(帶內容)。用最小的實現: 這個域只在乎「源的角色」, 不在乎來源是檔案還是字典。
	internal static DictCfgReader MkRo(params (str Key, obj? V)[] Items){
		var R = new DictCfgReader{ Name = "ro" };
		var D = new OrderedDictionary<str, obj?>();
		foreach(var One in Items){
			D[One.Key] = One.V;
		}
		R.Dict = D;
		return R;
	}

	/// 造一個可讀寫源(帶內容)。
	internal static DictCfgAccessor MkRw(params (str Key, obj? V)[] Items){
		var A = new DictCfgAccessor{ Name = "rw" };
		var D = new OrderedDictionary<str, obj?>();
		foreach(var One in Items){
			D[One.Key] = One.V;
		}
		A.Dict = D;
		return A;
	}
}
