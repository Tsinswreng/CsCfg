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
	按路徑取值, 並同時說出這個值是哪個讀者提供的。

	路徑為空、中途某段不是字典、或最後一段的鍵不存在, 都回 false; 此時 Got 與 Src 皆為 null。
	回 true 時 Got 可以是 null, 那表示明確設成了 null, 而 Src 一定非 null。

	把來源與值放在同一次調用, 是為了避免兩次遍歷之間被 Reload 打斷,
	出現「值來自新源、來源回舊源」這種矛盾。
	")]
	public bool TryGet(
		IReadOnlyList<str> Path, out obj? Got, out ICfgReader? Src
	);

	[Doc(@$"整體重讀。失敗時拋異常, 舊值保留。")]
	public void Reload();

	[Doc(@$"整體重讀(異步)。失敗時拋異常, 舊值保留。")]
	public Task<nil> Reload(CT Ct);
	
}
