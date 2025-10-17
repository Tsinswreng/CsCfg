
using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;



public  partial interface ICfgAccessor{
	/// <summary>
	/// 無旹返null
	/// </summary>
	/// <param name="Path"></param>
	/// <returns></returns>
	[Impl(typeof(ICfgAccessor))]
	public ICfgValue? GetByPath(IList<str> Path);

	[Impl(typeof(ICfgAccessor))]
	public bool TryGetByPath(
		IList<str> Path
		,out ICfgValue Got
	);
	/// <summary>
	/// 未必持久化
	/// </summary>
	/// <param name="Path"></param>
	/// <param name="Value"></param>
	/// <returns></returns>
	[Impl(typeof(ICfgAccessor))]
	public nil SetByPath(IList<str> Path, ICfgValue Value);
	/// <summary>
	/// 未必持久化
	/// </summary>
	/// <param name="Path"></param>
	/// <returns></returns>
	[Impl(typeof(ICfgAccessor))]
	public nil RmPath(IList<str> Path);
	/// <summary>
	///
	/// </summary>
	/// <returns></returns>
	[Impl(typeof(ICfgAccessor))]
	public nil ReLoad();
	[Impl(typeof(ICfgAccessor))]
	public Task<nil> ReLoadAsy(CT Ct);
	/// <summary>
	/// 持久化
	/// </summary>
	/// <returns></returns>
	[Impl(typeof(ICfgAccessor))]
	public nil Save();
	[Impl(typeof(ICfgAccessor))]
	public Task<nil> SaveAsy(CT Ct);

}


