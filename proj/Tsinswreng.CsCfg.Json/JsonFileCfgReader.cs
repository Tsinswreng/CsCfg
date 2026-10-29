namespace Tsinswreng.CsCfg.Json;

using System.Text.Json;
using Tsinswreng.CsCore;

[Doc($$"""
以一份 JSON 檔案為來源的唯讀配置源。

*為什麼不是繼承 {{nameof(JsonCfgReader)}}*: 那個類的 `Load()` 讀的是它持有的流;
本類沒有流, 若繼承它就會得到一個「呼叫了卻什麼都沒發生」的 `Load()`,
那是比缺功能更難查的錯。所以本類直接建在 {{nameof(DictCfgReader)}} 上, 自己實現檔案讀取;
JSON 的解析仍走 {{nameof(JsonCfgCodec)}}, 格式邏輯沒有第二份。

*誰負責開關檔*: 本類。每次讀都重開一次、用完就關(策略見 {{nameof(JsonCfgFileIo)}}),
所以這份檔案平時是「誰都能打開」的狀態, 不會被本進程長期佔住。

*檔案不存在時*: 是一個空源, 不是錯誤。建構與 {{nameof(Load)}} 都會得到空字典,
之後取值全走鍵上宣告的默認值。只有「存在但解析不了」與「沒有讀權限」才拋。

*建構即讀一次*: 這是刻意的, 免得每個呼叫方都忘了先 {{nameof(Load)}} 就去取值,
然後拿到一整組默認值卻沒有任何提示。因此本類只給「建構即讀」這一個入口;
日後真出現「要先建好、稍後才讀」的場景, 再補一個顯式時機的建構子, 不要靠事後改 {{nameof(Path)}}——
那個屬性是 init-only, 換檔案應該換一個源。

#Eg[
	```cs
	// Name 是這個源的技術標識; 多源裝配時, 設定頁就是靠它顯示「這一項由誰提供」
	var Ro = new JsonFileCfgReader("/etc/app/cfg.jsonc", "file");
	var Val = Ro.Get(Key);      // Key: 任意 IKeyNode<T>
	Ro.Reload();                // 檔案被人改過之後, 會觸發 BeforeReload／AfterReload
	```
]
""")]
public partial class JsonFileCfgReader : DictCfgReader{

	[Doc(@$"
	配置檔的絕對路徑。建構時正規化(走一次 `GetFullPath`), 所以 `sub/a.jsonc` 與 `sub\a.jsonc`
	在這裡是同一個字符串, 日誌與路徑比對不會出現兩種寫法。

	*為何是 init-only*: 路徑是這個源的身份, 換檔案應該換一個源。允許事後改的話,
	{nameof(FnReload)} 讀到的與外部快取的路徑就會不同步。
	")]
	public str Path{get;init;} = "";

	[Doc(@$"解析選項。默認允許註釋與尾逗號, 因為配置檔常由人手寫。")]
	public JsonDocumentOptions DocOpts{get;set;} = JsonCfgOpts.DfltDocOpts;

	[Doc($$"""
	接上一份配置檔並立刻讀一次。

	#Prm[{{nameof(Path)}}][配置檔路徑; 可以相對, 會正規化成絕對路徑。]
	#Prm[{{nameof(Name)}}][源的技術標識, 例如 `file`; 多源裝配時它就是「由誰接管」的依據, 同組源內必須唯一。]
	#Throw[{{nameof(ArgumentException)}}][路徑是空串時。]
	#Throw[{{nameof(IOException)}}][檔案存在但讀不了時; 那屬於部署錯誤, 不靜默降級。]
	#Throw[{{nameof(JsonException)}}][檔案存在但內容不是合法 JSON 時。]
	""")]
	public partial JsonFileCfgReader(str Path, str Name);

	[Doc(@$"立刻讀一次, 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。要事件就走 {nameof(Reload)}。")]
	public partial IDictionary<str, obj?> Load();

	[Doc(@$"立刻讀一次(異步), 不觸發事件; 失敗拋。回傳解析結果, 同時寫進 {nameof(Dict)}。")]
	public partial Task<IDictionary<str, obj?>> Load(CT Ct);
}
