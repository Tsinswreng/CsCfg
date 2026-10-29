//Impl: 聲明見 JsonFileCfgReader.cs。
namespace Tsinswreng.CsCfg.Json;

using Tsinswreng.CsCore;

public partial class JsonFileCfgReader{

	public partial JsonFileCfgReader(str Path, str Name){
		if(str.IsNullOrWhiteSpace(Path)){
			throw new ArgumentException("Config file path is empty.", nameof(Path));
		}
		// 正規化: 分隔符與相對段先收掉, 之後日誌裡的路徑只有一種寫法
		this.Path = System.IO.Path.GetFullPath(Path);
		this.Name = Name;
		// 重讀接到檔案上: 之後的 Reload 就是重開檔再解析, 事件由基底類拋
		this.FnReload = Load;
		this.FnReloadAsy = Load;
		// 建構時讀一次, 但不走 Reload: 建構期間宿主還沒機會訂閱, 拋事件沒有意義
		Load();
	}

	public partial IDictionary<str, obj?> Load(){
		// 檔案不存在就是空字典(由 IO 層決定), 這裡不再判斷
		var Parsed = JsonCfgFileIo.Read(Path, DocOpts, MkDict);
		Dict = Parsed;
		return Parsed;
	}

	public partial async Task<IDictionary<str, obj?>> Load(CT Ct){
		var Parsed = await JsonCfgFileIo.Read(Path, DocOpts, MkDict, Ct);
		Dict = Parsed;
		return Parsed;
	}
}
