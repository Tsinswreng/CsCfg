//Impl: 聲明見 JsonFileCfgAccessor.cs。
namespace Tsinswreng.CsCfg.Json;

using Tsinswreng.CsCore;

public partial class JsonFileCfgAccessor{

	public partial JsonFileCfgAccessor(str Path, str Name){
		if(str.IsNullOrWhiteSpace(Path)){
			throw new ArgumentException("Config file path is empty.", nameof(Path));
		}
		this.Path = System.IO.Path.GetFullPath(Path);
		this.Name = Name;

		// 重讀／保存全部接到檔案上; 基底類只提供抽象與寫入路由, 不知道「檔案」這回事
		this.FnReload = Load;
		this.FnReloadAsy = Load;
		this.FnSave = SaveFile;
		this.FnSaveAsy = SaveFile;

		// 建構時讀一次, 但不走 Reload(理由見宣告處); 同時把既有內容讀進來, 免第一次 Save 蓋掉它
		Load();
	}

	public partial IDictionary<str, obj?> Load(){
		var Parsed = JsonCfgFileIo.Read(Path, DocOpts, MkDict);
		Dict = Parsed;
		return Parsed;
	}

	public partial async Task<IDictionary<str, obj?>> Load(CT Ct){
		var Parsed = await JsonCfgFileIo.Read(Path, DocOpts, MkDict, Ct);
		Dict = Parsed;
		return Parsed;
	}

	private partial nil SaveFile(IDictionary<str, obj?> Dict){
		return JsonCfgFileIo.Write(Path, Dict, WriterOpts);
	}

	private partial Task<nil> SaveFile(IDictionary<str, obj?> Dict, CT Ct){
		return JsonCfgFileIo.Write(Path, Dict, WriterOpts, Ct);
	}
}
