namespace Tsinswreng.CsCfg;
using Tsinswreng.CsKeyNode;
public static class ICfgReaderExtn{
	
	
	public static bool TryGet(
		this ICfgReader z, IKeyNode KeyNode, out obj? Got
	){
		
		return z.TryGet(KeyNode.FullPathSegs, out Got);
	}
	
	
	public static Func<IKeyNode<T>, T> MkFnGet<T>(this ICfgReader z){
		return (CfgNode)=>{
			return z.Get(CfgNode);
		};
	}

	public static T Get<T>(
		this ICfgReader z
		,IKeyNode<T> CfgNode
	){
		var Node = CfgNode;
		if(!z.TryGet(Node.FullPathSegs, out var Got)){
			return Node.Data;
		}
		
		//顯式設置成null的返null 不用默認值
		if(Got is null){
			return default!;
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


}
