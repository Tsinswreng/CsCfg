using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;


[Doc(@$"Config Accessor")]
public partial interface ICfgAccessor:ICfgReader{

	public bool TrySetNoSave(IReadOnlyList<str> Path, obj? V);
	
	public void RmPathNoSave(IReadOnlyList<str> Path);

	public void Save();
	
	public Task<nil> Save(CT Ct);
}
