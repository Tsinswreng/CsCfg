namespace Tsinswreng.CsCfg;

using Tsinswreng.CsCore;

[Doc("""
在 `DictCfgReader` 之上加寫入與持久化。

寫入只改內存裡的配置樹, 落盤由 `FnSave` / `FnSaveAsy` 負責;
所以 `TrySetNoSave` 與 `RmPathNoSave` 之後必須再呼叫 `Save`, 才會進到檔案。
這也是與「出廠配置檔唯讀」配套的分工: 出廠那份只讀, 寫入只落程序專用的那份。
""")]
public class DictCfgAccessor : DictCfgReader, ICfgAccessor{

	[Doc(@$"同步保存; 參數是當前整棵配置樹。")]
	public Func<IDictionary<str, obj?>, nil>? FnSave{get;set;}

	[Doc(@$"異步保存; 參數是當前整棵配置樹。")]
	public Func<IDictionary<str, obj?>, Task<nil>>? FnSaveAsy{get;set;}

	[Doc(@$"
	按路徑寫值, 只改內存。
	缺中間節點就按 {nameof(MkDict)} 補; 中間節點存在但不是字典時回 false 且不改動任何東西。
	Before 會照樣觸發(確實嘗試過了), After 只在真的寫成功時觸發。
	")]
	public bool TrySetNoSave(IReadOnlyList<str> Path, obj? V){
		OnBeforeSet(new CfgChange(this, Path));
		var Ok = TrySetByPath(Dict, Path, V, MkDict);
		if(Ok){
			OnAfterSet(new CfgChange(this, Path));
		}
		return Ok;
	}

	[Doc(@$"
	按路徑刪鍵, 只改內存。
	刪完會由內而外清掉留下的空字典, 根不清理。
	回 true 表示這棵樹真的被改動了; 回 false 表示什麼都沒刪, 可能是鍵不存在、
	中途某段缺失、中途某段不是字典、或路徑為空。
	After 只在真的刪掉一個鍵時觸發: 什麼都沒刪就不算變更。
	")]
	public bool RmPathNoSave(IReadOnlyList<str> Path){
		OnBeforeRm(new CfgChange(this, Path));
		var Ok = RmByPath(Dict, Path);
		if(Ok){
			OnAfterRm(new CfgChange(this, Path));
		}
		return Ok;
	}

	[Doc(@$"把整棵配置樹交給 {nameof(FnSave)}。失敗時不觸發 After。")]
	public void Save(){
		if(FnSave is null){
			throw new InvalidOperationException($"{nameof(FnSave)} is not set. Assign a save target before calling {nameof(Save)}.");
		}
		OnBeforeSave(new CfgChange(this));
		FnSave(Dict);
		OnAfterSave(new CfgChange(this));
	}

	[Doc(@$"把整棵配置樹交給 {nameof(FnSaveAsy)}。失敗時不觸發 After。")]
	public async Task<nil> Save(CT Ct){
		if(FnSaveAsy is null){
			throw new InvalidOperationException($"{nameof(FnSaveAsy)} is not set. Assign a save target before calling {nameof(Save)}.");
		}
		OnBeforeSave(new CfgChange(this));
		var R = await FnSaveAsy(Dict);
		OnAfterSave(new CfgChange(this));
		return R;
	}
	
	
	
	#region static
[Doc(@$"沿嵌套字典按路徑取值。命中時 {nameof(Got)} 可以為 null。")]
	public static bool TryGetByPath(
		IDictionary<str, obj?> Dict
		,IReadOnlyList<str> Path
		,out obj? Got
	){
		Got = null;
		if(Dict is null || Path is null || Path.Count == 0){
			return false;
		}
		var Cur = Dict;
		// 逐段下鑽; 中途缺鍵或值不是字典, 都代表這條路徑不存在
		for(var i = 0; i < Path.Count - 1; i++){
			if(!Cur.TryGetValue(Path[i], out var Next) || Next is not IDictionary<str, obj?> Child){
				return false;
			}
			Cur = Child;
		}
		// 最後一段: 鍵存在即命中, 值可以為 null
		return Cur.TryGetValue(Path[Path.Count - 1], out Got);
	}

	[Doc(@$"
	沿嵌套字典按路徑寫值, 缺中間節點就用 {nameof(MkDict)} 造。
	中間節點存在但不是字典時回 false 且不改動任何東西: 那多半是鍵名寫錯,
	靜默覆蓋會把原本的配置弄丟。
	")]
	public static bool TrySetByPath(
		IDictionary<str, obj?> Dict
		,IReadOnlyList<str> Path
		,obj? V
		,Func<IDictionary<str, obj?>> MkDict
	){
		if(Dict is null || Path is null || Path.Count == 0){
			return false;
		}
		var Cur = Dict;
		var i = 0;
		// 第一趟只走既有節點: 遇到「鍵存在但不是字典」就放棄, 此時還沒動過任何東西
		for(; i < Path.Count - 1; i++){
			if(!Cur.TryGetValue(Path[i], out var Next)){
				break;
			}
			if(Next is not IDictionary<str, obj?> Child){
				return false;
			}
			Cur = Child;
		}
		// 第二趟: 從第一個缺口起新建中間節點
		for(; i < Path.Count - 1; i++){
			var New = MkDict();
			Cur[Path[i]] = New;
			Cur = New;
		}
		Cur[Path[Path.Count - 1]] = V;
		return true;
	}

	[Doc(@$"
	沿嵌套字典按路徑刪鍵, 然後由內而外清掉因此留下的空字典; 根不清理。
	回 true 表示真的刪掉了一個鍵; 路徑不存在或中途被標量擋住時回 false 且什麼都不改。
	")]
	public static bool RmByPath(
		IDictionary<str, obj?> Dict
		,IReadOnlyList<str> Path
	){
		if(Dict is null || Path is null || Path.Count == 0){
			return false;
		}
		// 走到底, 順便把經過的每一層記下來, 供之後由內而外清理
		var Chain = new List<IDictionary<str, obj?>>{Dict};
		var Cur = Dict;
		for(var i = 0; i < Path.Count - 1; i++){
			if(!Cur.TryGetValue(Path[i], out var Next) || Next is not IDictionary<str, obj?> Child){
				return false;
			}
			Cur = Child;
			Chain.Add(Cur);
		}
		var LastKey = Path[Path.Count - 1];
		// 鍵不存在時什麼都不做: 刪除是冪等的, 不算失敗, 只是沒改動
		if(!Cur.Remove(LastKey)){
			return false;
		}
		// Chain[j] 是 Path[j-1] 對應的字典; 空掉就把 Path[j-1] 從上一層摘掉
		for(var j = Chain.Count - 1; j >= 1; j--){
			if(Chain[j].Count != 0){
				break;
			}
			Chain[j - 1].Remove(Path[j - 1]);
		}
		return true;
	}
	#endregion static
}
