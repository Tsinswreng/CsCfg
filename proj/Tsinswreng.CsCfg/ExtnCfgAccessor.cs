namespace Tsinswreng.CsCfg;

public static class ExtnICfgAccessor{
	extension(ICfgAccessor z){

	}
	public static Func<ICfgItem<T>, T?> MkFnGet<T>(this ICfgAccessor z){
		return (CfgItem)=>{
			return CfgItem.GetFrom(z);
		};
	}

	public static T? Get<T>(
		this ICfgAccessor z
		,ICfgItem<T> CfgItem
	){
		return CfgItem.GetFrom(z);
	}

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
