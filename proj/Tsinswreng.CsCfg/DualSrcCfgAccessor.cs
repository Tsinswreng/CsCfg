using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;

[Doc(@$"Dual Source Config Accessor")]
public class DualSrcCfgAccessor
	:IDualSrcCfgAccessor
	,ICfgAccessor
{
	[Doc("read only config")]
	public ICfgReader? RoCfg{get;set;}
	[Doc("read write config")]
	public ICfgAccessor? RwCfg{get;set;}
	public DualSrcCfgAccessor(){}
	public DualSrcCfgAccessor(
		ICfgAccessor? RoCfg
		,ICfgAccessor? RwCfg
	){
		this.RoCfg = RoCfg;
		this.RwCfg = RwCfg;
	}

	[Impl(typeof(ICfgAccessor))]
	public bool TryGet(IReadOnlyList<str> Path, out obj? Got){
		if(RoCfg != null && RoCfg.TryGet(Path, out Got)){
			return true;
		}
		if(RwCfg != null && RwCfg.TryGet(Path, out Got)){
			return true;
		}
		Got = default;
		return false;
	}

	[Impl(typeof(ICfgAccessor))]
	public bool TrySetNoSave(IReadOnlyList<str> Path, obj? V){
		if(RwCfg == null){
			return false;
		}
		return RwCfg.TrySetNoSave(Path, V);
	}
	
	[Impl(typeof(ICfgAccessor))]
	public bool RmPathNoSave(IReadOnlyList<str> Path){
		// 只有可寫的那份能刪; 沒有可寫源就代表什麼都沒改動
		if(RwCfg is null){
			return false;
		}
		return RwCfg.RmPathNoSave(Path);
	}
	
	[Impl(typeof(ICfgAccessor))]
	public void Reload(){
		RoCfg?.Reload();
		RwCfg?.Reload();
	}

	[Impl(typeof(ICfgAccessor))]
	public async Task<nil> Reload(CT Ct){
		if(RoCfg is not null){
			return await RoCfg.Reload(Ct);
		}
		if(RwCfg is not null){
			return await RwCfg.Reload(Ct);
		}
		return NIL;
	}
	
	
	[Impl(typeof(ICfgAccessor))]
	public void Save(){
		if(RwCfg is not null){
			RwCfg.Save();
		}
	}
	
	[Impl(typeof(ICfgAccessor))]
	public async Task<nil> Save(CT Ct){
		if(RwCfg is not null){
			return await RwCfg.Save(Ct);
		}
		return NIL;
	}
}
