namespace Tsinswreng.CsCfg.Json;

using System.Text.Encodings.Web;
using System.Text.Json;
using Tsinswreng.CsCore;

[Doc($$"""
JSON 源的默認解析／寫出選項，只在本程序集內用。

*為何要抽出來*：{{nameof(JsonCfgReader)}}、{{nameof(JsonCfgAccessor)}} 與檔案式的那兩個源都要同一組默認值。
各寫一份的話，改一處就會出現「流式源允許註釋、檔案源不允許」這種不一致，而且很難看出來。

*為何可以是共用欄位*：`JsonDocumentOptions` 與 `JsonWriterOptions` 都是結構體，
讀取時是整份複製，所以共用一份不會讓兩個源的選項互相影響。
""")]
internal static class JsonCfgOpts{

	[Doc("默認解析選項: 允許註釋與尾逗號, 因為配置檔常由人手寫。")]
	internal static readonly JsonDocumentOptions DfltDocOpts = new(){
		CommentHandling = JsonCommentHandling.Skip
		,AllowTrailingCommas = true
	};

	[Doc("默認寫出選項: 縮排、不轉義非 ASCII, 這樣寫出來的文件還是人能讀的。")]
	internal static readonly JsonWriterOptions DfltWriterOpts = new(){
		Indented = true
		,Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};
}
