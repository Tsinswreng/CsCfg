namespace Tsinswreng.CsCfg;
using Tsinswreng.CsKeyNode;
public static class ICfgAccessorExtn{
	extension(ICfgAccessor z){

	}
	public static nil SetNoSave<T>(
		this ICfgAccessor z
		,IKeyNode CfgNode
		,T Value
	){
		z.TrySetNoSave(CfgNode.FullPathSegs, Value);
		return NIL;
	}
}
