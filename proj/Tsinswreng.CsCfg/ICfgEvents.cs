using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;
public  partial interface ICfgEvents{
	[Impl(typeof(ICfgEvents))]
	public event EventHandler? BeforeGetByPath;
	[Impl(typeof(ICfgEvents))]
	public event EventHandler? BeforeSetByPath;
	[Impl(typeof(ICfgEvents))]
	public event EventHandler? BeforeRmPath;
	[Impl(typeof(ICfgEvents))]
	public event EventHandler? BeforeReLoad;
	[Impl(typeof(ICfgEvents))]
	public event EventHandler? BeforeSave;


	[Impl(typeof(ICfgEvents))]
	public event EventHandler? AfterGetByPath;
	[Impl(typeof(ICfgEvents))]
	public event EventHandler? AfterSetByPath;
	[Impl(typeof(ICfgEvents))]
	public event EventHandler? AfterRmPath;
	[Impl(typeof(ICfgEvents))]
	public event EventHandler? AfterReLoad;
	[Impl(typeof(ICfgEvents))]
	public event EventHandler? AfterSave;
}
