namespace Tsinswreng.CsCfg.Json;

using Tsinswreng.CsCore;

[Doc($$"""
檔案系統的唯一落點, 只給 {{nameof(JsonFileCfgReader)}}／{{nameof(JsonFileCfgAccessor)}} 用。

專案的分層要求是「檔案系統留在最後」: 上面兩層(值模型、JSON 編解碼)編譯時不碰 `System.IO.File`,
開檔關檔、暫存檔、搬移只出現在檔案源所處的這一層。這個類就是那一層的實現細節, 所以不對外公開。

*策略*: 每次操作開一條新流、用完關閉; 寫入先落 `路徑.tmp`, 成功後搬移覆蓋正式檔。
不長期佔住檔案, 是為了讓別的程式(編輯器、備份)隨時打得開, 也避免進程崩潰時留下半份配置。
""")]
internal static partial class JsonCfgFileIo{

	[Doc($$"""
	打開一份配置檔供讀取。
	檔案或所在目錄不存在時回一條空流, 讓上層得到空配置而不是例外: 首次啟動必然是這兩種情形。

	*共享打開*: 用 `FileShare.ReadWrite`, 別的程式正在看這份檔案時也讀得到。

	*容忍 BOM*: 有些編輯器存 UTF-8 時會加 BOM, 而 JSON 解析器會把它當成非法字節。
	這份配置本來就是給人手改的, 所以由這一層把 BOM 吃掉。

	#Prm[{{nameof(FilePath)}}][配置檔路徑。]
	#Throw[{{nameof(IOException)}}][檔案存在但沒有讀權限時。]
	""")]
	internal static partial Stream OpenRead(str FilePath);

	[Doc($$"""
	為整份重寫打開暫存檔。所在目錄不存在時會建出來。

	#Prm[{{nameof(FilePath)}}][正式檔路徑; 暫存檔是它加 `.tmp`。]
	#Rtn[寫入用的流, 用完由調用方處置。]
	""")]
	internal static partial Stream OpenTmp(str FilePath);

	[Doc($$"""
	暫存檔的收尾。
	#Prm[{{nameof(FilePath)}}][正式檔路徑。]
	#Prm[{{nameof(Ok)}}][寫入是否成功。成功則搬移覆蓋正式檔, 失敗則刪掉暫存檔。]
	""")]
	internal static partial nil FinishTmp(str FilePath, bool Ok);

	[Doc(@$"暫存檔路徑: 原路徑加 `.tmp`, 並確保所在目錄存在。")]
	private static partial str MkTmpPath(str FilePath);

	[Doc(@$"把流的位置移到 BOM 之後; 沒有 BOM 就回到開頭。")]
	private static partial nil SkipBom(Stream Utf8);

	[Doc(@$"盡力刪掉暫存檔; 清理失敗不拋。")]
	private static partial void TryDel(str FilePath);
}
