// 一個 tester 對應一個被測類。被測類是 DictCfgAccessor:
// 它同時繼承了 DictCfgReader 的讀取、寫入與事件, 所以這三個面上的用例都掛在這裡。
// 主文件以下劃線開頭, 只負責組裝, 不在主文件裏寫用例。
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsCfg.Test.Domains.DictCfg;

public partial class TestDictCfg : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		// 這些用例改同一個實例的狀態, 必須按序跑
		Node.Ordered = true;
		Node.IsParallelRecursive = false;
		RegisterTryGet(Node);
		RegisterTrySet(Node);
		RegisterRm(Node);
		RegisterEvents(Node);
		return Node;
	}
}
