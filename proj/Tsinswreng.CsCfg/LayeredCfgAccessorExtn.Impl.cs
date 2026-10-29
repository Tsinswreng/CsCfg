//Impl: 聲明見 LayeredCfgAccessorExtn.cs。
namespace Tsinswreng.CsCfg;

using Tsinswreng.CsCore;

public static partial class LayeredCfgAccessorExtn{

	public static partial nil SetRoRw(this LayeredCfgAccessor z, ICfgReader Ro, ICfgAccessor Rw){
		if(Ro is null){
			throw new ArgumentNullException(nameof(Ro));
		}
		if(Rw is null){
			throw new ArgumentNullException(nameof(Rw));
		}
		if(ReferenceEquals(Ro, Rw)){
			// 同一個物件當兩個角色: 讀的優先級與寫的落點都說不清, 寧可早拋
			throw new ArgumentException(
				"The read-only and the writable source must be different objects."
				,nameof(Rw)
			);
		}
		z.SetSrcs([Ro, Rw]);
		// Writable 必須是 Srcs 的一員, 所以只能放在 SetSrcs 之後; 反過來 setter 會拋
		z.Writable = Rw;
		return NIL;
	}
}

