namespace Tsinswreng.CsCfg;

public  partial interface I_CfgDict{
	public IDictionary<str, object?> CfgDict{get;set;}
#if Impl
	= new Dictionary<str, object?>();
#endif
}
