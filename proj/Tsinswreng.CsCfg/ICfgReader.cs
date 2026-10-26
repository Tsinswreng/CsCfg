using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;


[Doc(@$"Config Reader")]
public partial interface ICfgReader{
	public bool TryGet(
		IReadOnlyList<str> Path, out obj? Got
	);
	
	
	public bool Reload();
	
	public Task<bool> Reload(CT Ct);
	
}
