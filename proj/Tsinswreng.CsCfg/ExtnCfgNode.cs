namespace Tsinswreng.CsCfg;

public static class ExtnCfgNode{
	public static T? GetFrom<T>(
		this ICfgNode<T> Node // `int?` is not `int`
		,ICfgAccessor CfgAccessor
	)
	{
		if(!CfgAccessor.TryGet(Node.GetFullPathSegs(), out var Got) || Got == null){
			return Node.DfltValue;
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

	public static IList<str> GetFullPathSegs(
		this ICfgNode Node
	){
		var Cur = Node;
		var List2D = new List<IList<str>>();
		for(;;){
			if(Cur == null){break;}
			List2D.Add(Cur.RelaPathSegs);
			Cur = Cur.Parent;
		}
		List2D.Reverse();
		var R = new List<str>();
		foreach(var List in List2D){
			foreach(var Path in List){
				R.Add(Path);
			}
		}
		return R;
	}

	public static str GetFullPath(
		this ICfgNode z
		,str Sep = CfgNode<nil>.PathSep
	){
		if(z._FullPathCache is not null){
			return z._FullPathCache;
		}
		var Segs = z.GetFullPathSegs();
		var R = string.Join(Sep, Segs);
		z._FullPathCache = R;
		return R;
	}
}
