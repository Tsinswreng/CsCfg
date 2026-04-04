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
	[Doc(@$"Json File Path")]
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
		if(str.IsNullOrEmpty(JsonStr)){
			z.CfgDict = MkDict();
		}else{
			z.CfgDict = ToolJson.JsonStrToDict(JsonStr)??MkDict();
		}
		return z;
	}


	public JsonFileCfgAccessor FromFile(str FilePath){
		var z = this;
		z.FilePath = FilePath;
		var JsonStr = File.ReadAllText(FilePath);
		if(str.IsNullOrEmpty(JsonStr)){
			z.CfgDict = MkDict();
		}else{
			z.CfgDict = ToolJson.JsonStrToDict(JsonStr)??MkDict();
		}
		return z;
	}


	[Impl]
	public nil Reload(){
		Reload(default).Wait();
		return NIL;
	}

	[Impl]
	public async Task<nil> Reload(CT Ct) {
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
	public bool TryGet(
		IList<str> Path, out obj? Got
	){
		Got = default;
		if( ToolDict.TryGetValueByPath(CfgDict, Path, out var VObj) ){
			Got = VObj;
			return true;
		}
		return false;
	}
	
	[Impl(typeof(ICfgAccessor))]
	public bool TrySetNoSave(IList<str> Path, obj? V){
		return ToolDict.SetValueByPath(CfgDict, Path, V);
	}
	

	[Impl]
	public nil RmPathNoSave(IList<str> Path){
		ToolDict.SetValueByPath(CfgDict, Path, NIL);
		return NIL;
	}

	[Impl]
	public nil Save(){
		Save(default).Wait();
		return NIL;
	}


	[Impl]
	public async Task<nil> Save(CT Ct) {
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
	
	#region Obslt

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
	public nil SetBoxedByPathNonSave(IList<str> Path, ICfgValue Value){
		ToolDict.SetValueByPath(CfgDict, Path, Value.Data);
		return NIL;
	}
	#endregion Obslt
}

