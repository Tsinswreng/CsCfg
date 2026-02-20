namespace Tsinswreng.CsCfg;

public static class ExtnICfgItem{

	public static T? GetFrom<T>(
		this ICfgItem<T> Item //int? is not int
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
		this ICfgItem Item
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
		this ICfgItem z
		,str Sep = CfgItem<nil>.PathSep
	){
		if(z._LazyFullPath is not null){
			return z._LazyFullPath;
		}
		var Segs = z.GetFullPathSegs();
		var R = string.Join(Sep, Segs);
		z._LazyFullPath = R;
		return R;
	}



}
