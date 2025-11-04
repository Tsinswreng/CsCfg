namespace Tsinswreng.CsCfg;

public partial class CfgItem<T>:ICfgItem<T>{
	public const str PathSep = "/";
	public CfgItem(){

	}

	public IList<str> RelaPathSegs{get;set;} = [];
	public str? _LazyFullPath{get;set;}
	public ICfgValue? DfltValue{get;set;}
	public ICfgItem? Parent{get;set;}
	public IList<ICfgItem>? Children{get;set;}

	public static ICfgItem<object?>Mk(
		ICfgItem? Parent
		,IList<str> Path
		,ICfgValue? DfltValue = null
	){
		return new CfgItem<object?>{RelaPathSegs=Path, DfltValue=DfltValue, Parent=Parent};
	}

	// [Obsolete("用Mk")]
	// public static ICfgItem<object?> OldMk(
	// 	IList<str> Path
	// 	,ICfgValue? DfltValue = null
	// 	,ICfgItem? Parent = null
	// ){
	// 	return new CfgItem<object?>{RelaPath=Path, DfltValue=DfltValue, Parent=Parent};
	// }

/// <summary>
/// 如需列表則需定義潙IList<object> 不支持IList<str>等!
/// </summary>
/// <typeparam name="T2"></typeparam>
/// <param name="Path"></param>
/// <param name="DfltValue"></param>
/// <returns></returns>
	public static ICfgItem<T2> Mk<T2>(
		ICfgItem? Parent
		,IList<str> Path
		,T2 DfltValue = default!
	){
		var V = new CfgValue(){Type=typeof(T2), Data=DfltValue};
		return new CfgItem<T2>{RelaPathSegs=Path, DfltValue=V, Parent=Parent};
	}
}

public static class ExtnCfgItem{
	public static Func<ICfgItem<T>, T?> MkFnGet<T>(this ICfgAccessor z){
		return (CfgItem)=>{
			return CfgItem.GetFrom(z);
		};
	}

	public static T? Get<T>(
		this ICfgAccessor z
		,ICfgItem<T> CfgItem
	){
		return CfgItem.GetFrom(z);
	}

/// <summary>
///
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Item"></param>
/// <param name="CfgAccessor"></param>
/// <returns></returns>
/// <exception cref="ArgumentException"></exception>
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
