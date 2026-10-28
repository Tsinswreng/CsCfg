namespace Tsinswreng.CsCfg.Test;

using Tsinswreng.CsTreeTest;
using Tsinswreng.CsCfg.Test.Domains.DictCfg;
using Tsinswreng.CsCfg.Test.Domains.LayeredCfg;
using Tsinswreng.CsCfg.Test.Domains.JsonCfg;

/// 把各域的 tester 掛成一棵測試樹。
/// 類名不能帶點, 否則 CsTreeTest 在生成節點名時會出錯。
public class CsCfgTestMgr : DiEtTestMgr{
	public static CsCfgTestMgr Inst = new();

	public override ITestNode RegisterTestsInto(ITestNode? Node){
		Node = this.TestNode;
		this.RegisterTester<TestDictCfg>();
		this.RegisterTester<TestLayeredCfg>();
		this.RegisterTester<TestJsonCfg>();
		return Node;
	}
}
