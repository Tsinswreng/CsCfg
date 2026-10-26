namespace Tsinswreng.CsCfg;
using Tsinswreng.CsKeyNode;
public static class ICfgAccessorExtn{
	extension(ICfgAccessor z){

	}
	
	static T? GetFrom<T>(
		this IKeyNode<T> Node // `int?` is not `int`
		,ICfgAccessor CfgAccessor
	)
	{
		if(!CfgAccessor.TryGet(Node.FullPathSegs, out var Got) || Got == null){
			return Node.Data;
		}

		if(Got is T Typed){
			return Typed;
		}

		var TargetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
		if(TargetType.IsValueType){
			var Converted = Convert.ChangeType(Got, TargetType);
			return (T)Converted;
		}

		throw new ArgumentException("Cfg value type mismatch: "+typeof(T));
	}
	
	public static Func<IKeyNode<T>, T?> MkFnGet<T>(this ICfgAccessor z){
		return (CfgItem)=>{
			return CfgItem.GetFrom(z);
		};
	}

	public static T? Get<T>(
		this ICfgAccessor z
		,IKeyNode<T> CfgNode
	){
		return CfgNode.GetFrom(z);
	}

	public static nil Set<T>(
		this ICfgAccessor z
		,IKeyNode CfgNode
		,T Value
	){
		z.TrySetNoSave(CfgNode.FullPathSegs, Value);
		return NIL;
	}
}
