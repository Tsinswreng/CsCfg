//Impl: 聲明見 ILayeredCfgAccessorExtn.cs。
namespace Tsinswreng.CsCfg;

using Tsinswreng.CsCore;

public static partial class ILayeredCfgAccessorExtn{

	public static partial nil SetRoRw(
		this ILayeredCfgAccessor z
		,IReadOnlyList<ICfgReader> RoSrcs
		,ICfgAccessor Rw
	){
		if(RoSrcs is null){
			throw new ArgumentNullException(nameof(RoSrcs));
		}
		if(Rw is null){
			throw new ArgumentNullException(nameof(Rw));
		}
		foreach(var One in RoSrcs){
			if(One is null){
				throw new ArgumentNullException(nameof(RoSrcs), "The read-only source list contains null.");
			}
			if(ReferenceEquals(One, Rw)){
				// 同一個物件當兩個角色: 讀的優先級與寫的落點都說不清, 寧可早拋
				throw new ArgumentException(
					"The writable source must not appear in the read-only source list."
					,nameof(Rw)
				);
			}
		}
		// 讀源依序在前, 可寫源放最後: 寫入只落它, 而讀取時它只在前面都沒命中時才被用到
		z.SetSrcs([.. RoSrcs, Rw]);
		// Writable 必須是 Srcs 的一員, 所以只能放在 SetSrcs 之後; 反過來 setter 會拋
		z.Writable = Rw;
		return NIL;
	}
}
