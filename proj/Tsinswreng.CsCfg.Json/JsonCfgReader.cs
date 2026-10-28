namespace Tsinswreng.CsCfg.Json;

using System.Text.Json;
using Tsinswreng.CsCore;

[Doc("""
從 UTF-8 JSON 流讀配置的源, 只讀。

*流的所有權*: 本類不關閉 `InputStream`; 用完由調用方負責。
*重讀*: 每次重讀都回到流的開頭, 所以流必須可 Seek; 不可 Seek 的流只有第一次讀得到。
*根形狀*: JSON 根必須是對象, 否則拋; 數組或標量沒辦法用路徑尋址。
""")]
public partial class JsonCfgReader : DictCfgReader{

	[Doc(@$"讀的那份流; 沒設就沒有來源。")]
	public Stream? InputStream{get;set;}

	[Doc(@$"解析選項。默認允許註釋與尾逗號, 因為出廠配置是人手寫的。")]
	public JsonDocumentOptions DocOpts{get;set;} = new(){
		CommentHandling = JsonCommentHandling.Skip
		,AllowTrailingCommas = true
	};

	[Doc("""
	#Prm[InputStream][讀的那份流; 可以傳 null, 之後再設並呼叫 Reload。]
	#Prm[Name][源的技術標識。]
	構造後立刻載入一次; 載入失敗會拋。
	""")]
	public partial JsonCfgReader(Stream? InputStream, str Name = "json");

	[Doc(@$"立刻讀一次, 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。要事件就走 {nameof(Reload)}。")]
	public partial IDictionary<str, obj?> Load();

	[Doc(@$"立刻讀一次(異步), 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。")]
	public partial Task<IDictionary<str, obj?>> Load(CT Ct);
}
