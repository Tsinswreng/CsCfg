// 一個 tester 對應一個被測類。這裡的被測面是 JSON 源: JsonCfgCodec 負責轉換,
// JsonCfgReader / JsonCfgAccessor 只是把它接到流上, 所以按「解析」與「寫出」分兩片。
// 主文件以下劃線開頭, 只負責組裝, 不在主文件裏寫用例。
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsCfg.Test.Domains.JsonCfg;

public partial class TestJsonCfg : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		// 每個用例各建各的流, 彼此獨立, 不需要固定的執行順序
		Node.Ordered = false;
		Node.IsParallelRecursive = false;
		RegisterParse(Node);
		RegisterWrite(Node);
		return Node;
	}

	/// 把一段 JSON 文本包成 UTF-8 流; 每個用例都用新的流, 免得互相污染位置。
	internal static MemoryStream MkStream(str Text){
		return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Text));
	}

	/// 把流讀回文本; 調用前寫入器必須已處置(它會把剩下的字節沖進流)。
	internal static str ReadText(MemoryStream S){
		return System.Text.Encoding.UTF8.GetString(S.ToArray());
	}
}
