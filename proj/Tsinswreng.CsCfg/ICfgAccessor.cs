namespace Tsinswreng.CsCfg;

public interface ICfgAccessor{
	public ICfgValue? GetByPath(IList<str> Path);
	public nil SetByPath(IList<str> Path, ICfgValue Value);
	public nil RmPath(IList<str> Path);
}


