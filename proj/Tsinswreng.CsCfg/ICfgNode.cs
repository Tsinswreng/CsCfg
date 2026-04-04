using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;

[Doc("Config Node")]
public partial interface ICfgNode{
	[Doc(@$"Relative Path Segments.
	without path of `{nameof(Parent)}`
	")]
	public IList<str> RelaPathSegs{get;set;}
	public str? _FullPathCache{get;set;}
	[Doc(@$"Default Value")]
	public obj? DfltValueObj{get;set;}
	public ICfgNode? Parent{get;set;}
	public IList<ICfgNode>? Children{get;set;}

}

public partial interface ICfgNode<T>:ICfgNode{
	public T? DfltValue{get;set;}
}
