namespace Tsinswreng.CsCfg;

public static class ExtnICfgAccessor{
	extension(ICfgAccessor z){

	}
	public static Func<ICfgNode<T>, T?> MkFnGet<T>(this ICfgAccessor z){
		return (CfgItem)=>{
			return CfgItem.GetFrom(z);
		};
	}

	public static T? Get<T>(
		this ICfgAccessor z
		,ICfgNode<T> CfgNode
	){
		return CfgNode.GetFrom(z);
	}

	public static nil Set<T>(
		this ICfgAccessor z
		,ICfgNode CfgNode
		,T Value
	){
		z.TrySetNoSave(CfgNode.GetFullPathSegs(), Value);
		return NIL;
	}
}
