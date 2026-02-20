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

/// 如需列表則需定義潙IList<object> 不支持IList<str>等!
	public static ICfgItem<T2> Mk<T2>(
		ICfgItem? Parent
		,IList<str> Path
		,T2 DfltValue = default!
	){
		var V = new CfgValue(){Type=typeof(T2), Data=DfltValue};
		return new CfgItem<T2>{RelaPathSegs=Path, DfltValue=V, Parent=Parent};
	}
}

