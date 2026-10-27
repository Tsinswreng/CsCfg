using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;


[Doc(@$"Config Reader")]
public partial interface ICfgReader{
	public bool TryGet(
		IReadOnlyList<str> Path, out obj? Got
	);
	
	
	public void Reload();
	
	public Task<nil> Reload(CT Ct);
	
}
