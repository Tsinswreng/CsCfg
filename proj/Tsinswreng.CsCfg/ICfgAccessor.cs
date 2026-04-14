using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;


[Doc(@$"Config Accessor")]
public partial interface ICfgAccessor{
	public bool TryGet(
		IList<str> Path, out obj? Got
	);
	
	public bool TrySetNoSave(IList<str> Path, obj? V);
	
	/// 未必持久化
	public bool RmPathNoSave(IList<str> Path);

	public bool Reload();
	
	public Task<bool> Reload(CT Ct);

	
	public bool Save();
	
	public Task<bool> Save(CT Ct);
}
