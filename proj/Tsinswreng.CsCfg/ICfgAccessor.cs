using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;


[Doc(@$"Config Accessor")]
public partial interface ICfgAccessor:ICfgReader{

	public bool TrySetNoSave(IReadOnlyList<str> Path, obj? V);
	
	[Doc(@$"
	按路徑刪鍵, 只改內存。
	回 true 表示這棵樹真的被改動了; 回 false 表示什麼都沒刪, 可能是:
	鍵不存在、中途某段缺失、中途某段不是字典、路徑為空。
	刪除是冪等的, 所以「本來就沒有」不算失敗, 也不拋異常。
	")]
	public bool RmPathNoSave(IReadOnlyList<str> Path);

	public void Save();
	
	public Task<nil> Save(CT Ct);
}
