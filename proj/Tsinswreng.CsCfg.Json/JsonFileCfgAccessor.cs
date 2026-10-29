namespace Tsinswreng.CsCfg.Json;

using System.Text.Json;
using Tsinswreng.CsCore;

[Doc($$"""
以一份 JSON 檔案為來源的可讀可寫配置源。

*為什麼不是繼承 {{nameof(JsonCfgAccessor)}}*: 那個類的 `Load()`／`SaveToStream()` 都是接到它持有的流上;
本類沒有流, 繼承就會得到「呼叫了卻什麼都沒發生」或「一呼叫就拋」的方法。
比缺功能更難查, 所以本類直接建在 {{nameof(DictCfgAccessor)}} 上, 自己實現讀寫;
JSON 的編解碼仍走 {{nameof(JsonCfgCodec)}}, 格式邏輯沒有第二份。

*寫入是原子的*: 先寫 `路徑.tmp` 再覆蓋搬移(見 {{nameof(JsonCfgFileIo)}})。
中途失敗最多留一個孤兒暫存檔, 正式檔要嘛是舊的完整版、要嘛是新的完整版。

*建構即讀一次*: 兩個理由。一是免得呼叫方忘了 `Load()` 就去取值;
二是必須先把檔案既有內容讀進來, 否則第一次 {{nameof(Save)}} 會把原本的鍵整份蓋掉——
那些鍵可能不是本進程寫的(人手改過、舊版本留下的)。

*整份重寫的代價*: 檔案裡的註釋、縮排、鍵序都保不住。需要保住註釋的那份配置,
要另存一個唯讀源(例如 {{nameof(JsonFileCfgReader)}}), 由業務決定兩者的優先級。

#Eg[
	```cs
	var Rw = new JsonFileCfgAccessor("/data/app/cfg.rw.jsonc", "gui");
	Rw.SetNoSave(Key, "en");    // Key: 任意 IKeyNode<T>
	await Rw.Save(Ct);          // 整份重寫, 原子完成
	```
]
""")]
public partial class JsonFileCfgAccessor : DictCfgAccessor{

	[Doc(@$"
	配置檔的絕對路徑。建構時正規化, 理由同 {nameof(JsonFileCfgReader.Path)}。

	*為何是 init-only*: 路徑是這個源的身份, 換檔案應該換一個源;
	允許事後改會讓讀寫落到與外部認知不同的檔案上。
	")]
	public str Path{get;init;} = "";

	[Doc(@$"解析選項。默認允許註釋與尾逗號, 因為配置檔常由人手寫。")]
	public JsonDocumentOptions DocOpts{get;set;} = JsonCfgOpts.DfltDocOpts;

	[Doc(@$"寫出選項。默認縮排、不轉義非 ASCII, 這樣寫出來的文件還是人能讀的。")]
	public JsonWriterOptions WriterOpts{get;set;} = JsonCfgOpts.DfltWriterOpts;

	[Doc($$"""
	接上一份配置檔, 立刻讀一次, 之後的讀寫都落在這一份檔案上。

	#Prm[{{nameof(Path)}}][配置檔路徑; 可以相對, 會正規化成絕對路徑。]
	#Prm[{{nameof(Name)}}][源的技術標識, 例如 `gui`; 多源裝配時它就是「由誰接管」的依據, 同組源內必須唯一。]
	#Throw[{{nameof(ArgumentException)}}][路徑是空串時。]
	#Throw[{{nameof(IOException)}}][檔案存在但讀不了時; 那屬於部署錯誤, 不靜默降級。]
	#Throw[{{nameof(JsonException)}}][檔案存在但內容不是合法 JSON 時。]
	""")]
	public partial JsonFileCfgAccessor(str Path, str Name);

	[Doc(@$"立刻讀一次, 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。要事件就走 {nameof(Reload)}。")]
	public partial IDictionary<str, obj?> Load();

	[Doc(@$"立刻讀一次(異步), 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。")]
	public partial Task<IDictionary<str, obj?>> Load(CT Ct);

	[Doc(@$"整份寫回檔案(原子), 給 {nameof(FnSave)} 用。")]
	private partial nil SaveFile(IDictionary<str, obj?> Dict);

	[Doc(@$"整份寫回檔案(原子、異步), 給 {nameof(FnSaveAsy)} 用。")]
	private partial Task<nil> SaveFile(IDictionary<str, obj?> Dict, CT Ct);
}






