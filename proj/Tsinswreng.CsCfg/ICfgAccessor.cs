using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;


[Doc(@$"Config Accessor")]
public partial interface ICfgAccessor{
	public bool TryGet(
		IList<str> Path, out obj? Got
	);
	
	public bool TrySetNoSave(IList<str> Path, obj? V);
	
	/// 未必持久化
	public nil RmPathNoSave(IList<str> Path);

	public nil Reload();
	
	public Task<nil> Reload(CT Ct);

	
	public nil Save();
	
	public Task<nil> Save(CT Ct);
}
