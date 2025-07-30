namespace Tsinswreng.CsCfg;
public partial class JsonFileCfgAccessor{
	public event EventHandler? BeforeGetByPath;
	public event EventHandler? BeforeSetByPath;
	public event EventHandler? BeforeRmPath;
	public event EventHandler? BeforeReLoad;
	public event EventHandler? BeforeSave;

	public event EventHandler? AfterGetByPath;
	public event EventHandler? AfterSetByPath;
	public event EventHandler? AfterRmPath;
	public event EventHandler? AfterReLoad;
	public event EventHandler? AfterSave;

}
