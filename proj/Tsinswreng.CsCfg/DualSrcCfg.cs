using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;

[Doc(@$"Dual Source Config Accessor")]
public class DualSrcCfg:ICfgAccessor{
	[Doc("read only config")]
	public ICfgAccessor? RoCfg{get;set;}
	[Doc("read write config")]
	public ICfgAccessor? RwCfg{get;set;}
	public DualSrcCfg(){}
	public DualSrcCfg(
		ICfgAccessor? RoCfg
		,ICfgAccessor? RwCfg
	){
		this.RoCfg = RoCfg;
		this.RwCfg = RwCfg;
	}

	[Impl(typeof(ICfgAccessor))]
	public bool TryGet(IList<str> Path, out obj? Got){
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
	public bool TrySetNoSave(IList<str> Path, obj? V){
		if(RwCfg == null){
			return false;
		}
		return RwCfg.TrySetNoSave(Path, V);
	}
	
	[Impl(typeof(ICfgAccessor))]
	public nil RmPathNoSave(IList<str> Path){
		RwCfg?.RmPathNoSave(Path);
		return NIL;
	}
	
	[Impl(typeof(ICfgAccessor))]
	public nil Reload(){
		RoCfg?.Reload();
		RwCfg?.Reload();
		return NIL;
	}

	[Impl(typeof(ICfgAccessor))]
	public async Task<nil> Reload(CT Ct){
		if(RoCfg != null){
			await RoCfg.Reload(Ct);
		}
		if(RwCfg != null){
			await RwCfg.Reload(Ct);
		}
		return NIL;
	}
	
	
	[Impl(typeof(ICfgAccessor))]
	public nil Save(){
		if(RwCfg != null){
			RwCfg.Save();
		}
		return NIL;
	}
	[Impl(typeof(ICfgAccessor))]
	public async Task<nil> Save(CT Ct){
		if(RwCfg != null){
			await RwCfg.Save(Ct);
		}
		return NIL;
	}
}
