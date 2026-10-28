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
public class DictCfgReader : ICfgReader, ICfgEvents{

	[Doc(@$"
	本源的技術標識, 例如 `file`、`gui`、`defaults`。
	多源裝配時它就是界面顯示「由誰接管」的依據, 故同一組源裡必須唯一。
	")]
	public str Name{get;set;} = "dict";

	// ===== 事件: 本類只會觸發 Reload 那一對, 其餘由寫入方(DictCfgAccessor)觸發 =====

	[Doc(@$"將要整體重讀。")]
	public event EventHandler<ICfgChange>? BeforeReload;
	[Doc(@$"重讀完成, Path 為 null 表示整份換掉。")]
	public event EventHandler<ICfgChange>? AfterReload;

	/// 本類沒有寫入能力, 這四個事件是為了滿足 ICfgEvents; 由子類或組合方去觸發。
	public event EventHandler<ICfgChange>? BeforeSetByPath;
	public event EventHandler<ICfgChange>? AfterSetByPath;
	public event EventHandler<ICfgChange>? BeforeRmPath;
	public event EventHandler<ICfgChange>? AfterRmPath;
	public event EventHandler<ICfgChange>? BeforeSave;
	public event EventHandler<ICfgChange>? AfterSave;

	// C# 的 field-like 事件只能在宣告它的類裡觸發, 所以子類(寫入方)透過這幾個方法拋。
	// 命名照 BCL 慣例用 On*。
	protected void OnBeforeSet(ICfgChange E){
		BeforeSetByPath?.Invoke(this, E);
	}
	protected void OnAfterSet(ICfgChange E){
		AfterSetByPath?.Invoke(this, E);
	}
	protected void OnBeforeRm(ICfgChange E){
		BeforeRmPath?.Invoke(this, E);
	}
	protected void OnAfterRm(ICfgChange E){
		AfterRmPath?.Invoke(this, E);
	}
	protected void OnBeforeReload(ICfgChange E){
		BeforeReload?.Invoke(this, E);
	}
	protected void OnAfterReload(ICfgChange E){
		AfterReload?.Invoke(this, E);
	}
	protected void OnBeforeSave(ICfgChange E){
		BeforeSave?.Invoke(this, E);
	}
	protected void OnAfterSave(ICfgChange E){
		AfterSave?.Invoke(this, E);
	}

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
		// Before 時還沒有新值, Src 只能填自己; Path 為 null 表示整份換掉
		OnBeforeReload(new CfgChange(this));
		Dict = FnReload();
		OnAfterReload(new CfgChange(this));
	}

	[Doc(@$"整體重讀(異步)。替換引用而非原地改, 理由同同步版; 失敗時不觸發 After。")]
	public async Task<nil> Reload(CT Ct){
		if(FnReloadAsy is null){
			throw new InvalidOperationException($"{nameof(FnReloadAsy)} is not set. Assign a config source before calling {nameof(Reload)}.");
		}
		OnBeforeReload(new CfgChange(this));
		Dict = await FnReloadAsy(Ct);
		OnAfterReload(new CfgChange(this));
		return NIL;
	}

	[Doc(@$"
	按路徑取值。命中時 Src 就是自己: 單源讀者沒有別人可以提供這個值。
	路徑為空、中途某段不是字典、或最後一段的鍵不存在, 都回 false, 此時 Got 與 Src 皆為 null。
	回 true 時 Got 可以是 null, 那表示明確設成了 null。
	")]
	public bool TryGet(IReadOnlyList<str> Path, out obj? Got, out ICfgReader? Src){
		if(DictCfgAccessor.TryGetByPath(Dict, Path, out Got)){
			Src = this;
			return true;
		}
		Src = null;
		return false;
	}
	
}
