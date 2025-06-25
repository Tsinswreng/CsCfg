namespace Tsinswreng.CsCfg;
//TODO獨立作項目
public class CfgItem<T>:ICfgItem<T>{
	public CfgItem(){

	}
	public IList<str> Path{get;set;} = [];
	public ICfgValue? DfltValue{get;set;}
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
		this ICfgItem<T> Item
		,ICfgAccessor CfgAccessor
	)where T: class{
		var Got = CfgAccessor.GetByPath(Item.Path);
		if(Got == null){
			return Item.DfltValue as T;
		}
		if (Got.Data == null){
			return null;
		}
		if(Got.Data is not T R){
			throw new ArgumentException("Got.Data is not T: "+typeof(T));
		}
		return R;
	}

	public static ICfgItem<object?> Mk(IList<str> Path, ICfgValue? DfltValue = null){
		return new CfgItem<object?>{Path=Path, DfltValue=DfltValue};
	}
/// <summary>
/// 如需列表則需定義潙IList<object> 不支持IList<str>等!
/// </summary>
/// <typeparam name="T2"></typeparam>
/// <param name="Path"></param>
/// <param name="DfltValue"></param>
/// <returns></returns>
	public static ICfgItem<T2> Mk<T2>(IList<str> Path, T2 DfltValue = default!){
		var V = new CfgValue(){Type=typeof(T2), Data=DfltValue};
		return new CfgItem<T2>{Path=Path, DfltValue=V};
	}


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
