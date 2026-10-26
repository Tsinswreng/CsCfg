namespace Tsinswreng.CsCfg;
using Tsinswreng.CsKeyNode;
public static class ExtnKeyNode{
	// public static IList<str> GetFullPathSegs(
	// 	this IKeyNode Node
	// ){
	// 	var Cur = Node;
	// 	var List2D = new List<IList<str>>();
	// 	for(;;){
	// 		if(Cur == null){break;}
	// 		List2D.Add(Cur.RelaPathSegs);
	// 		Cur = Cur.Parent;
	// 	}
	// 	List2D.Reverse();
	// 	var R = new List<str>();
	// 	foreach(var List in List2D){
	// 		foreach(var Path in List){
	// 			R.Add(Path);
	// 		}
	// 	}
	// 	return R;
	// }

	// public static str GetFullPath(
	// 	this IKeyNode z
	// 	,str Sep = KeyNode<nil>.PathSep
	// ){
	// 	if(z._FullPathCache is not null){
	// 		return z._FullPathCache;
	// 	}
	// 	var Segs = z.GetFullPathSegs();
	// 	var R = string.Join(Sep, Segs);
	// 	z._FullPathCache = R;
	// 	return R;
	// }
	
}
