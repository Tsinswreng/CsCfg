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

*為何 Srcs 對外只讀*: 源的增減牽涉事件的訂閱與退訂。若讓調用方直接改 IList,
一定有人忘了退訂, 於是出現「已移除的源還在廣播」「新加的源不廣播」「同一個源訂兩次」。
改動一律走下面三個方法, 由實現負責掛鉤。
""")]
public interface ILayeredCfgAccessor : ICfgAccessor{

	[Doc(@$"有序的源; 順序即優先級, 前優先。對外只讀, 改動走下面的方法。")]
	public IReadOnlyList<ICfgReader> Srcs{get;}

	[Doc(@$"
	唯一可寫的源; 為 null 表示整份配置只讀。
	必須是 Srcs 的一員, 所以要先把源加進去, 再設它。
	")]
	public ICfgAccessor? Writable{get;set;}

	[Doc(@$"
	追加一個源到末尾, 也就是優先級最低。
	已在清單裡的同一個源會被忽略: 重複加入等於事件訂兩次、廣播兩次。
	")]
	public void AddSrc(ICfgReader Src);

	[Doc(@$"
	移除一個源。
	若它正好是可寫源, Writable 會一併清成 null, 否則寫入會落進一個讀不回來的源。
	")]
	public void RemoveSrc(ICfgReader Src);

	[Doc(@$"
	整體替換源清單, 順序即優先級。重複的源只保留第一次出現的位置。
	替換後若 Writable 不在新清單裡, 會一併清成 null。
	")]
	public void SetSrcs(IReadOnlyList<ICfgReader> Srcs);
}
