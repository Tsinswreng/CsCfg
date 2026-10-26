using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;


[Doc(@$"Config Accessor")]
public partial interface ICfgAccessor{
	public bool TryGet(
		IReadOnlyList<str> Path, out obj? Got
	);
	
	public bool TrySetNoSave(IReadOnlyList<str> Path, obj? V);
	
	public bool RmPathNoSave(IReadOnlyList<str> Path);

	public bool Reload();
	
	public Task<bool> Reload(CT Ct);
	
	public bool Save();
	
	public Task<bool> Save(CT Ct);
}
