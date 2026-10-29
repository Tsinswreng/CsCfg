namespace Tsinswreng.CsCfg.Json;

using System.Text.Json;
using Tsinswreng.CsCore;

[Doc($$"""
以一份 JSON 檔案為來源的唯讀配置源。

*檔案系統只出現在這一層*: 上面兩層不認識檔案。本類只做兩件事: 記住路徑, 以及把
{{nameof(JsonCfgReader.OpenInputStream)}} 與 {{nameof(JsonCfgReader.CloseInputStream)}} 換成
「每次讀取開一次檔、讀完關掉」。解析、鍵序、事件全由基底類負責。

*檔案不存在時*: 是一個空源, 不是錯誤。建構與 {{nameof(Load)}} 都會得到空字典,
之後取值全走鍵上宣告的默認值。只有「存在但解析不了」與「沒有讀權限」才拋。

#Eg[
	```cs
	var Ro = new JsonFileCfgReader("/etc/app/cfg.jsonc", "file");
	var Val = Ro.Get(Key);   // Key: 任意 IKeyNode<T>
	```
]
""")]
public partial class JsonFileCfgReader : JsonCfgReader{

	[Doc(@$"
	配置檔的絕對路徑。建構時正規化(走一次 `GetFullPath`), 所以 `sub/a.jsonc` 與 `sub\a.jsonc`
	在這裡是同一個字符串。
	*為何是 init-only*: 路徑是這個源的身份, 換檔案應該換一個源。
	")]
	public str Path{get;init;} = "";

	[Doc($$"""
	接上一份配置檔並立刻讀一次。

	#Prm[{{nameof(Path)}}][配置檔路徑; 可以相對, 會正規化成絕對路徑。]
	#Prm[{{nameof(Name)}}][源的技術標識, 例如 `file`; 多源裝配時它就是「由誰接管」的依據, 同組源內必須唯一。]
	#Throw[{{nameof(ArgumentException)}}][路徑是空串時。]
	#Throw[{{nameof(IOException)}}][檔案存在但讀不了時; 那屬於部署錯誤, 不靜默降級。]
	#Throw[{{nameof(JsonException)}}][檔案存在但內容不是合法 JSON 時。]
	""")]
	public partial JsonFileCfgReader(str Path, str Name);
}
