//Impl: 聲明見 JsonFileCfgAccessor.cs。
namespace Tsinswreng.CsCfg.Json;

using Tsinswreng.CsCore;

public partial class JsonFileCfgAccessor{

	public partial JsonFileCfgAccessor(str Path, str Name) : base(null, null, Name){
		if(str.IsNullOrWhiteSpace(Path)){
			throw new ArgumentException("Config file path is empty.", nameof(Path));
		}
		this.Path = System.IO.Path.GetFullPath(Path);
		// 建構時讀一次, 走的是基底類的 Load, 它會呼叫本類覆寫的 OpenInputStream
		Load();
	}

	[Doc(@$"每次都開一條新的讀取流; 檔案不存在時拿到空流, 上層因此得到空配置。")]
	protected override Stream? OpenInputStream(){
		return JsonCfgFileIo.OpenRead(Path);
	}

	[Doc(@$"讀完就關: 本類不長期持有檔案。")]
	protected override nil CloseInputStream(Stream Utf8){
		Utf8.Dispose();
		return NIL;
	}

	[Doc(@$"每次保存都開一條新的寫入流, 寫的是暫存檔; 正式檔在它上面不動。")]
	protected override Stream? OpenOutputStream(){
		return JsonCfgFileIo.OpenTmp(Path);
	}

	[Doc(@$"關掉暫存檔, 並依 {nameof(Ok)} 決定搬移覆蓋正式檔或刪掉暫存檔。")]
	protected override nil CloseOutputStream(Stream Utf8, bool Ok){
		Utf8.Dispose();
		JsonCfgFileIo.FinishTmp(Path, Ok);
		return NIL;
	}
}
