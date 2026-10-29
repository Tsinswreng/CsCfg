namespace Tsinswreng.CsCfg.Json;

using System.Text.Json;
using Tsinswreng.CsCore;

[Doc("""
從 UTF-8 JSON 流讀配置的源, 只讀。

*流從哪來*: 由建構子給, 或由子類覆寫 `OpenInputStream` 自己取得。
第一種情形本類不關閉那條流, 用完由調用方負責; 第二種情形由覆寫的類在 `CloseInputStream` 裡關掉。
這條界線就是「檔案系統留在最後一層」的落點: 本類不認識檔案, 只在拿到流之後解析。

*重讀*: 每次重讀都重新向 `OpenInputStream` 要一條流, 所以檔案源可以每次重開檔案;
本類自己則回到同一條流的開頭, 因此那條流必須可 Seek, 不可 Seek 的流只有第一次讀得到。

*根形狀*: JSON 根必須是對象, 否則拋; 數組或標量沒辦法用路徑尋址。
""")]
public partial class JsonCfgReader : DictCfgReader{

	[Doc(@$"讀的那份流; 沒設就沒有來源, 此後重載一律 no-op, 不拋。子類覆寫 {nameof(OpenInputStream)} 之後可以不使用它。")]
	public Stream? InputStream{get;set;}

	[Doc(@$"解析選項。默認允許註釋與尾逗號, 因為配置檔常由人手寫。默認值見 {nameof(JsonCfgOpts)}。")]
	public JsonDocumentOptions DocOpts{get;set;} = JsonCfgOpts.DfltDocOpts;

	[Doc($$"""
	#Prm[{{nameof(InputStream)}}][讀的那份流; 可以傳 null, 之後再設並呼叫 Reload。]
	#Prm[{{nameof(Name)}}][源的技術標識。]
	構造後立刻載入一次; 載入失敗會拋。
	""")]
	public partial JsonCfgReader(Stream? InputStream, str Name = "json");

	[Doc(@$"
	取得一條要讀的流, 每次讀取(含建構時那一次)都會呼叫它。

	默認實作回 {nameof(InputStream)}; 檔案源的子類覆寫它, 每次重新打開檔案。
	回 null 表示這次讀取什麼都不做, 保留當前的 {nameof(Dict)}。
	")]
	protected virtual Stream? OpenInputStream(){ return InputStream; }

	[Doc(@$"
	用完一條讀取流之後的通知, 與 {nameof(OpenInputStream)} 成對。
	默認什麼都不做: 建構子給的流由調用方負責關閉; 子類自己開的流要在這裡關掉。
	")]
	protected virtual nil CloseInputStream(Stream Utf8){ return NIL; }

	[Doc(@$"立刻讀一次, 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。要事件就走 {nameof(Reload)}。")]
	public partial IDictionary<str, obj?> Load();

	[Doc(@$"立刻讀一次(異步), 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。")]
	public partial Task<IDictionary<str, obj?>> Load(CT Ct);
}
