using Tsinswreng.CsCore.Tools;

namespace Tsinswreng.CsCfg;
public class JsonCfgAccessor: ICfgAccessor{
// protected static JsonCfgAccessor? _Inst = null;
// public static JsonCfgAccessor Inst => _Inst??= new JsonCfgAccessor();

	public IDictionary<str, object?> CfgDict{get;set;} = new Dictionary<str, object?>();
	public JsonCfgAccessor FromJson(str JsonStr){
		CfgDict = ToolJson.JsonStrToDict(JsonStr);
		return this;
	}

	public virtual ICfgValue? GetByPath(IList<str> Path){
		var V = ToolDict.GetValueByPath(CfgDict, Path);
		return new CfgValue{Data=V};
	}
	public virtual nil SetByPath(IList<str> Path, ICfgValue Value){
		ToolDict.PutValueByPath(CfgDict, Path, Value);
		return NIL;
	}
	public virtual nil RmPath(IList<str> Path){
		ToolDict.PutValueByPath(CfgDict, Path, NIL);
		return NIL;
	}
}

