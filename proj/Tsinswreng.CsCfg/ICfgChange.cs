namespace Tsinswreng.CsCfg;

using Tsinswreng.CsCore;

[Doc("""
一次配置變更。

Path 為 null 表示整體替換(例如 Reload); 非 null 表示單鍵變更。
Src 是真正提供該值的源: Before 時填將要寫入的那個源, After 時填真正命中的源。

*為何用接口而不是具體類*: 變更有多種形態(單鍵、整體替換、日後可能的批次),
各自可以帶不同的附加資訊, 訂閱者只依賴最小的那幾個成員。
代價是每次觸發要分配一個物件; 配置變更頻率低, 這點代價可以接受。
""")]
public interface ICfgChange{

	[Doc(@$"真正提供這個值的源。")]
	public ICfgReader Src{get;}

	[Doc(@$"變更的路徑; 為 null 表示整體替換。")]
	public IReadOnlyList<str>? Path{get;}
}

[Doc(@$"{nameof(ICfgChange)} 的默認實現。要帶額外資訊就自己再寫一個實現類。")]
public class CfgChange : ICfgChange{

	[Doc(@$"真正提供這個值的源。")]
	public ICfgReader Src{get;set;} = null!;

	[Doc(@$"變更的路徑; 為 null 表示整體替換。")]
	public IReadOnlyList<str>? Path{get;set;}

	public CfgChange(){}

	[Doc("""
	#Prm[Src][真正提供這個值的源。]
	#Prm[Path][變更的路徑; 省略或傳 null 表示整體替換。]
	""")]
	public CfgChange(ICfgReader Src, IReadOnlyList<str>? Path = null){
		this.Src = Src;
		this.Path = Path;
	}
}
