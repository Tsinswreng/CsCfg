namespace Tsinswreng.CsCfg;

using Tsinswreng.CsCore;

[Doc("""
以嵌套字典作格式無關的中間表示: 不管來源是 JSON 還是 YAML, 先由各自的 codec 解成
`IDictionary<str, obj?>`, 之後的按路徑取值全在這裡做, 與格式無關。

*值模型*: 標量(字符串、整數族、浮點、布爾)、`null`、`IList<obj?>`、`IDictionary<str, obj?>`。
嵌套一律是同一種字典型別, 換型別會讓按路徑下鑽中斷。

*鍵序*: 本類不決定字典的實作。`Dict` 由 codec 給, 新建的中間節點由 `MkDict` 造,
兩者必須一致; 格式要求鍵序穩定時(例如要原樣寫回檔案), 就把 `MkDict` 設成有序字典的工廠。

*取值三態*: 回 false 表示沒有這個鍵; 回 true 且值為 null 表示明確設成了 null。
兩者混為一談的話, 「用 null 表示關閉」這種配置就表達不出來。
""")]
public class DictCfgReader : ICfgReader{

	[Doc(@$"
	本源的技術標識, 例如 `file`、`gui`、`defaults`。
	多源裝配時它就是界面顯示「由誰接管」的依據, 故同一組源裡必須唯一。
	")]
	public str Name{get;set;} = "dict";

	[Doc(@$"配置樹的根。由 codec 解出; {nameof(Reload)} 重讀時整體替換。")]
	public IDictionary<str, obj?> Dict{get;set;} = new OrderedDictionary<str, obj?>();

	[Doc(@$"
	新建中間節點用的工廠。默認造普通 Dictionary; 要保鍵序就換成有序實現。
	*必須*與 {nameof(Dict)} 的實際型別一致, 否則寫入之後樹上會混兩種字典。
	")]
	public Func<IDictionary<str, obj?>> MkDict{get;set;} = ()=> new OrderedDictionary<str, obj?>();

	[Doc(@$"異步重載的數據源; 由宿主設定, 通常是讀檔加解析。")]
	public Func<CT, Task<IDictionary<str, obj?>>>? FnReloadAsy{get;set;}

	[Doc(@$"同步重載的數據源。")]
	public Func<IDictionary<str, obj?>>? FnReload{get;set;}

	[Doc(@$"
	整體重讀。
	替換 {nameof(Dict)} 的引用而不是原地清空再填, 所以正在讀的線程不會看到半份字典。
	")]
	public void Reload(){
		if(FnReload is null){
			throw new InvalidOperationException($"{nameof(FnReload)} is not set. Assign a config source before calling {nameof(Reload)}.");
		}
		Dict = FnReload();
	}

	[Doc(@$"整體重讀(異步)。替換引用而非原地改, 理由同同步版。")]
	public async Task<nil> Reload(CT Ct){
		if(FnReloadAsy is null){
			throw new InvalidOperationException($"{nameof(FnReloadAsy)} is not set. Assign a config source before calling {nameof(Reload)}.");
		}
		Dict = await FnReloadAsy(Ct);
		return NIL;
	}

	[Doc(@$"
	按路徑取值。
	路徑為空、中途某段不是字典、或最後一段的鍵不存在, 都回 false;
	回 true 時值可以是 null, 那表示明確設成了 null。
	")]
	public bool TryGet(IReadOnlyList<str> Path, out obj? Got){
		return DictCfgAccessor.TryGetByPath(Dict, Path, out Got);
	}

	[Doc(@$"單源讀者: 命中就是自己提供了值, 沒命中回 null。")]
	public ICfgReader? ReaderOf(IReadOnlyList<str> Path){
		return TryGet(Path, out _) ? this : null;
	}
	
}
