namespace Tsinswreng.CsCfg.Json;

using System.Text.Encodings.Web;
using System.Text.Json;
using Tsinswreng.CsCore;

[Doc("""
從 UTF-8 JSON 流讀寫配置的源。

*兩條流*: `InputStream` 讀, `OutputStream` 寫; 兩者都不由本類關閉。
寫的時候整份重寫, 所以輸出流裡的鍵序就是 `Dict` 的鍵序, 註釋與原排版不會保留。
這是刻意的: 能被程序重寫的那份配置本來就不該放註釋, 手寫那份要保持原樣就得另存一個源。

*事件*: 繼承 `DictCfgAccessor`, 所以 Set / Rm / Reload / Save 都會觸發 `ICfgEvents`。
""")]
public partial class JsonCfgAccessor : DictCfgAccessor{

	[Doc(@$"讀的那份流; 沒設就沒有可讀的來源, 此後重載一律 no-op, 不拋。")]
	public Stream? InputStream{get;set;}

	[Doc(@$"寫的那份流; 沒設就保存不了。")]
	public Stream? OutputStream{get;set;}

	[Doc(@$"解析選項。默認允許註釋與尾逗號, 因為出廠配置是人手寫的。")]
	public JsonDocumentOptions DocOpts{get;set;} = new(){
		CommentHandling = JsonCommentHandling.Skip
		,AllowTrailingCommas = true
	};

	[Doc(@$"寫出選項。默認縮排、不轉義非 ASCII, 這樣寫出來的文件還是人能讀的。")]
	public JsonWriterOptions WriterOpts{get;set;} = new(){
		Indented = true
		,Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	[Doc("""
	#Prm[InputStream][讀的那份流; 可以傳 null。]
	#Prm[OutputStream][寫的那份流; 可以傳 null, 但那樣 Save 會拋。]
	#Prm[Name][源的技術標識。]
	構造後立刻載入一次; 載入失敗會拋。
	""")]
	public partial JsonCfgAccessor(Stream? InputStream, Stream? OutputStream, str Name = "json");

	[Doc(@$"立刻讀一次, 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。要事件就走 {nameof(Reload)}。")]
	public partial IDictionary<str, obj?> Load();

	[Doc(@$"立刻讀一次(異步), 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。")]
	public partial Task<IDictionary<str, obj?>> LoadAsy(CT Ct);

	[Doc(@$"把整棵配置樹寫到 {nameof(OutputStream)}。沒有輸出流就拋。")]
	private partial nil SaveToStream(IDictionary<str, obj?> Dict);

	[Doc(@$"同 {nameof(SaveToStream)}, 但異步。")]
	private partial Task<nil> SaveToStreamAsy(IDictionary<str, obj?> Dict);
}
