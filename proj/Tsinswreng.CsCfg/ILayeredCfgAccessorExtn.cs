namespace Tsinswreng.CsCfg;

using Tsinswreng.CsCore;

[Doc($$"""
{{nameof(ILayeredCfgAccessor)}} 的裝配便捷寫法。

*為何要有這個擴展*: 「讀源在前、可讀寫源在後、寫入只落可寫那個」在型別上沒有任何東西擋著,
唯一正確的寫法是「先 SetSrcs, 再設 Writable」, 順序反了 setter 會拋。
這個順序陷阱原本每個宿主都要自己記一次, 所以在這裡收成一個入口。

*為何掛在接口上*: 呼叫方拿到的是 {{nameof(ILayeredCfgAccessor)}}, 不是某個實現類;
擴展掛在接口上, 換實現時不需要改呼叫端。
""")]
public static partial class ILayeredCfgAccessorExtn{

	[Doc($$"""
	接上多個只讀源與一個可讀寫源: {{nameof(RoSrcs)}} 依序在前、{{nameof(Rw)}} 在最後, 寫入只落 {{nameof(Rw)}}。

	*語義*: 同一個鍵在多個源都有時, 排在前面的贏。讀源給多個就得到多層覆蓋,
	例如「環境變量源 > 出廠配置檔 > 預設源」; 最後那個可寫源只在前面都沒有這個鍵時才被命中。
	代價是界面上改了卻看不出為什麼沒生效, 所以取值時 `TryGet` 會吐出命中的源,
	設定頁可以據此顯示「這一項由哪個源接管」。

	*與檔案無關*: 本方法不知道路徑、檔名或「某個配置鍵指向配置檔」這種事,
	源從哪裡來、怎麼裝配, 都是呼叫方(應用)的事。

	#Prm[{{nameof(RoSrcs)}}][只讀源清單, 順序即優先級, 前面優先。可以為空, 那就是只有一個可寫源。]
	#Prm[{{nameof(Rw)}}][唯一可寫源, 優先級最低。]
	#Throw[{{nameof(ArgumentNullException)}}][{{nameof(RoSrcs)}} 或 {{nameof(Rw)}} 為 null, 或清單裡有 null 元素時。]
	#Throw[{{nameof(ArgumentException)}}][{{nameof(Rw)}} 也在 {{nameof(RoSrcs)}} 裡時: 同一個源既只讀又可寫, 語義說不清, 事件也會訂兩次。]
	""")]
	public static partial nil SetRoRw(
		this ILayeredCfgAccessor z
		,IReadOnlyList<ICfgReader> RoSrcs
		,ICfgAccessor Rw
	);
}
