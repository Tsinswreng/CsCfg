//Impl: 聲明見 JsonCfgAccessor.cs。
namespace Tsinswreng.CsCfg.Json;

public partial class JsonCfgAccessor{

	public partial JsonCfgAccessor(Stream? InputStream, Stream? OutputStream, str Name){
		this.InputStream = InputStream;
		this.OutputStream = OutputStream;
		this.Name = Name;
		// 讀寫都接到流上; 之後 Reload / Save 就是解析與重寫整份
		this.FnReload = Load;
		this.FnReloadAsy = LoadAsy;
		this.FnSave = SaveToStream;
		this.FnSaveAsy = SaveToStreamAsy;
		if(InputStream is not null){
			Load();
		}
	}

	public partial IDictionary<str, obj?> Load(){
		// 沒有流就沒有可讀的東西: 重載是 no-op, 不拋也不清空(理由同 JsonCfgReader)
		if(InputStream is null){
			return Dict;
		}
		var S = InputStream;
		if(S.CanSeek){
			S.Position = 0;
		}
		var Parsed = JsonCfgCodec.ParseObject(S, DocOpts, MkDict);
		Dict = Parsed;
		return Parsed;
	}

	public partial async Task<IDictionary<str, obj?>> LoadAsy(CT Ct){
		if(InputStream is null){
			return Dict;
		}
		var S = InputStream;
		if(S.CanSeek){
			S.Position = 0;
		}
		var Parsed = await JsonCfgCodec.ParseObjectAsy(S, DocOpts, MkDict, Ct);
		Dict = Parsed;
		return Parsed;
	}

	private partial nil SaveToStream(IDictionary<str, obj?> Dict){
		var S = OutputStream ?? throw new InvalidOperationException(
			$"{nameof(OutputStream)} is not set. Assign a JSON stream before saving."
		);
		// 整份重寫, 先把舊內容截掉, 免得新內容比舊的短時尾巴殘留
		if(S.CanSeek){
			S.SetLength(0);
			S.Position = 0;
		}
		JsonCfgCodec.Write(Dict, S, WriterOpts);
		return NIL;
	}

	private partial async Task<nil> SaveToStreamAsy(IDictionary<str, obj?> Dict){
		var S = OutputStream ?? throw new InvalidOperationException(
			$"{nameof(OutputStream)} is not set. Assign a JSON stream before saving."
		);
		if(S.CanSeek){
			S.SetLength(0);
			S.Position = 0;
		}
		return await JsonCfgCodec.WriteAsy(Dict, S, WriterOpts, default);
	}
}
