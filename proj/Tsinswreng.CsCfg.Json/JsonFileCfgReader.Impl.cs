//Impl: 聲明見 JsonFileCfgReader.cs。
namespace Tsinswreng.CsCfg.Json;

using Tsinswreng.CsCore;

public partial class JsonFileCfgReader{

	public partial JsonFileCfgReader(str Path, str Name) : base(null, Name){
		if(str.IsNullOrWhiteSpace(Path)){
			throw new ArgumentException("Config file path is empty.", nameof(Path));
		}
		// 正規化: 分隔符與相對段先收掉, 之後日誌裡的路徑只有一種寫法
		this.Path = System.IO.Path.GetFullPath(Path);
		// 建構時讀一次, 但走的是基底類的 Load, 它會呼叫本類覆寫的 OpenInputStream
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
}
