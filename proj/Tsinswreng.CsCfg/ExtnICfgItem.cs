namespace Tsinswreng.CsCfg;

public static class ExtnICfgItem{

	public static T? GetFrom<T>(
		this ICfgNode<T> Item //int? is not int
		,ICfgAccessor CfgAccessor
	)
	//where T: class
	{
		var Got = CfgAccessor.GetBoxedByPath(Item.GetFullPathSegs());
		if(Got == null || Got.Data == null){
			return (T?)Item.DfltValue?.Data;
		}

		var TypeOfT = typeof(T);
		if(!TypeOfT.IsValueType){
			if(Got.Data is not T R){
				throw new ArgumentException("Got.Data is not T: "+typeof(T));
			}
			return R;
		}else{
			//return (T?)Got.Data; i64轉i32會報錯
			return (T)Convert.ChangeType(Got.Data, typeof(T));
		}
	}

	public static IList<str> GetFullPathSegs(
		this ICfgNode Item
	){
		var Cur = Item;
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
