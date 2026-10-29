namespace Tsinswreng.CsCfg;

using Tsinswreng.CsCore;

[Doc($$"""
{{nameof(LayeredCfgAccessor)}} 的裝配便捷寫法。

*為何要有這個擴展*: 「只讀源在前、可讀寫源在後、寫入只落可寫那個」在型別上沒有任何東西擋著,
唯一正確的寫法是「先 SetSrcs, 再設 Writable」——順序反了 setter 會拋。
這個順序陷阱原本每個宿主都要自己記一次, 所以在這裡收成一個入口。
""")]
public static partial class LayeredCfgAccessorExtn{

	[Doc($$"""
	接上兩個源: {{nameof(Ro)}} 只讀在前、{{nameof(Rw)}} 可讀寫在後, 寫入只落 {{nameof(Rw)}}。

	*語義*: 同一個鍵兩邊都有時, 前面那個贏。呼叫方把只讀源放前面, 就得到「只讀優先」——
	這是本方法叫 {{nameof(SetRoRw)}} 而不是「隨便給兩個源」的原因。
	代價是界面上改了卻看不出為什麼沒生效, 所以取值時 `TryGet` 會吐出命中的源,
	設定頁可以據此顯示「這一項由哪個源接管」。呼叫方若想要相反的方向, 就不要用本方法,
	自己按需要的順序呼叫 `SetSrcs`。

	*與檔案無關*: 本方法不知道路徑、檔名或「某個配置鍵指向配置檔」這種事,
	兩份檔放在哪裡、怎麼找到, 都是呼叫方(應用)的事。

	#Prm[{{nameof(Ro)}}][只讀源, 優先級較高。]
	#Prm[{{nameof(Rw)}}][唯一可寫源, 優先級較低。]
	#Throw[{{nameof(ArgumentNullException)}}][任一參數為 null 時。]
	#Throw[{{nameof(ArgumentException)}}][兩者是同一個物件時: 那等於同一個源既只讀又可寫, 而且事件會訂兩次。]
	""")]
	public static partial nil SetRoRw(this LayeredCfgAccessor z, ICfgReader Ro, ICfgAccessor Rw);
}


