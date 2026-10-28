namespace Tsinswreng.CsCfg;
public partial class JsonFileCfgAccessor:ICfgEvents{
	public event EventHandler<ICfgChange>? BeforeSetByPath;
	public event EventHandler<ICfgChange>? BeforeRmPath;
	public event EventHandler<ICfgChange>? BeforeReload;
	public event EventHandler<ICfgChange>? BeforeSave;

	public event EventHandler<ICfgChange>? AfterSetByPath;
	public event EventHandler<ICfgChange>? AfterRmPath;
	public event EventHandler<ICfgChange>? AfterReload;
	public event EventHandler<ICfgChange>? AfterSave;

}
