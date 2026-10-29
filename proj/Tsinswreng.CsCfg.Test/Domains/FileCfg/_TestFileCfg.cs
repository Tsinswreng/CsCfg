// 一個 tester 對應一組被測面。這裡的被測面是「檔案源」: JsonFileCfgReader 負責讀,
// JsonFileCfgAccessor 負責讀寫, 兩者共用同一套 IO 策略, 所以按「讀」與「寫」分兩片。
// 主文件以下劃線開頭, 只負責組裝與提供檔案系統輔助, 不在主文件裏寫用例。
using Tsinswreng.CsTreeTest;

namespace Tsinswreng.CsCfg.Test.Domains.FileCfg;

public partial class TestFileCfg : ITester{
	public ITestNode RegisterTestsInto(ITestNode? Node){
		Node ??= new TestNode();
		// 每個用例各建各的目錄與檔案, 彼此獨立, 不需要固定的執行順序
		Node.Ordered = false;
		Node.IsParallelRecursive = false;
		RegisterFileRead(Node);
		RegisterFileWrite(Node);
		return Node;
	}

	/// 造一個本次用例專用的目錄。
	/// 放在測試程式自己的輸出目錄下, 不碰系統暫存與使用者目錄: 測試資料要跟這次執行綁在一起,
	/// 執行完就刪, 不留下會影響下一次執行的殘留檔。
	internal static str MkTempDir(){
		var Dir = System.IO.Path.Combine(
			AppContext.BaseDirectory
			,"_TestFileCfg"
			,Guid.NewGuid().ToString("N")
		);
		Directory.CreateDirectory(Dir);
		return Dir;
	}

	/// 刪掉用例目錄。刪不掉不算失敗: 用例已經跑完, 清不掉只是留下垃圾, 不該讓測試結果變紅。
	internal static void RmTempDir(str Dir){
		try{
			if(Directory.Exists(Dir)){
				Directory.Delete(Dir, recursive: true);
			}
		}
		catch(IOException){
			// 檔案仍被別的進程佔用(例如使用者開著它): 留著即可
		}
	}

	/// 寫一份檔案內容(UTF-8, 不帶 BOM)。
	/// 不指定編碼: BCL 的默認就是「UTF-8 無 BOM」, 這也正是配置庫寫出來的形式。
	internal static void MkFile(str FilePath, str Text){
		Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath)!);
		File.WriteAllText(FilePath, Text);
	}

	/// 寫一份帶 UTF-8 BOM 的檔案內容, 用來驗證讀取端吃得下某些編輯器的存檔習慣。
	internal static void MkFileWithBom(str FilePath, str Text){
		Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath)!);
		File.WriteAllText(FilePath, Text, System.Text.Encoding.UTF8);
	}

	/// 讀回檔案全文(自動辨識 BOM)。
	internal static str ReadFile(str FilePath){
		return File.ReadAllText(FilePath);
	}
}
