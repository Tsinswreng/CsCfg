//Impl: 聲明見 JsonCfgFileIo.cs。
namespace Tsinswreng.CsCfg.Json;

using Tsinswreng.CsCore;

internal static partial class JsonCfgFileIo{

	internal static partial Stream OpenRead(str FilePath){
		try{
			// 共享打開: 別人正在編輯或讀取這份檔案時不擋
			var S = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			SkipBom(S);
			return S;
		}
		catch(FileNotFoundException){
			// 先 File.Exists 再開檔會有 TOCTOU(兩步之間被刪掉), 所以按例外來判「不存在」
			return new MemoryStream();
		}
		catch(DirectoryNotFoundException){
			// 連目錄都還沒有: 與「檔案不存在」同義, 首次啟動的正常情形
			return new MemoryStream();
		}
	}

	internal static partial Stream OpenTmp(str FilePath){
		var Tmp = MkTmpPath(FilePath);
		// 暫存檔不與別人共享: 它只屬於這次寫入
		return new FileStream(Tmp, FileMode.Create, FileAccess.Write, FileShare.None);
	}

	internal static partial nil FinishTmp(str FilePath, bool Ok){
		var Tmp = FilePath + ".tmp";
		if(Ok){
			// 覆蓋式搬移: 同一卷內是一次 rename, 正式檔不會停留在半份狀態
			File.Move(Tmp, FilePath, overwrite: true);
		}
		else{
			// 寫到一半失敗: 清掉半份暫存檔, 正式檔仍是舊的完整版
			TryDel(Tmp);
		}
		return NIL;
	}

	private static partial str MkTmpPath(str FilePath){
		if(str.IsNullOrWhiteSpace(FilePath)){
			throw new ArgumentException("Config file path is empty.", nameof(FilePath));
		}
		var Dir = System.IO.Path.GetDirectoryName(FilePath);
		if(!str.IsNullOrWhiteSpace(Dir)){
			Directory.CreateDirectory(Dir);
		}
		return FilePath + ".tmp";
	}

	private static partial nil SkipBom(Stream Utf8){
		if(!Utf8.CanSeek){
			return NIL;
		}
		// 先讀三個字節再看: 檔案頭不足三個字節時就是沒有 BOM, 回到開頭即可
		var Head = new byte[3];
		var N = 0;
		while(N < Head.Length){
			var Got = Utf8.Read(Head, N, Head.Length - N);
			if(Got <= 0){
				break;
			}
			N += Got;
		}
		var HasBom = N == Head.Length
			&& Head[0] == 0xEF && Head[1] == 0xBB && Head[2] == 0xBF;
		Utf8.Position = HasBom ? Head.Length : 0;
		return NIL;
	}

	private static partial void TryDel(str FilePath){
		try{
			if(File.Exists(FilePath)){
				File.Delete(FilePath);
			}
		}
		catch(IOException){
			// 留一個孤兒暫存檔不影響正確性, 只是礙眼
		}
		catch(UnauthorizedAccessException){
			// 同上: 清理是盡力而為, 不該讓它變成第二個例外
		}
	}
}
