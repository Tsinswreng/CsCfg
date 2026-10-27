namespace Tsinswreng.CsCfg;

public class DictCfgAccessor : DictCfgReader, ICfgAccessor {
	public Func<IDictionary<str, obj?>, Task<nil>>? FnSaveAsy{get;set;}
	public Func<IDictionary<str, obj?>, nil>? FnSave{get;set;}
	
	public void RmPathNoSave(IReadOnlyList<string> Path) {
		
	}

	public void Save() {
		if(FnSave is null){
			throw new ArgumentNullException(nameof(FnSave));
		}
		FnSave(Dict);
	}

	public async Task<nil> Save(CT Ct) {
		if(FnSaveAsy is null){
			throw new ArgumentNullException(nameof(FnSaveAsy));
		}
		return await FnSaveAsy(Dict);
	}

	public bool TrySetNoSave(IReadOnlyList<string> Path, object? V) {
		
	}
}
