//Impl: 聲明見 JsonCfgCodec.cs。
namespace Tsinswreng.CsCfg.Json;

using System.Text.Json;
using Tsinswreng.CsCore;

public static partial class JsonCfgCodec{

	public static partial IDictionary<str, obj?> ParseObject(
		Stream Utf8
		,JsonDocumentOptions DocOpts
		,Func<IDictionary<str, obj?>> MkDict
	){
		// 空檔案是常態(首次啟動、剛建檔), 不是格式錯誤; 先擋掉再交給解析器
		if(IsEmpty(Utf8)){
			return MkDict();
		}
		using var Doc = JsonDocument.Parse(Utf8, DocOpts);
		var Root = Doc.RootElement;
		// 配置文件必須是對象; 根是數組或標量時沒辦法用路徑尋址
		if(Root.ValueKind != JsonValueKind.Object){
			throw new InvalidDataException($"The JSON root must be an object, but found {Root.ValueKind}.");
		}
		return (IDictionary<str, obj?>)ToValue(Root, MkDict)!;
	}

	public static partial async Task<IDictionary<str, obj?>> ParseObject(
		Stream Utf8
		,JsonDocumentOptions DocOpts
		,Func<IDictionary<str, obj?>> MkDict
		,CT Ct
	){
		if(IsEmpty(Utf8)){
			return MkDict();
		}
		using var Doc = await JsonDocument.ParseAsync(Utf8, DocOpts, Ct);
		var Root = Doc.RootElement;
		if(Root.ValueKind != JsonValueKind.Object){
			throw new InvalidDataException($"The JSON root must be an object, but found {Root.ValueKind}.");
		}
		return (IDictionary<str, obj?>)ToValue(Root, MkDict)!;
	}

	[Doc(@$"
	流裡還有沒有東西可讀。
	只有可 Seek 的流判斷得出來; 不可 Seek 的流只能交給解析器去撞。
	回 true 表示確定是空的, 此時解析器會把零字節當成格式錯誤而拋。
	")]
	private static bool IsEmpty(Stream Utf8){
		if(!Utf8.CanSeek){
			return false;
		}
		return Utf8.Length - Utf8.Position <= 0;
	}

	public static partial obj? ToValue(
		JsonElement El
		,Func<IDictionary<str, obj?>> MkDict
	){
		switch(El.ValueKind){
			case JsonValueKind.Object:{
				// 用調用方給的工廠造字典, 這樣鍵序與 Dict 的實作保持一致
				var R = MkDict();
				foreach(var P in El.EnumerateObject()){
					R[P.Name] = ToValue(P.Value, MkDict);
				}
				return R;
			}
			case JsonValueKind.Array:{
				var R = new List<obj?>();
				foreach(var Item in El.EnumerateArray()){
					R.Add(ToValue(Item, MkDict));
				}
				return R;
			}
			case JsonValueKind.String:
				return El.GetString();
			case JsonValueKind.Number:
				// 先試 i64, 裝不下再用 f64
				if(El.TryGetInt64(out var L)){
					return L;
				}
				if(El.TryGetDouble(out var D)){
					return D;
				}
				return null;
			case JsonValueKind.True:
				return true;
			case JsonValueKind.False:
				return false;
			default:
				// Null 與 Undefined 都算沒有值
				return null;
		}
	}

	public static partial nil Write(
		IDictionary<str, obj?> Dict
		,Stream Utf8
		,JsonWriterOptions WriterOpts
	){
		using(var W = new Utf8JsonWriter(Utf8, WriterOpts)){
			WriteValue(W, Dict);
		}
		// 寫入器的 Dispose 只把它自己那份推到流上; 流自己(例如 FileStream)還有一層緩衝。
		// 不沖的話, 另一個 FileStream 立刻去讀會讀到舊內容——存檔看起來成功, 檔案其實沒變。
		Utf8.Flush();
		return NIL;
	}

	public static partial async Task<nil> Write(
		IDictionary<str, obj?> Dict
		,Stream Utf8
		,JsonWriterOptions WriterOpts
		,CT Ct
	){
		var W = new Utf8JsonWriter(Utf8, WriterOpts);
		try{
			WriteValue(W, Dict);
			await W.FlushAsync(Ct);
		}finally{
			await W.DisposeAsync();
		}
		// 寫入器只是把字節推進流, 流本身也要沖一次
		await Utf8.FlushAsync(Ct);
		return NIL;
	}

	private static partial void WriteValue(
		Utf8JsonWriter W
		,obj? V
	){
		switch(V){
			case null:
				W.WriteNullValue();
				return;
			case str S:
				W.WriteStringValue(S);
				return;
			case bool B:
				W.WriteBooleanValue(B);
				return;
			case i8 or i16 or i32 or i64:
				W.WriteNumberValue(Convert.ToInt64(V));
				return;
			case u8 or u16 or u32 or u64:
				W.WriteNumberValue(Convert.ToUInt64(V));
				return;
			case f32 or f64:
				W.WriteNumberValue(Convert.ToDouble(V));
				return;
			case decimal M:
				W.WriteNumberValue(M);
				return;
			case IDictionary<str, obj?> D:
				W.WriteStartObject();
				foreach(var Kv in D){
					W.WritePropertyName(Kv.Key);
					WriteValue(W, Kv.Value);
				}
				W.WriteEndObject();
				return;
			case IEnumerable<obj?> L:
				W.WriteStartArray();
				foreach(var Item in L){
					WriteValue(W, Item);
				}
				W.WriteEndArray();
				return;
			default:
				// 值模型以外的型別(DateTime、enum 等)寫成字符串, 讀回來就是字符串
				W.WriteStringValue(V.ToString());
				return;
		}
	}
}
