using Tsinswreng.CsCore;

namespace Tsinswreng.CsCfg;


[Doc(@$"Config Reader")]
public partial interface ICfgReader{

	[Doc(@$"
	本讀者的名字。放穩定的技術標識(例如 `env`、`cmdline`、`file`、`gui`), 供來源回報與 i18n 查表;
	不要放顯示文案, 那是界面的事。不當來源的門面可以留空。
	")]
	public str Name{get;set;}

	[Doc(@$"
	按路徑取值。
	路徑為空、中途某段不是字典、或最後一段的鍵不存在, 都回 false;
	回 true 時值可以是 null, 那表示明確設成了 null。
	")]
	public bool TryGet(
		IReadOnlyList<str> Path, out obj? Got
	);

	[Doc(@$"
	這個鍵由哪個讀者提供; 沒命中回 null。
	默認實現把自己當唯一的來源; 多源聚合層要覆寫成按優先級逐個問。
	")]
	public ICfgReader? ReaderOf(IReadOnlyList<str> Path){
		return TryGet(Path, out _) ? this : null;
	}

	[Doc(@$"整體重讀。失敗時拋異常, 舊值保留。")]
	public void Reload();

	[Doc(@$"整體重讀(異步)。失敗時拋異常, 舊值保留。")]
	public Task<nil> Reload(CT Ct);
	
}
