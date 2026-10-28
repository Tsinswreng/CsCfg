using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;

[Doc("""
多源配置的門面: 一組有序的源, 前優先; 其中最多一個可寫, 寫入只落它。

*為何不另立一套「源」接口*: 源與門面的結構完全一樣, 都是「按路徑取值加自我重載」,
所以源就用 ICfgReader 表示, 可寫的源就是 ICfgAccessor。真正的差別只在這一層。

*優先級*: 依 Srcs 的順序取第一個命中的源; 命中即停, 即使值是 null。
null 也是值, 上層寫了 null 就是明確遮蔽下層。

*寫入*: 只路由到 Writable。它必須是 Srcs 的一員, 否則寫進去的東西讀不回來。
Writable 為 null 表示整份配置只讀。
""")]
public interface ILayeredCfgAccessor : ICfgAccessor{

	[Doc(@$"有序的源; 順序即優先級, 前優先。")]
	public IList<ICfgReader> Srcs{get;set;}

	[Doc(@$"唯一可寫的源; 為 null 表示整份配置只讀。")]
	public ICfgAccessor? Writable{get;set;}

}
