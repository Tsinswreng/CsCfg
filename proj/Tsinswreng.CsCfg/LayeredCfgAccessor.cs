namespace Tsinswreng.CsCfg;

using Tsinswreng.CsCore;

[Doc("""
ILayeredCfgAccessor 的實現。
自己不含配置樹, 只負責按順序問源、把寫入路由到唯一可寫源。

讀取是鍵級別的疊加: 同一個路徑下, 不同源可以分別提供不同的子鍵,
但同一個鍵只認第一個命中的源。
""")]
public class LayeredCfgAccessor : ILayeredCfgAccessor{

	[Doc(@$"有序的源; 順序即優先級, 前優先。")]
	public IList<ICfgReader> Srcs{get;set;} = [];

	[Doc(@$"唯一可寫的源; 為 null 表示整份配置只讀。")]
	public ICfgAccessor? Writable{get;set;}

	[Impl]
	public str Name{get;set;} = "";

	[Doc(@$"
	依 Srcs 的順序取第一個命中的源。
	命中即停, 即使值是 null: null 也是值, 上層寫了 null 就是明確遮蔽下層。
	Src 往外吐的是最內層的提供者: 若某個源本身又是聚合層, 它會回報自己的源。
	")]
	public bool TryGet(IReadOnlyList<str> Path, out obj? Got, out ICfgReader? Src){
		foreach(var One in Srcs){
			if(One is null){
				continue;
			}
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
		if(Writable is null){
			return false;
		}
		return Writable.TrySetNoSave(Path, V);
	}

	[Doc(@$"刪除只落可寫源; 沒有可寫源就回 false。")]
	public bool RmPathNoSave(IReadOnlyList<str> Path){
		if(Writable is null){
			return false;
		}
		return Writable.RmPathNoSave(Path);
	}

	[Doc(@$"沒有可寫源就沒有可保存的東西, 不拋。")]
	public void Save(){
		Writable?.Save();
	}

	[Doc(@$"沒有可寫源就沒有可保存的東西, 不拋。")]
	public async Task<nil> Save(CT Ct){
		if(Writable is null){
			return NIL;
		}
		return await Writable.Save(Ct);
	}

	[Doc(@$"依序重載全部源。中途失敗就向上拋, 已重載的保持新值, 沒有回滾。")]
	public void Reload(){
		foreach(var Src in Srcs){
			if(Src is null){
				continue;
			}
			Src.Reload();
		}
	}

	[Doc(@$"依序重載全部源。中途失敗就向上拋, 已重載的保持新值, 沒有回滾。")]
	public async Task<nil> Reload(CT Ct){
		foreach(var Src in Srcs){
			if(Src is null){
				continue;
			}
			await Src.Reload(Ct);
		}
		return NIL;
	}

}
