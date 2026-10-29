namespace Tsinswreng.CsCfg.Json;

using System.Text.Json;
using Tsinswreng.CsCore;

[Doc("""
從 UTF-8 JSON 流讀寫配置的源。

*流從哪來*: 由建構子給, 或由子類覆寫 `OpenInputStream`／`OpenOutputStream` 自己取得。
第一種情形本類不關閉那些流, 用完由調用方負責; 第二種情形由覆寫的類在對應的 `Close*` 裡關掉。
這條界線就是「檔案系統留在最後一層」的落點: 本類不認識檔案, 只在拿到流之後解析與寫出。

*寫的時候整份重寫*, 所以輸出流裡的鍵序就是 `Dict` 的鍵序, 註釋與原排版不會保留。
這是刻意的: 能被程序重寫的那份配置本來就不該放註釋, 手寫那份要保持原樣就得另存一個源。

*事件*: 繼承 `DictCfgAccessor`, 所以 Set / Rm / Reload / Save 都會觸發 `ICfgEvents`。
""")]
public partial class JsonCfgAccessor : DictCfgAccessor{

	[Doc(@$"讀的那份流; 沒設就沒有可讀的來源, 此後重載一律 no-op, 不拋。子類覆寫 {nameof(OpenInputStream)} 之後可以不使用它。")]
	public Stream? InputStream{get;set;}

	[Doc(@$"寫的那份流; 沒設就保存不了。子類覆寫 {nameof(OpenOutputStream)} 之後可以不使用它。")]
	public Stream? OutputStream{get;set;}

	[Doc(@$"解析選項。默認允許註釋與尾逗號, 因為配置檔常由人手寫。默認值見 {nameof(JsonCfgOpts)}。")]
	public JsonDocumentOptions DocOpts{get;set;} = JsonCfgOpts.DfltDocOpts;

	[Doc(@$"寫出選項。默認縮排、不轉義非 ASCII, 這樣寫出來的文件還是人能讀的。默認值見 {nameof(JsonCfgOpts)}。")]
	public JsonWriterOptions WriterOpts{get;set;} = JsonCfgOpts.DfltWriterOpts;

	[Doc($$"""
	#Prm[{{nameof(InputStream)}}][讀的那份流; 可以傳 null。]
	#Prm[{{nameof(OutputStream)}}][寫的那份流; 可以傳 null, 但那樣 Save 會拋。]
	#Prm[{{nameof(Name)}}][源的技術標識。]
	構造後立刻載入一次; 載入失敗會拋。
	""")]
	public partial JsonCfgAccessor(Stream? InputStream, Stream? OutputStream, str Name = "json");

	[Doc(@$"立刻讀一次, 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。要事件就走 {nameof(Reload)}。")]
	public partial IDictionary<str, obj?> Load();

	[Doc(@$"立刻讀一次(異步), 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。")]
	public partial Task<IDictionary<str, obj?>> Load(CT Ct);

	[Doc(@$"
	取得一條要讀的流, 每次讀取(含建構時那一次)都會呼叫它。
	默認實作回 {nameof(InputStream)}; 檔案源的子類覆寫它, 每次重新打開檔案。回 null 表示這次讀取什麼都不做。
	")]
	protected virtual Stream? OpenInputStream(){ return InputStream; }

	[Doc(@$"
	用完一條讀取流之後的通知, 與 {nameof(OpenInputStream)} 成對。
	默認什麼都不做: 建構子給的流由調用方負責關閉; 子類自己開的流要在這裡關掉。
	")]
	protected virtual nil CloseInputStream(Stream Utf8){ return NIL; }

	[Doc(@$"
	取得一條要寫的流, 每次保存都會呼叫它。
	默認實作回 {nameof(OutputStream)}; 檔案源的子類覆寫它, 每次打開暫存檔。回 null 表示沒有可寫的目標, 保存會拋。
	")]
	protected virtual Stream? OpenOutputStream(){ return OutputStream; }

	[Doc($$"""
	寫完(或寫失敗)之後的通知, 與 {{nameof(OpenOutputStream)}} 成對。
	檔案源的子類在這裡關閉暫存檔, 並依 {{nameof(Ok)}} 決定搬移覆蓋或刪除。
	默認什麼都不做: 建構子給的流由調用方負責關閉。
	#Prm[{{nameof(Utf8)}}][剛才寫的那條流。]
	#Prm[{{nameof(Ok)}}][寫入是否成功。]
	""")]
	protected virtual nil CloseOutputStream(Stream Utf8, bool Ok){ return NIL; }

	[Doc(@$"把整棵配置樹寫到 {nameof(OutputStream)}。沒有輸出流就拋。")]
	private partial nil SaveToStream(IDictionary<str, obj?> Dict);

	[Doc(@$"同 {nameof(SaveToStream)}, 但異步。")]
	private partial Task<nil> SaveToStream(IDictionary<str, obj?> Dict, CT Ct);
}
