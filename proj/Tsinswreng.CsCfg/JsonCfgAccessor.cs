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
	public Func<JsonFileCfgAccessor, CT, Task<bool>>? FnReLoadAsy{get;set;}
	public Func<JsonFileCfgAccessor, CT, Task<bool>>? FnSaveAsy{get;set;}

	IDictionary<str, obj?> MkDict()=> new Dictionary<str, obj?>();
	public JsonFileCfgAccessor FromJson(str JsonStr){
		CfgDict = ToolJson.JsonStrToDict(JsonStr)??MkDict();
		return this;
	}
	// avoid static, keep fluent style
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
	public bool Reload(){
		var b = Reload(default).Result;
		return b;
	}

	[Impl]
	public async Task<bool> Reload(CT Ct) {
		BeforeReLoad?.Invoke(this, null!);
		if(FnReLoadAsy != null){
			return await FnReLoadAsy(this, Ct);
		}
		await _ReloadAsy(Ct);
		AfterReLoad?.Invoke(this, null!);
		return true;
	}

	public async Task<bool> _ReloadAsy(CT Ct) {
		await FromFileAsy(FilePath, Ct);
		return true;
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
	public bool RmPathNoSave(IList<str> Path){
		ToolDict.SetValueByPath(CfgDict, Path, NIL);
		return true;
	}

	[Impl]
	public bool Save(){
		var b = Save(default).Result;
		return b;
	}


	[Impl]
	public async Task<bool> Save(CT Ct) {
		BeforeSave?.Invoke(this, null!);
		if(FnSaveAsy!=null){
			return await FnSaveAsy(this, Ct);
		}
		var b = await _Save(Ct);
		AfterSave?.Invoke(this, null!);
		return b;
	}

	public async Task<bool> _Save(CT Ct) {
		var Json = ToolJson.DictToJson(CfgDict);
		await File.WriteAllTextAsync(FilePath, Json, Ct);
		return true;
	}
}
