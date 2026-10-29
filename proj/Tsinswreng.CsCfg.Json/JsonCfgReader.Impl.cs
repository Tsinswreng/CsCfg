//Impl: 聲明見 JsonCfgReader.cs。
namespace Tsinswreng.CsCfg.Json;

public partial class JsonCfgReader{

	public partial JsonCfgReader(Stream? InputStream, str Name){
		this.InputStream = InputStream;
		this.Name = Name;
		// 把重讀接到自己身上: 之後 Reload() 就是重新解析一次, 並且自動帶上 Before/After 事件
		this.FnReload = Load;
		this.FnReloadAsy = Load;
		if(InputStream is not null){
			Load();
		}
	}

	public partial IDictionary<str, obj?> Load(){
		// 每次讀取都向鉤子要一條流: 檔案源的鉤子會重新開檔, 本類的默認鉤子回同一條
		var S = OpenInputStream();
		if(S is null){
			// 沒有流就沒有可讀的東西: 重載是 no-op, 不拋也不清空
			return Dict;
		}
		try{
			// 回到開頭再解析; 不可 Seek 的流只能讀一次
			if(S.CanSeek){
				S.Position = 0;
			}
			var Parsed = JsonCfgCodec.ParseObject(S, DocOpts, MkDict);
			Dict = Parsed;
			return Parsed;
		}
		finally{
			CloseInputStream(S);
		}
	}

	public partial async Task<IDictionary<str, obj?>> Load(CT Ct){
		var S = OpenInputStream();
		if(S is null){
			return Dict;
		}
		try{
			if(S.CanSeek){
				S.Position = 0;
			}
			var Parsed = await JsonCfgCodec.ParseObject(S, DocOpts, MkDict, Ct);
			Dict = Parsed;
			return Parsed;
		}
		finally{
			CloseInputStream(S);
		}
	}
}
