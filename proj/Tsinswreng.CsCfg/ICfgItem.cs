namespace Tsinswreng.CsCfg;

public partial interface ICfgItem{
//帶上Parent之Path纔是完整Path
	public IList<str> RelaPathSegs{get;set;}
	public str? _LazyFullPath{get;set;}
	public ICfgValue? DfltValue{get;set;}
	public ICfgItem? Parent{get;set;}
	public IList<ICfgItem>? Children{get;set;}

}
//勿用泛型ˉIList<> 緣讀配置旹只能得IList<obj>
public partial interface ICfgItem<T>:ICfgItem
{

}

