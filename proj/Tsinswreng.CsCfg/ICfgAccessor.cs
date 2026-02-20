
using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;



public partial interface ICfgAccessor{
	/// 無旹返null
	[Impl(typeof(ICfgAccessor))]
	public ICfgValue? GetBoxedByPath(IList<str> Path);

	[Impl(typeof(ICfgAccessor))]
	public bool TryGetBoxedByPath(
		IList<str> Path
		,out ICfgValue Got
	);
	/// 未必持久化
	[Impl(typeof(ICfgAccessor))]
	public nil SetBoxedByPathNonSave(IList<str> Path, ICfgValue Value);
	/// 未必持久化
	[Impl(typeof(ICfgAccessor))]
	public nil RmPath(IList<str> Path);
	[Impl(typeof(ICfgAccessor))]
	public nil ReLoad();
	[Impl(typeof(ICfgAccessor))]
	public Task<nil> ReLoadAsy(CT Ct);
	/// 持久化
	[Impl(typeof(ICfgAccessor))]
	public nil Save();
	[Impl(typeof(ICfgAccessor))]
	public Task<nil> SaveAsy(CT Ct);

}


