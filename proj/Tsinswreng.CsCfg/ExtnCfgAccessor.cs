namespace Tsinswreng.CsCfg;

public static class ExtnCfgAccessor{
	public static nil Set<T>(
		this ICfgAccessor z
		,ICfgItem CfgItem
		,T Value
	){
		z.SetBoxedByPathNonSave(
			CfgItem.GetFullPathSegs()
			,new CfgValue(){Type=typeof(T), Data=Value}
		);
		return NIL;
	}
}
