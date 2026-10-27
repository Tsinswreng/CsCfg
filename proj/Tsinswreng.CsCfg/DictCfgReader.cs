namespace Tsinswreng.CsCfg;
using Tsinswreng.CsCore;
[Doc("不管何格式 Json抑yaml, 皆可用Dict作抽像。")]
public class DictCfgReader : ICfgReader {
	public IDictionary<str, obj> Dict{get;set;} = new Dictionary<str, obj>();
	public Func<CT, Task<IDictionary<str, obj>>>? FnReloadAsy{get;set;}
	public Func<IDictionary<str, obj>>? FnReload{get;set;}
	public void Reload() {
		Dict = FnReload();
	}

	public async Task<nil> Reload(CT Ct) {
		Dict = await FnReloadAsy(Ct);
		return NIL;
	}

	public bool TryGet(IReadOnlyList<string> Path, out object? Got) {
		throw new NotImplementedException();
	}
}
