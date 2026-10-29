namespace Tsinswreng.CsCfg.Json;

using System.Text.Json;
using Tsinswreng.CsCore;

[Doc($$"""
配置檔讀寫的唯一入口(程序集內部), 供 {{nameof(JsonFileCfgReader)}}／{{nameof(JsonFileCfgAccessor)}} 用。

*為何不對外公開*: 它是那兩個源的實現細節, 不是庫的契約。對外公開等於把「開關檔策略」
變成別人可以依賴的形狀, 日後想換成別的 IO 方案就得兼容這份公開面。
真要單獨讀寫一份配置檔的場景, 用一次性的源(`new JsonFileCfgReader(Path, Name)`)即可。

*為何集中在一個類*: 「開、用完、關」與「先寫暫存檔再搬移」這兩條策略只能有一份實現,
否則兩個源的寫入語義會慢慢漂開。這也是日後要換 IO 實現(例如注入 `IFileSystem`)時唯一要改的地方。

*策略*:
- 不長期佔住檔案: 每次要用才開、用完關。配置檔要能被人和別的程式隨時打開(編輯、備份、`cat`),
  拿著帶寫權限的 `FileStream` 會讓嚴格的讀取方開不起來, 進程崩潰時檔案狀態也不明。
- 寫入經過暫存檔: 先寫 `路徑.tmp`, 再覆蓋式搬移到目標。中途失敗最多留一個孤兒暫存檔,
  不會出現半份配置——半份配置比舊配置更難救。搬移留在同一個目錄是為了不跨檔案系統。
- 空檔與不存在的檔都是空配置, 回空字典、不拋: 首次啟動必然是這兩種情形之一。
  只有「存在但解析不了」與「沒有讀寫權限」才拋, 因為那兩類是部署錯誤, 靜默降級會更難查。
""")]
internal static partial class JsonCfgFileIo{

	[Doc($$"""
	讀一份配置檔。檔案或所在目錄不存在時回空字典。

	*共享打開*: 用 `FileShare.ReadWrite` 開檔, 所以別的程式正在看這份檔案時也讀得到。

	*容忍 BOM*: 有些編輯器存 UTF-8 時會加 BOM, 而 JSON 解析器會把它當成非法字節。
	這份配置本來就是給人手改的, 所以由這層把 BOM 吃掉, 不把編輯器的習慣變成使用者的錯誤。
	只吃文件開頭那一個, 中間出現的照樣算格式錯誤。

	#Prm[{{nameof(FilePath)}}][配置檔的絕對或相對路徑。]
	#Prm[{{nameof(DocOpts)}}][解析選項。]
	#Prm[{{nameof(MkDict)}}][造字典的工廠, 必須與讀者的 {{nameof(DictCfgReader.MkDict)}} 一致。]
	#Throw[{{nameof(InvalidDataException)}}][根不是對象時。]
	#Throw[{{nameof(JsonException)}}][內容不是合法 JSON 時。]
	""")]
	internal static partial IDictionary<str, obj?> Read(
		str FilePath
		,JsonDocumentOptions DocOpts
		,Func<IDictionary<str, obj?>> MkDict
	);

	[Doc(@$"同 {nameof(Read)}, 但異步讀檔。")]
	internal static partial Task<IDictionary<str, obj?>> Read(
		str FilePath
		,JsonDocumentOptions DocOpts
		,Func<IDictionary<str, obj?>> MkDict
		,CT Ct
	);

	[Doc($$"""
	整份重寫一份配置檔, 原子完成。

	#Prm[{{nameof(FilePath)}}][配置檔路徑; 所在目錄不存在時會建出來。]
	#Prm[{{nameof(Dict)}}][要寫出的整棵配置樹; 鍵序就是寫出來的鍵序。]
	#Prm[{{nameof(WriterOpts)}}][寫出選項。]
	#Throw[{{nameof(IOException)}}][寫不進去時; 暫存檔會被清掉。]
	""")]
	internal static partial nil Write(
		str FilePath
		,IDictionary<str, obj?> Dict
		,JsonWriterOptions WriterOpts
	);

	[Doc(@$"同 {nameof(Write)}, 但異步寫檔。")]
	internal static partial Task<nil> Write(
		str FilePath
		,IDictionary<str, obj?> Dict
		,JsonWriterOptions WriterOpts
		,CT Ct
	);

	[Doc(@$"
	暫存檔路徑: 原路徑加 `.tmp`, 並確保所在目錄存在。
	放同一個目錄是為了讓之後的搬移留在同一個檔案系統內。
	")]
	private static partial str MkTmpPath(str FilePath);

	[Doc(@$"把暫存檔搬成正式檔。")]
	private static partial nil Replace(str TmpPath, str FilePath);

	[Doc(@$"盡力刪掉暫存檔; 清理失敗不拋, 否則會蓋掉原本那個有意義的例外。")]
	private static partial void TryDel(str FilePath);

	[Doc(@$"
	把流的位置移到 BOM 之後; 沒有 BOM 就回到開頭。
	只處理可 Seek 的流: 檔案流必然可 Seek, 不可 Seek 的流本來就只讀得到一次。
	")]
	private static partial nil SkipBom(Stream S);
}
