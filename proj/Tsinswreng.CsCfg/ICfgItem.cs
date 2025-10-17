namespace Tsinswreng.CsCfg;

public partial interface ICfgItem{
//帶上Parent之Path纔是完整Path
	public IList<str> RelaPathSegs{get;set;}
	public ICfgValue? DfltValue{get;set;}
	public ICfgItem? Parent{get;set;}
	public IList<ICfgItem>? Children{get;set;}
}

public partial interface ICfgItem<T>:ICfgItem
{

}

