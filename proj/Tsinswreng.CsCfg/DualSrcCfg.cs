using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;

//TODO implement event
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


	// [Impl(typeof(ICfgAccessor))]
	// public ICfgValue? GetByPath(IList<str> Path){
	// 	var R = RoCfg?.GetByPath(Path);
	// 	if(R != null){
	// 		return R;
	// 	}
	// 	return RwCfg?.GetByPath(Path);
	// }


	/// 未必持久化
	/// <param name="Path"></param>
	/// <param name="Value"></param>
	/// <returns></returns>
	[Impl(typeof(ICfgAccessor))]
	public nil SetBoxedByPathNonSave(IList<str> Path, ICfgValue Value){
		RwCfg?.SetBoxedByPathNonSave(Path, Value);
		return NIL;
	}
	/// 未必持久化
	/// <param name="Path"></param>
	/// <returns></returns>
	[Impl(typeof(ICfgAccessor))]
	public nil RmPath(IList<str> Path){
		RwCfg?.RmPath(Path);
		return NIL;
	}
	///
	/// <returns></returns>
	[Impl(typeof(ICfgAccessor))]
	public nil ReLoad(){
		RoCfg?.ReLoad();
		RwCfg?.ReLoad();
		return NIL;
	}

	[Impl(typeof(ICfgAccessor))]
	public async Task<nil> ReLoadAsy(CT Ct){
		if(RoCfg != null){
			await RoCfg.ReLoadAsy(Ct);
		}
		if(RwCfg != null){
			await RwCfg.ReLoadAsy(Ct);
		}
		return NIL;
	}
	/// 持久化
	/// <returns></returns>
	[Impl(typeof(ICfgAccessor))]
	public nil Save(){
		//RoCfg.Save();
		if(RwCfg != null){
			RwCfg.Save();
		}
		return NIL;
	}
	[Impl(typeof(ICfgAccessor))]
	public async Task<nil> SaveAsy(CT Ct){
		//await RoCfg.SaveAsy(Ct);
		if(RwCfg != null){
			await RwCfg.SaveAsy(Ct);
		}
		return NIL;
	}

}
