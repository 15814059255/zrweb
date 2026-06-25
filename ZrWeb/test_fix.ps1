Add-Type -AssemblyName "System.Data"
$connStr = "Server=www123ic.sqlserver.rds.aliyuncs.com,3433;Database=hjhdb;User ID=zr001;Password=zr001@123;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()
$cmd = $conn.CreateCommand()

# 模拟修复后的 SQL：搜索 "0.1uf 50v"
# 0.1uf 的变体: 0.1uf(Cap), 0.1uF(Cap), 104(Cap), 100nf(Cap), 100nF(Cap)
# 50v 的变体: 50v(Vol), 50 v(Vol), 50V(Vol)
$sql = @"
SELECT COUNT(*) FROM goods g 
WHERE g.dataFlag = 1 AND g.isSale = 1
AND (
  (g.Capacitance LIKE @kw0 OR g.goodsDesc LIKE @kw0)
  OR (g.Capacitance LIKE @kw1 OR g.goodsDesc LIKE @kw1)
  OR (g.Capacitance LIKE @kw2 OR g.goodsDesc LIKE @kw2)
  OR (g.Capacitance LIKE @kw3 OR g.goodsDesc LIKE @kw3)
  OR (g.Capacitance LIKE @kw4 OR g.goodsDesc LIKE @kw4)
)
AND (
  (g.Voltage LIKE @kw5 OR g.goodsDesc LIKE @kw5)
  OR (g.Voltage LIKE @kw6 OR g.goodsDesc LIKE @kw6)
  OR (g.Voltage LIKE @kw7 OR g.goodsDesc LIKE @kw7)
)
"@
$cmd.CommandText = $sql
$cmd.Parameters.AddWithValue("@kw0", "%0.1uf%")
$cmd.Parameters.AddWithValue("@kw1", "%0.1uF%")
$cmd.Parameters.AddWithValue("@kw2", "%104%")
$cmd.Parameters.AddWithValue("@kw3", "%100nf%")
$cmd.Parameters.AddWithValue("@kw4", "%100nF%")
$cmd.Parameters.AddWithValue("@kw5", "%50v%")
$cmd.Parameters.AddWithValue("@kw6", "%50 v%")
$cmd.Parameters.AddWithValue("@kw7", "%50V%")

$result = $cmd.ExecuteScalar()
Write-Host ("Search '0.1uf 50v' COUNT result: " + $result)

# 列出匹配的产品
$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "SELECT TOP 10 goodsId, goodsSn, Capacitance, Voltage FROM goods WHERE dataFlag = 1 AND isSale = 1 AND Capacitance = '100nF' AND Voltage = '50V'"
$reader = $cmd2.ExecuteReader()
Write-Host "Products with Capacitance=100nF AND Voltage=50V:"
while ($reader.Read()) {
    Write-Host ("  id=" + $reader["goodsId"] + " sn=" + $reader["goodsSn"] + " cap=" + $reader["Capacitance"] + " vol=" + $reader["Voltage"])
}
$reader.Close()

$conn.Close()
