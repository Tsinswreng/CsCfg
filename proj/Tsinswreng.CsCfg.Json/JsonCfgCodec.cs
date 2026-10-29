namespace Tsinswreng.CsCfg.Json;

using System.Text.Json;
using Tsinswreng.CsCore;

[Doc("""
JSON 樹與本庫值模型之間的轉換。

*值模型*: 標量(字符串、整數族、浮點、布林)、null、`IList<obj?>`、`IDictionary<str, obj?>`。
轉換只認這幾種; 其餘型別(例如 DateTime、enum)寫出時會被寫成字符串, 讀回來就是字符串。

*為何不走 IJsonSerializer*: 反序列化成 `object` 時 STJ 會給 `JsonElement`, 不是本庫要的標量;
而且那條路要走 `TypeInfoResolver`, 反射被關掉的環境下直接拋。
直接走 `JsonDocument` 與 `Utf8JsonWriter` 不需要任何型別契約。

*數字*: 解析時優先 `i64`, 裝不下再 `f64`; 這與值模型的約定一致。
""")]
public static partial class JsonCfgCodec{

	[Doc(@$"把 UTF-8 JSON 文本解析成配置樹。根必須是對象, 否則拋 {nameof(InvalidDataException)}。空流回空字典。")]
	public static partial IDictionary<str, obj?> ParseObject(
		Stream Utf8
		,JsonDocumentOptions DocOpts
		,Func<IDictionary<str, obj?>> MkDict
	);

	[Doc(@$"同 {nameof(ParseObject)}, 但異步讀流。")]
	public static partial Task<IDictionary<str, obj?>> ParseObject(
		Stream Utf8
		,JsonDocumentOptions DocOpts
		,Func<IDictionary<str, obj?>> MkDict
		,CT Ct
	);

	[Doc(@$"一個 JSON 節點 → 值模型。")]
	public static partial obj? ToValue(
		JsonElement El
		,Func<IDictionary<str, obj?>> MkDict
	);

	[Doc(@$"把配置樹寫成 UTF-8 JSON。")]
	public static partial nil Write(
		IDictionary<str, obj?> Dict
		,Stream Utf8
		,JsonWriterOptions WriterOpts
	);

	[Doc(@$"同 {nameof(Write)}, 但異步寫流。")]
	public static partial Task<nil> Write(
		IDictionary<str, obj?> Dict
		,Stream Utf8
		,JsonWriterOptions WriterOpts
		,CT Ct
	);

	[Doc(@$"值模型 → JSON 寫入器。值模型以外的型別一律寫成字符串。")]
	private static partial void WriteValue(
		Utf8JsonWriter W
		,obj? V
	);
}
