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
	public obj? DfltValueObj{get;set;}
	public T? DfltValue{
		get=>DfltValueObj is null?default:(T?)DfltValueObj;
		set=>DfltValueObj = value;
	}
	public ICfgNode? Parent{get;set;}
	public IList<ICfgNode>? Children{get;set;}

	public static ICfgNode<object?> Mk(
		ICfgNode? Parent
		,IList<str> Path
		,obj? DfltValue = null
	){
		return new CfgNode<object?>{RelaPathSegs=Path, DfltValue=DfltValue, Parent=Parent};
	}

	public static ICfgNode<T2> Mk<T2>(
		ICfgNode? Parent
		,IList<str> Path
		,T2 DfltValue = default!
	){
		return new CfgNode<T2>{RelaPathSegs=Path, DfltValue=DfltValue, Parent=Parent};
	}
}
