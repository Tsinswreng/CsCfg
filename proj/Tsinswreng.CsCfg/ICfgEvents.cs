using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;

[Doc("""
配置的操作鉤子與變更信號。與 ICfgReader / ICfgAccessor 分開, 由需要的實現類同時實現。

*Before 系列*: 將要執行時觸發。事件裡的 Path 是目標路徑, Src 是將要寫入的那個源。
適合審計與防護性攔截; 它會伴隨失敗一起觸發, 因為那時確實嘗試過了。

*After 系列*: 執行成功之後觸發。失敗不觸發。

*哪些是「值變了」的信號*: 只有 AfterSetByPath、AfterRmPath、AfterReload 三個。
AfterSave 只表示檔案寫完了, 內存沒動; 拿它當刷新信號會多刷一次。

*線程與重入*: 在觸發操作的那條線程上同步觸發, 庫不做線程切換。
回調期間不允許再改配置, 否則會遞歸。
""")]
public partial interface ICfgEvents{

	[Doc(@$"將要按路徑寫值。")]
	public event EventHandler<ICfgChange>? BeforeSetByPath;

	[Doc(@$"將要按路徑刪鍵。")]
	public event EventHandler<ICfgChange>? BeforeRmPath;

	[Doc(@$"將要整體重讀。")]
	public event EventHandler<ICfgChange>? BeforeReload;

	[Doc(@$"將要落盤。")]
	public event EventHandler<ICfgChange>? BeforeSave;

	[Doc(@$"寫值成功。這是「值變了」的信號。")]
	public event EventHandler<ICfgChange>? AfterSetByPath;

	[Doc(@$"刪鍵成功。這是「值變了」的信號。")]
	public event EventHandler<ICfgChange>? AfterRmPath;

	[Doc(@$"重讀完成。這是「值變了」的信號, Path 為 null 表示整份換掉。")]
	public event EventHandler<ICfgChange>? AfterReload;

	[Doc(@$"落盤完成。不是「值變了」的信號: 內存沒動, 只是檔案寫完了。")]
	public event EventHandler<ICfgChange>? AfterSave;
}
