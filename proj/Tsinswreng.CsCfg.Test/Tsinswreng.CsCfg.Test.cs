namespace Tsinswreng.CsCfg.Test;

using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.CsTreeTest;

/// 測試專案的 CLR 入口: 建容器、註冊 tester、跑整棵測試樹。
internal class Program{
	public static IServiceCollection SvcColct = new ServiceCollection();
	public static IServiceProvider SvcProvdr = null!;

	public static async Task Main(str[] args){
		var Mgr = CsCfgTestMgr.Inst;
		SvcProvdr = Mgr.InitSvc(SvcColct, sc => sc.BuildServiceProvider());

		ITestExecutor Executor = new TreeTestExecutor();
		await Executor.RunEtPrint(Mgr.TestNode);
	}
}
