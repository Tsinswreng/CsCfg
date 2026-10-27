namespace Tsinswreng.CsCfg;
using System.Globalization;
using Tsinswreng.CsKeyNode;
public static class ICfgReaderExtn{
	
	
	public static bool TryGet(
		this ICfgReader z, IKeyNode KeyNode, out obj? Got
	){
		
		return z.TryGet(KeyNode.FullPathSegs, out Got);
	}
	
	
	public static Func<IKeyNode<T>, T> MkFnGet<T>(this ICfgReader z){
		return (CfgNode)=>{
			return z.Get(CfgNode);
		};
	}

	public static T Get<T>(
		this ICfgReader z
		,IKeyNode<T> CfgNode
	){
		var Node = CfgNode;
		if(!z.TryGet(Node.FullPathSegs, out var Got)){
			return Node.Data;
		}
		
		//顯式設置成null的返null 不用默認值
		if(Got is null){
			return default!;
		}

		if(Got is T Typed){
			return Typed;
		}

		// 值模型只認標量(str/整數族/f32/f64/decimal/bool)與 null。
		// 一律先化成 invariant 字符串再按目標型別 TryParse, 所以「1.5 給 i32」
		// 不論原本寫成數字還是字符串, 都會被判成壞值, 不會一邊拋一邊四捨五入。
		if(!TryCoerce(Got, out T V)){
			throw new ArgumentException(
				$"配置值型別不符: 鍵 {Node.BuildFullPath()} 期望 {typeof(T).Name}, 實際 {Got.GetType().Name}"
			);
		}
		return V;
	}

	/// 把配置裡取到的裸值轉成 T。
	/// 值模型只認標量(str/整數族/f32/f64/decimal/bool)與 null, 不含 enum;
	/// 一律先化成 invariant 字符串再按目標型別 TryParse,
	/// 所以「1.5 給 i32」不論來源是 f64 還是 str 都回 false, 不會四捨五入, 也不會一邊拋一邊過。
	public static bool TryCoerce<T>(obj Got, out T V){
		V = default!;
		if(Got is T Direct){
			V = Direct;
			return true;
		}
		var Target = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

		// enum 不在值模型裡, 先擋掉: 否則 Type.GetTypeCode 會回它的底層整數型別, 這裡就會誤判成整數
		if(Target.IsEnum){
			return false;
		}

		// 目標是字符串: 任何標量都轉成 invariant 表示法
		if(Target == typeof(str)){
			V = (T)(obj)MkInvariantStr(Got);
			return true;
		}

		// 其餘目標型別: 來源先化成 invariant 字符串, 再按目標型別 TryParse。
		// 用 TypeCode 而不是逐個比對型別, 是為了涵蓋整數族(含 u64, 業務側確實在用)。
		var S = Got as str ?? MkInvariantStr(Got);
		switch(Type.GetTypeCode(Target)){
			case TypeCode.Boolean:
				if(bool.TryParse(S, out var B)){
					V = (T)(obj)B;
					return true;
				}
				return false;
			case TypeCode.Int32:
				if(i32.TryParse(S, NumberStyles.Integer, CultureInfo.InvariantCulture, out var I32)){
					V = (T)(obj)I32;
					return true;
				}
				return false;
			case TypeCode.Int64:
				if(i64.TryParse(S, NumberStyles.Integer, CultureInfo.InvariantCulture, out var I64)){
					V = (T)(obj)I64;
					return true;
				}
				return false;
			case TypeCode.UInt64:
				if(u64.TryParse(S, NumberStyles.Integer, CultureInfo.InvariantCulture, out var U64)){
					V = (T)(obj)U64;
					return true;
				}
				return false;
			case TypeCode.UInt32:
				if(u32.TryParse(S, NumberStyles.Integer, CultureInfo.InvariantCulture, out var U32)){
					V = (T)(obj)U32;
					return true;
				}
				return false;
			case TypeCode.Int16:
				if(i16.TryParse(S, NumberStyles.Integer, CultureInfo.InvariantCulture, out var I16)){
					V = (T)(obj)I16;
					return true;
				}
				return false;
			case TypeCode.UInt16:
				if(u16.TryParse(S, NumberStyles.Integer, CultureInfo.InvariantCulture, out var U16)){
					V = (T)(obj)U16;
					return true;
				}
				return false;
			case TypeCode.Byte:
				if(u8.TryParse(S, NumberStyles.Integer, CultureInfo.InvariantCulture, out var U8)){
					V = (T)(obj)U8;
					return true;
				}
				return false;
			case TypeCode.SByte:
				if(i8.TryParse(S, NumberStyles.Integer, CultureInfo.InvariantCulture, out var I8)){
					V = (T)(obj)I8;
					return true;
				}
				return false;
			case TypeCode.Double:
				if(f64.TryParse(S, NumberStyles.Float, CultureInfo.InvariantCulture, out var F64)){
					V = (T)(obj)F64;
					return true;
				}
				return false;
			case TypeCode.Single:
				if(f32.TryParse(S, NumberStyles.Float, CultureInfo.InvariantCulture, out var F32)){
					V = (T)(obj)F32;
					return true;
				}
				return false;
			case TypeCode.Decimal:
				if(decimal.TryParse(S, NumberStyles.Number, CultureInfo.InvariantCulture, out var M)){
					V = (T)(obj)M;
					return true;
				}
				return false;
			default:
				// 值模型不含 enum 與其它型別: 由業務側以字符串鍵自行轉, 不在此處猜。
				return false;
		}
	}

	/// 把標量寫成 invariant 字符串。
	/// 中間經過字符串是刻意的: 它讓「數字 1.5」與「字符串 1.5」給 i32 鍵時走同一條判斷。
	public static str MkInvariantStr(obj V){
		return V switch{
			str S => S,
			bool B => B ? "true" : "false",
			i32 I => I.ToString(CultureInfo.InvariantCulture),
			i64 L => L.ToString(CultureInfo.InvariantCulture),
			f32 F => F.ToString("R", CultureInfo.InvariantCulture),
			f64 D => D.ToString("R", CultureInfo.InvariantCulture),
			// 其餘標量走 BCL 的 invariant 表示法
			_ => Convert.ToString(V, CultureInfo.InvariantCulture) ?? "",
		};
	}


}
