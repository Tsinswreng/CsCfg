namespace Tsinswreng.CsCfg.Json;

using System.Text.Json;
using Tsinswreng.CsCore;

[Doc($$"""
以一份 JSON 檔案為來源的可讀可寫配置源。

*檔案系統只出現在這一層*: 上面兩層不認識檔案。本類只做三件事: 記住路徑, 把讀取換成
「每次開一次檔、讀完關掉」, 把保存換成「寫暫存檔、成功後搬移覆蓋」。解析與事件全由基底類負責。

*寫入是原子的*: 正式檔在暫存檔搬移過來之前一直是完整的。中途失敗只會留下一個被刪掉的暫存檔。

*建構時先讀一次*: 免得呼叫方忘了呼叫讀取就去取值, 也讓第一次保存不會蓋掉檔案裡原有的鍵。

#Eg[
	```cs
	var Rw = new JsonFileCfgAccessor("/data/app/cfg.rw.jsonc", "gui");
	Rw.SetNoSave(Key, "en");    // Key: 任意 IKeyNode<T>
	await Rw.Save(Ct);
	```
]
""")]
public partial class JsonFileCfgAccessor : JsonCfgAccessor{

	[Doc(@$"
	配置檔的絕對路徑。建構時正規化, 理由同 {nameof(JsonFileCfgReader.Path)}。
	*為何是 init-only*: 路徑是這個源的身份, 換檔案應該換一個源。
	")]
	public str Path{get;init;} = "";

	[Doc($$"""
	接上一份配置檔, 立刻讀一次, 之後的讀寫都落在這一份檔案上。

	#Prm[{{nameof(Path)}}][配置檔路徑; 可以相對, 會正規化成絕對路徑。]
	#Prm[{{nameof(Name)}}][源的技術標識, 例如 `gui`; 多源裝配時它就是「由誰接管」的依據, 同組源內必須唯一。]
	#Throw[{{nameof(ArgumentException)}}][路徑是空串時。]
	#Throw[{{nameof(IOException)}}][檔案存在但讀不了時; 那屬於部署錯誤, 不靜默降級。]
	#Throw[{{nameof(JsonException)}}][檔案存在但內容不是合法 JSON 時。]
	""")]
	public partial JsonFileCfgAccessor(str Path, str Name);
}
