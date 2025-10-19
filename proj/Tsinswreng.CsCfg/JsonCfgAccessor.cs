#define Impl
namespace Tsinswreng.CsCfg;
using System.Collections;
using Tsinswreng.CsCore;
using Tsinswreng.CsTools;


public partial class JsonFileCfgAccessor
	:ICfgAccessor
	,ICfgEvents
	,I_CfgDict
{
// protected static JsonCfgAccessor? _Inst = null;
// public static JsonCfgAccessor Inst => _Inst??= new JsonCfgAccessor();

	public str FilePath{get;set;} = "";

	[Impl]
	public IDictionary<str, object?> CfgDict{get;set;}
#if Impl
	= new Dictionary<str, object?>();
#endif
	public Func<JsonFileCfgAccessor, CT, Task<nil>>? FnReLoadAsy{get;set;}
	public Func<JsonFileCfgAccessor, CT, Task<nil>>? FnSaveAsy{get;set;}

	IDictionary<str, obj?> MkDict()=> new Dictionary<str, obj?>();
	public JsonFileCfgAccessor FromJson(str JsonStr){
		CfgDict = ToolJson.JsonStrToDict(JsonStr)??MkDict();
		return this;
	}
	//勿用static 以適單例
	public async Task<JsonFileCfgAccessor> FromFileAsy(str FilePath, CT Ct){
		var z = this;
		z.FilePath = FilePath;
		var JsonStr = await File.ReadAllTextAsync(FilePath, Ct);
		z.CfgDict = ToolJson.JsonStrToDict(JsonStr)??MkDict();
		return z;
	}



	[Impl]
	public nil ReLoad(){
		ReLoadAsy(default).Wait();
		return NIL;
	}

	[Impl]
	public async Task<nil> ReLoadAsy(CT Ct) {
		BeforeReLoad?.Invoke(this, null!);
		if(FnReLoadAsy != null){
			return await FnReLoadAsy(this, Ct);
		}
		await _ReLoadAsy(Ct);
		AfterReLoad?.Invoke(this, null!);
		return NIL;
	}

	public async Task<nil> _ReLoadAsy(CT Ct) {
		await FromFileAsy(FilePath, Ct);
		return NIL;
	}

	[Impl(typeof(ICfgAccessor))]
	public bool TryGetBoxedByPath(
		IList<str> Path
		,out ICfgValue Got
	){
		if( ToolDict.TryGetValueByPath(CfgDict, Path, out var VObj) ){
			Got = new CfgValue{Data=VObj};
			return true;
		}
		Got = default!;
		return false;
	}


	[Impl]
	public ICfgValue? GetBoxedByPath(IList<str> Path){
		if(this.TryGetBoxedByPath(Path, out var Got)){
			return Got;
		}
		return null;
	}

	[Impl]
	public nil SetBoxedByPath(IList<str> Path, ICfgValue Value){
		ToolDict.PutValueByPath(CfgDict, Path, Value.Data);
		return NIL;
	}

	[Impl]
	public nil RmPath(IList<str> Path){
		ToolDict.PutValueByPath(CfgDict, Path, NIL);
		return NIL;
	}

	[Impl]
	public nil Save(){
		SaveAsy(default).Wait();
		return NIL;
	}


	[Impl]
	public async Task<nil> SaveAsy(CT Ct) {
		BeforeSave?.Invoke(this, null!);
		if(FnSaveAsy!=null){
			return await FnSaveAsy(this, Ct);
		}
		await _SaveAsy(Ct);
		AfterSave?.Invoke(this, null!);
		return NIL;
	}

	public async Task<nil> _SaveAsy(CT Ct) {
		var Json = ToolJson.DictToJson(CfgDict);
		await File.WriteAllTextAsync(FilePath, Json, Ct);
		return NIL;
	}
}

