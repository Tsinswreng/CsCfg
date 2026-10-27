using Tsinswreng.CsJson;

namespace Tsinswreng.CsCfg.Json;

public class JsonCfgReader : DictCfgReader {
	public IJsonSerializerCore JsonSerializer{get;set;} = CsJson.JsonSerializer.Inst;
	public Stream JsonStreamUtf8{get;set;}
	public JsonCfgReader(
		Stream JsonStreamReaderUtf8
	){
		this.JsonStreamUtf8 = JsonStreamReaderUtf8;
		Init();
	}
	
	void Init(){
		var s = JsonSerializer;
		var GotDict = s.Deserialize<IDictionary<str, obj?>>(JsonStreamUtf8, s.Options);
		this.Dict = GotDict??this.Dict;
	}
}

public class JsonCfgAccessor: JsonCfgReader, ICfgAccessor{
	public Stream JsonStreamWriterUtf8{get;set;}
	public JsonCfgAccessor(
		Stream JsonStreamReaderUtf8,
		Stream JsonStreamWriterUtf8
	):base(JsonStreamReaderUtf8){
		this.JsonStreamWriterUtf8 = JsonStreamWriterUtf8;
	}

	public bool TrySetNoSave(IReadOnlyList<string> Path, object? V) {
		throw new NotImplementedException();
	}

	public bool RmPathNoSave(IReadOnlyList<string> Path) {
		throw new NotImplementedException();
	}

	public bool Save() {
		throw new NotImplementedException();
	}

	public Task<bool> Save(CT Ct) {
		throw new NotImplementedException();
	}
}
