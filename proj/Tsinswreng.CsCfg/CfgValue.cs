namespace Tsinswreng.CsCfg;
public  partial class CfgValue: ICfgValue{
	public Type? Type{get;set;}
	//用戶自定義
	public long TypeCode{get;set;}
	public object? Data{get;set;}
	public static ICfgValue Mk<T>(T Data){
		var R = new CfgValue();
		R.Type = typeof(T);
		R.Data = Data;
		return R;
	}
}
