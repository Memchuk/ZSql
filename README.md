# ZSql
### [В РАЗРАБОТКЕ]
ВАЖНО! Если вы не хотите каждый раз передавать конект стринг, создайте конфиг по пути: "{приложение.exe}\config\ZSql.data".    
В него надо вписать данные для подключения в виде:
```json
{
	"Server":"0.0.0.0",
	"Port":"3306",
	"DataBase":"DB name",
	"Login":"Your login",
	"Password":"Your pass"
}
```
Также можно подписаться на метод для ловли ошибок (на примере WinForms):
```cs
using ZSqlLibrary;
	ZSql sql = new ZSql();

	private void Form1_Load(object sender, EventArgs e)
	{
	    sql.Error += onError;
	}
	private void onError( object sender, ZSQLException e)
        {
            MessageBox.Show($"{e.Exception.HResult}: {e.Exception.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
```
