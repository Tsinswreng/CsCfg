namespace Tsinswreng.CsCfg;

public interface ICfgItem{
//帶上Parent之Path纔是完整Path
	public IList<str> RelaPath{get;set;}
	public ICfgValue? DfltValue{get;set;}
	public ICfgItem? Parent{get;set;}
	public IList<ICfgItem>? Children{get;set;}
}

public interface ICfgItem<T>:ICfgItem
{

}

