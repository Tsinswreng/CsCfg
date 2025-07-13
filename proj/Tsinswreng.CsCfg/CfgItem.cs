namespace Tsinswreng.CsCfg;
//TODO獨立作項目
public class CfgItem<T>:ICfgItem<T>{
	public CfgItem(){

	}
	public IList<str> RelaPath{get;set;} = [];
	public ICfgValue? DfltValue{get;set;}
	public ICfgItem? Parent{get;set;}
	public IList<ICfgItem>? Children{get;set;}
}

public static class ExtnCfgItem{

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
		var Got = CfgAccessor.GetByPath(Item.GetFullPath());
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

	public static IList<str> GetFullPath(
		this ICfgItem Item
	){
		var Cur = Item;
		var List2D = new List<IList<str>>();
		for(;;){
			if(Cur == null){break;}
			List2D.Add(Cur.RelaPath);
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

	public static ICfgItem<object?> Mk(
		ICfgItem? Parent
		,IList<str> Path
		,ICfgValue? DfltValue = null
	){
		return new CfgItem<object?>{RelaPath=Path, DfltValue=DfltValue, Parent=Parent};
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
		return new CfgItem<T2>{RelaPath=Path, DfltValue=V, Parent=Parent};
	}

	// [Obsolete("用Mk")]
	// public static ICfgItem<T2> OldMk<T2>(
	// 	IList<str> Path
	// 	,T2 DfltValue = default!
	// 	,ICfgItem? Parent = null
	// ){
	// 	var V = new CfgValue(){Type=typeof(T2), Data=DfltValue};
	// 	return new CfgItem<T2>{RelaPath=Path, DfltValue=V, Parent=Parent};
	// }


	// public static T? Get<T>(
	// 	this ICfgItem Item
	// 	,ICfgAccessor? CfgAccessor = null
	// )where T: class{
	// 	CfgAccessor ??= AppCfg.Inst;
	// 	var Got = CfgAccessor.GetByPath(Item.Path);
	// 	if(Got == null){
	// 		return null;
	// 	}
	// 	return Got.Data as T;
	// }
}
