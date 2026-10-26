using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;

[Doc(@$"Dual Source Config Accessor")]
public interface IDualSrcCfg:ICfgAccessor{
	[Doc("read only config")]
	public ICfgAccessor? RoCfg{get;set;}
	[Doc("read write config")]
	public ICfgAccessor? RwCfg{get;set;}
	
}
