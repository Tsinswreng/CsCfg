//Impl: 聲明見 JsonCfgAccessor.cs。
namespace Tsinswreng.CsCfg.Json;

public partial class JsonCfgAccessor{

	public partial JsonCfgAccessor(Stream? InputStream, Stream? OutputStream, str Name){
		this.InputStream = InputStream;
		this.OutputStream = OutputStream;
		this.Name = Name;
		// 讀寫都接到自己身上; 之後 Reload / Save 就是解析與重寫整份
		this.FnReload = Load;
		this.FnReloadAsy = Load;
		this.FnSave = SaveToStream;
		this.FnSaveAsy = SaveToStream;
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

	private partial nil SaveToStream(IDictionary<str, obj?> Dict){
		var S = OpenOutputStream() ?? throw new InvalidOperationException(
			$"{nameof(OutputStream)} is not set. Assign a JSON stream before saving."
		);
		var Ok = false;
		try{
			// 整份重寫, 先把舊內容截掉, 免得新內容比舊的短時尾巴殘留
			if(S.CanSeek){
				S.SetLength(0);
				S.Position = 0;
			}
			JsonCfgCodec.Write(Dict, S, WriterOpts);
			Ok = true;
		}
		finally{
			CloseOutputStream(S, Ok);
		}
		return NIL;
	}

	private partial async Task<nil> SaveToStream(IDictionary<str, obj?> Dict, CT Ct){
		var S = OpenOutputStream() ?? throw new InvalidOperationException(
			$"{nameof(OutputStream)} is not set. Assign a JSON stream before saving."
		);
		var Ok = false;
		try{
			if(S.CanSeek){
				S.SetLength(0);
				S.Position = 0;
			}
			await JsonCfgCodec.Write(Dict, S, WriterOpts, Ct);
			Ok = true;
		}
		finally{
			CloseOutputStream(S, Ok);
		}
		return NIL;
	}
}
