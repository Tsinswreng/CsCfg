
namespace Tsinswreng.CsCfg;


public interface ICfgAccessor{
	public ICfgValue? GetByPath(IList<str> Path);
	/// <summary>
	/// 未必持久化
	/// </summary>
	/// <param name="Path"></param>
	/// <param name="Value"></param>
	/// <returns></returns>
	public nil SetByPath(IList<str> Path, ICfgValue Value);
	/// <summary>
	/// 未必持久化
	/// </summary>
	/// <param name="Path"></param>
	/// <returns></returns>
	public nil RmPath(IList<str> Path);
	/// <summary>
	///
	/// </summary>
	/// <returns></returns>
	public nil ReLoad();
	public Task<nil> ReLoadAsy(CT Ct);
	/// <summary>
	/// 持久化
	/// </summary>
	/// <returns></returns>
	public nil Save();
	public Task<nil> SaveAsy(CT Ct);

}


