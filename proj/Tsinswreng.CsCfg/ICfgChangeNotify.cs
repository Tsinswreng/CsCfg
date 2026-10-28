namespace Tsinswreng.CsCfg;

using Tsinswreng.CsCore;

[Doc("""
一次配置變更。

Path 為 null 表示整體替換(例如 Reload); 非 null 表示單鍵變更。
Src 是真正提供該值的源; 由廣播方填, 訂閱者據它決定要不要理會。
""")]
public readonly record struct CfgChange(
	ICfgReader Src
	,IReadOnlyList<str>? Path
);

[Doc("""
變更廣播。*目前只有形狀, 尚未接到任何實現*。

*與 ICfgEvents 的分工*: ICfgEvents 是操作鉤子(Before / After 的 Set、Rm、Save、Reload),
適合審計與日誌; 本接口只回答「什麼變了」, 帶負載, 供訂閱者失效緩存或刷新界面。

*尚未實裝的三塊*:
一, 源在 Set / Rm / Reload 之後 raise; 二, 聚合層訂閱各源並轉發;
三, 檔案監視發現外部改動後呼叫 Reload, 這是整條鏈的觸發源, 沒有它事件只是空轉。
""")]
public interface ICfgChangeNotify{

	[Doc(@$"
	變更已發生。
	在觸發操作的那條線程上同步觸發, 庫不做線程切換; 界面要自己 marshal。
	回調期間不允許再改配置, 否則會遞歸。
	")]
	public event Action<CfgChange>? Changed;
}
