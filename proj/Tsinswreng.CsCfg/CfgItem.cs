using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;

public partial class CfgNode<T>:ICfgNode<T>{
	[Doc(@$"Path Separator.
	default is `/`, NOT `.`
	Examples([`Foo/Bar/Baz`])
	")]
	public const str PathSep = "/";
	public CfgNode(){}
	public IList<str> RelaPathSegs{get;set;} = [];
	public str? _FullPathCache{get;set;}
	public ICfgValue? DfltValue{get;set;}
	public ICfgNode? Parent{get;set;}
	public IList<ICfgNode>? Children{get;set;}

	public static ICfgNode<object?>Mk(
		ICfgNode? Parent
		,IList<str> Path
		,ICfgValue? DfltValue = null
	){
		return new CfgNode<object?>{RelaPathSegs=Path, DfltValue=DfltValue, Parent=Parent};
	}

/// 如需列表則需定義潙IList<object> 不支持IList<str>等!
	public static ICfgNode<T2> Mk<T2>(
		ICfgNode? Parent
		,IList<str> Path
		,T2 DfltValue = default!
	){
		var V = new CfgValue(){Type=typeof(T2), Data=DfltValue};
		return new CfgNode<T2>{RelaPathSegs=Path, DfltValue=V, Parent=Parent};
	}
}

