using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;

[Doc(@$"Dual Source Config Accessor")]
public class DualSrcCfg:ICfgAccessor{
	/// read only config
	/// GetByPath旹更優先
	public ICfgAccessor? RoCfg{get;set;}
	/// read write config
	/// 用作用戶GUI配置
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
	public bool TryGetBoxedByPath(
		IList<str> Path
		,out ICfgValue Got
	){
		if(RoCfg != null && RoCfg.TryGetBoxedByPath(Path, out Got)){
			return true;
		}
		if(RwCfg != null && RwCfg.TryGetBoxedByPath(Path, out Got)){
			return true;
		}
		Got = default!;
		return false;
	}

	[Impl(typeof(ICfgAccessor))]
	public ICfgValue? GetBoxedByPath(IList<str> Path){
		if(this.TryGetBoxedByPath(Path, out var Got)){
			return Got;
		}
		return null;
	}

	[Impl(typeof(ICfgAccessor))]
	public nil SetBoxedByPathNonSave(IList<str> Path, ICfgValue Value){
		RwCfg?.SetBoxedByPathNonSave(Path, Value);
		return NIL;
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
		//RoCfg.Save();
		if(RwCfg != null){
			RwCfg.Save();
		}
		return NIL;
	}
	[Impl(typeof(ICfgAccessor))]
	public async Task<nil> Save(CT Ct){
		//await RoCfg.SaveAsy(Ct);
		if(RwCfg != null){
			await RwCfg.Save(Ct);
		}
		return NIL;
	}

}
