namespace Tsinswreng.CsCfg;
[Obsolete("事件只能在內類觸發")]
public  partial class CfgEvents: ICfgEvents{
	public event EventHandler BeforeGetByPath = null!;
	public event EventHandler BeforeSetByPath = null!;
	public event EventHandler BeforeRmPath = null!;
	public event EventHandler BeforeReLoad = null!;
	public event EventHandler BeforeSave = null!;

	public event EventHandler AfterGetByPath = null!;
	public event EventHandler AfterSetByPath = null!;
	public event EventHandler AfterRmPath = null!;
	public event EventHandler AfterReLoad = null!;
	public event EventHandler AfterSave = null!;

}
