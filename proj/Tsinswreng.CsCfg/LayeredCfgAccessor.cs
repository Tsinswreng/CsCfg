namespace Tsinswreng.CsCfg;

using Tsinswreng.CsCore;

[Doc("""
ILayeredCfgAccessor 的實現。
自己不含配置樹, 只負責按順序問源、把寫入路由到唯一可寫源。

讀取是鍵級別的疊加: 同一個路徑下, 不同源可以分別提供不同的子鍵,
但同一個鍵只認第一個命中的源。
""")]
public class LayeredCfgAccessor : ILayeredCfgAccessor, ICfgEvents{

	/// 真正存源的地方。對外只給只讀視圖, 這樣調用方改不了、也就不會漏掉事件的退訂。
	List<ICfgReader> _Srcs = [];

	[Doc(@$"有序的源; 順序即優先級, 前優先。")]
	public IReadOnlyList<ICfgReader> Srcs => _Srcs;

	ICfgAccessor? _Writable;

	[Doc(@$"
	唯一可寫的源; 為 null 表示整份配置只讀。
	只接受已在 Srcs 裡的源, 否則寫進去的東西讀不回來, 所以要先 AddSrc 再設。
	")]
	public ICfgAccessor? Writable{
		get{
			return _Writable;
		}
		set{
			if(value is not null && !_Srcs.Contains(value)){
				throw new ArgumentException(
					$"{nameof(Writable)} must be one of {nameof(Srcs)}; call {nameof(AddSrc)} first."
					,nameof(Writable)
				);
			}
			_Writable = value;
		}
	}

	[Impl]
	public str Name{get;set;} = "";

	// ===== 事件: 全部由各源轉發而來, 聚合層自己不產生事件 =====

	public event EventHandler<ICfgChange>? BeforeSetByPath;
	public event EventHandler<ICfgChange>? BeforeRmPath;
	public event EventHandler<ICfgChange>? BeforeReload;
	public event EventHandler<ICfgChange>? BeforeSave;

	public event EventHandler<ICfgChange>? AfterSetByPath;
	public event EventHandler<ICfgChange>? AfterRmPath;
	public event EventHandler<ICfgChange>? AfterReload;
	public event EventHandler<ICfgChange>? AfterSave;

	[Doc(@$"
	追加到末尾, 優先級最低。
	同一個源重複加入會被忽略: 那等於事件訂兩次、廣播兩次, 而且在移除時只退得掉一份。
	")]
	public void AddSrc(ICfgReader Src){
		if(Src is null){
			throw new ArgumentNullException(nameof(Src));
		}
		// 把自己當源會讓 TryGet 與 Reload 無限遞歸
		if(ReferenceEquals(Src, this)){
			throw new ArgumentException($"A {nameof(LayeredCfgAccessor)} cannot be its own source.", nameof(Src));
		}
		if(_Srcs.Contains(Src)){
			return;
		}
		_Srcs.Add(Src);
		if(Src is ICfgEvents Ev){
			Hook(Ev);
		}
	}

	[Doc(@$"
	移除一個源。
	若它就是可寫源, Writable 一併清成 null: 否則寫入會落進一個 TryGet 永遠命不中的源。
	")]
	public void RemoveSrc(ICfgReader Src){
		if(Src is null){
			return;
		}
		if(!_Srcs.Remove(Src)){
			return;
		}
		// 不退訂的話, 已移除的源還會繼續往這裡廣播
		if(Src is ICfgEvents Ev){
			Unhook(Ev);
		}
		if(ReferenceEquals(_Writable, Src)){
			_Writable = null;
		}
	}

	[Doc(@$"整體替換。重複的源只留第一次出現的位置; 換完若舊可寫源還在新清單裡就保住它。")]
	public void SetSrcs(IReadOnlyList<ICfgReader> Srcs){
		// 先複製再清空: 調用方很可能就傳進來我們自己的 Srcs, 直接清會邊清邊讀
		var Incoming = Srcs is null ? [] : new List<ICfgReader>(Srcs);
		var OldWritable = _Writable;
		// 走 RemoveSrc 而不是 _Srcs.Clear(), 否則舊源的事件退不掉
		foreach(var Old in new List<ICfgReader>(_Srcs)){
			RemoveSrc(Old);
		}
		foreach(var Src in Incoming){
			AddSrc(Src);
		}
		// RemoveSrc 會把被移除的可寫源清掉; 若它還在新清單裡就恢復
		if(OldWritable is not null && _Srcs.Contains(OldWritable)){
			_Writable = OldWritable;
		}
	}

	[Doc(@$"
	依 Srcs 的順序取第一個命中的源。
	命中即停, 即使值是 null: null 也是值, 上層寫了 null 就是明確遮蔽下層。
	Src 往外吐的是最內層的提供者: 若某個源本身又是聚合層, 它會回報自己的源。
	")]
	public bool TryGet(IReadOnlyList<str> Path, out obj? Got, out ICfgReader? Src){
		foreach(var One in _Srcs){
			if(One.TryGet(Path, out Got, out var Hit)){
				Src = Hit ?? One;
				return true;
			}
		}
		Got = null;
		Src = null;
		return false;
	}

	[Doc(@$"寫入只落可寫源; 沒有可寫源就回 false。")]
	public bool TrySetNoSave(IReadOnlyList<str> Path, obj? V){
		if(_Writable is null){
			return false;
		}
		return _Writable.TrySetNoSave(Path, V);
	}

	[Doc(@$"刪除只落可寫源; 沒有可寫源就回 false。")]
	public bool RmPathNoSave(IReadOnlyList<str> Path){
		if(_Writable is null){
			return false;
		}
		return _Writable.RmPathNoSave(Path);
	}

	[Doc(@$"沒有可寫源就沒有可保存的東西, 不拋。")]
	public void Save(){
		_Writable?.Save();
	}

	[Doc(@$"沒有可寫源就沒有可保存的東西, 不拋。")]
	public async Task<nil> Save(CT Ct){
		if(_Writable is null){
			return NIL;
		}
		return await _Writable.Save(Ct);
	}

	[Doc(@$"依序重載全部源。中途失敗就向上拋, 已重載的保持新值, 沒有回滾。")]
	public void Reload(){
		foreach(var Src in _Srcs){
			Src.Reload();
		}
	}

	[Doc(@$"依序重載全部源。中途失敗就向上拋, 已重載的保持新值, 沒有回滾。")]
	public async Task<nil> Reload(CT Ct){
		foreach(var Src in _Srcs){
			await Src.Reload(Ct);
		}
		return NIL;
	}

	// ===== 事件的訂閱與轉發 =====

	/// 把一個源的事件接到本聚合層。
	/// 用具名方法而不是 lambda: lambda 每次求值都是新的委託實例, 退訂時匹配不上。
	private void Hook(ICfgEvents Ev){
		Ev.BeforeSetByPath += FwdBeforeSet;
		Ev.BeforeRmPath += FwdBeforeRm;
		Ev.BeforeReload += FwdBeforeReload;
		Ev.BeforeSave += FwdBeforeSave;
		Ev.AfterSetByPath += FwdAfterSet;
		Ev.AfterRmPath += FwdAfterRm;
		Ev.AfterReload += FwdAfterReload;
		Ev.AfterSave += FwdAfterSave;
	}

	/// 對稱地退訂。少退一個, 已移除的源就會繼續往這裡廣播。
	private void Unhook(ICfgEvents Ev){
		Ev.BeforeSetByPath -= FwdBeforeSet;
		Ev.BeforeRmPath -= FwdBeforeRm;
		Ev.BeforeReload -= FwdBeforeReload;
		Ev.BeforeSave -= FwdBeforeSave;
		Ev.AfterSetByPath -= FwdAfterSet;
		Ev.AfterRmPath -= FwdAfterRm;
		Ev.AfterReload -= FwdAfterReload;
		Ev.AfterSave -= FwdAfterSave;
	}

	// 轉發時 sender 換成聚合層自己, 負載原樣傳: E.Src 仍是真正變動的那個源。
	private void FwdBeforeSet(object? Sender, ICfgChange E){
		BeforeSetByPath?.Invoke(this, E);
	}
	private void FwdBeforeRm(object? Sender, ICfgChange E){
		BeforeRmPath?.Invoke(this, E);
	}
	private void FwdBeforeReload(object? Sender, ICfgChange E){
		BeforeReload?.Invoke(this, E);
	}
	private void FwdBeforeSave(object? Sender, ICfgChange E){
		BeforeSave?.Invoke(this, E);
	}
	private void FwdAfterSet(object? Sender, ICfgChange E){
		AfterSetByPath?.Invoke(this, E);
	}
	private void FwdAfterRm(object? Sender, ICfgChange E){
		AfterRmPath?.Invoke(this, E);
	}
	private void FwdAfterReload(object? Sender, ICfgChange E){
		AfterReload?.Invoke(this, E);
	}
	private void FwdAfterSave(object? Sender, ICfgChange E){
		AfterSave?.Invoke(this, E);
	}

}
