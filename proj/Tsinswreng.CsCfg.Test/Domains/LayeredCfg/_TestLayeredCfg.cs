// 一個 tester 對應一個被測類。被測類是 LayeredCfgAccessor:
// 它管源清單與讀寫路由, 事件只是把各源的轉發出來, 故分兩個分片。
// 主文件以下劃線開頭, 只負責組裝, 不在主文件裏寫用例。
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsCfg.Test.Domains.LayeredCfg;

public partial class TestLayeredCfg : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		// 用例裡會建各自的源, 彼此獨立, 不需要固定的執行順序
		Node.Ordered = false;
		Node.IsParallelRecursive = false;
		RegisterResolve(Node);
		RegisterSrcManagement(Node);
		return Node;
	}
}
