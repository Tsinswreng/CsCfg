// #define Impl
// using Tsinswreng.CsCore;

// namespace Tsinswreng.CsCfg;

// public  partial class MultiSrcCfg
// 	:ICfgAccessor
// 	,ICfgEvents
// {

// 	public IList<ICfgAccessor> CfgSrc{get;set;} = new List<ICfgAccessor>();


// 	[Impl(typeof(ICfgAccessor))]
// 	public ICfgValue? GetByPath(IList<string> Path) {
// 		ICfgValue? R = null;
// 		foreach(var Accessor in CfgSrc){
// 			R = Accessor.GetByPath(Path);
// 			if(R != null){
// 				return R;
// 			}
// 		}
// 		return R;
// 	}

// 	[Impl(typeof(ICfgAccessor))]
// 	public nil ReLoad() {
// 		CfgSrc.Select(x=>x.ReLoad());
// 		return NIL;
// 	}

// 	[Impl(typeof(ICfgAccessor))]
// 	public async Task<nil> ReLoadAsy(CT Ct) {
// 		foreach(var CfgAcc in CfgSrc){
// 			await CfgAcc.ReLoadAsy(Ct);
// 		}
// 		return NIL;
// 	}
// 	[Impl]

// 	public nil RmPath(IList<string> Path) {
// 		throw new NotImplementedException();
// 	}
// 	[Impl]
// 	public nil Save() {
// 		throw new NotImplementedException();
// 	}

// 	[Impl]
// 	public Task<nil> SaveAsy(CT Ct) {
// 		throw new NotImplementedException();
// 	}
// 	[Impl]
// 	public nil SetByPath(IList<string> Path, ICfgValue Value) {
// 		throw new NotImplementedException();
// 	}


// 	[Impl(typeof(ICfgEvents))]
// 	public event EventHandler? BeforeGetByPath;
// 	[Impl(typeof(ICfgEvents))]
// 	public event EventHandler? BeforeSetByPath;
// 	[Impl(typeof(ICfgEvents))]
// 	public event EventHandler? BeforeRmPath;
// 	[Impl(typeof(ICfgEvents))]
// 	public event EventHandler? BeforeReLoad;
// 	[Impl(typeof(ICfgEvents))]
// 	public event EventHandler? BeforeSave;


// 	[Impl(typeof(ICfgEvents))]
// 	public event EventHandler? AfterGetByPath;
// 	[Impl(typeof(ICfgEvents))]
// 	public event EventHandler? AfterSetByPath;
// 	[Impl(typeof(ICfgEvents))]
// 	public event EventHandler? AfterRmPath;
// 	[Impl(typeof(ICfgEvents))]
// 	public event EventHandler? AfterReLoad;
// 	[Impl(typeof(ICfgEvents))]
// 	public event EventHandler? AfterSave;

// }
