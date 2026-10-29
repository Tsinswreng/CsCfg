//Impl: 聲明見 JsonCfgReader.cs。
namespace Tsinswreng.CsCfg.Json;

public partial class JsonCfgReader{

	public partial JsonCfgReader(Stream? InputStream, str Name){
		this.InputStream = InputStream;
		this.Name = Name;
		// 把重讀接到流上: 之後 Reload() 就是重新解析一次, 並且自動帶上 Before/After 事件
		this.FnReload = Load;
		this.FnReloadAsy = Load;
		if(InputStream is not null){
			Load();
		}
	}

	public partial IDictionary<str, obj?> Load(){
		// 沒有流就沒有可讀的東西: 重載是 no-op, 不拋也不清空。
		// 這條讓「開發配置檔在本機不存在」這種正常情形不必在每個宿主各寫一次判斷。
		if(InputStream is null){
			return Dict;
		}
		var S = InputStream;
		// 回到開頭再解析; 不可 Seek 的流只能讀一次
		if(S.CanSeek){
			S.Position = 0;
		}
		// 回傳值是給 FnReload 用的; Dict 由 DictCfgReader.Reload 再賦一次, 這裡先設好方便直接呼叫
		var Parsed = JsonCfgCodec.ParseObject(S, DocOpts, MkDict);
		Dict = Parsed;
		return Parsed;
	}

	public partial async Task<IDictionary<str, obj?>> Load(CT Ct){
		if(InputStream is null){
			return Dict;
		}
		var S = InputStream;
		if(S.CanSeek){
			S.Position = 0;
		}
		var Parsed = await JsonCfgCodec.ParseObject(S, DocOpts, MkDict, Ct);
		Dict = Parsed;
		return Parsed;
	}
}
