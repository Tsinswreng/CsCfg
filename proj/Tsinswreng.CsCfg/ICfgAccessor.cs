using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;


[Doc(@$"Config Accessor")]
public partial interface ICfgAccessor:ICfgReader{

	public bool TrySetNoSave(IReadOnlyList<str> Path, obj? V);
	
	public bool RmPathNoSave(IReadOnlyList<str> Path);

	public bool Save();
	
	public Task<bool> Save(CT Ct);
}
