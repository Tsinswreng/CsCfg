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
	public bool RmPathNoSave(IList<str> Path){
		var b = RwCfg?.RmPathNoSave(Path);
		if(b == false){return false;}
		return true;
	}
	
	[Impl(typeof(ICfgAccessor))]
	public bool Reload(){
		var b1 = RoCfg?.Reload();
		var b2 = RwCfg?.Reload();
		if(b1 == false || b2 == false){
			return false;
		}
		return true;
	}

	[Impl(typeof(ICfgAccessor))]
	public async Task<bool> Reload(CT Ct){
		if(RoCfg is not null){
			return await RoCfg.Reload(Ct);
		}
		if(RwCfg is not null){
			return await RwCfg.Reload(Ct);
		}
		return true;
	}
	
	
	[Impl(typeof(ICfgAccessor))]
	public bool Save(){
		if(RwCfg is not null){
			return RwCfg.Save();
		}
		return true;
	}
	[Impl(typeof(ICfgAccessor))]
	public async Task<bool> Save(CT Ct){
		if(RwCfg is not null){
			return await RwCfg.Save(Ct);
		}
		return true;
	}
}
