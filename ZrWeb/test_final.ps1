Add-Type -AssemblyName "System.Data"
$connStr = "Server=www123ic.sqlserver.rds.aliyuncs.com,3433;Database=hjhdb;User ID=zr001;Password=zr001@123;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()

# 测试1: 搜索 "100nf" (应该匹配 Capacitance=100nF 或 goodsSn 含 104)
$cmd = $conn.CreateCommand()
$sql = "SELECT COUNT(*) FROM goods g WHERE g.dataFlag = 1 AND g.isSale = 1 AND ((g.Capacitance LIKE '%100nF%' OR g.goodsSn LIKE '%100nF%' OR g.goodsDesc LIKE '%100nF%') OR (g.Capacitance LIKE '%104%' OR g.goodsSn LIKE '%104%' OR g.goodsDesc LIKE '%104%'))"
$cmd.CommandText = $sql
Write-Host ("Search '100nf' count: " + $cmd.ExecuteScalar())

# 测试2: 搜索 "50v" (应该匹配 Voltage=50V)
$cmd2 = $conn.CreateCommand()
$sql2 = "SELECT COUNT(*) FROM goods g WHERE g.dataFlag = 1 AND g.isSale = 1 AND ((g.Voltage LIKE '%50v%' OR g.goodsSn LIKE '%50v%' OR g.goodsDesc LIKE '%50v%') OR (g.Voltage LIKE '%50V%' OR g.goodsSn LIKE '%50V%' OR g.goodsDesc LIKE '%50V%'))"
$cmd2.CommandText = $sql2
Write-Host ("Search '50v' count: " + $cmd2.ExecuteScalar())

# 测试3: 搜索 "0.1uf 50v" (AND 组合)
$cmd3 = $conn.CreateCommand()
$sql3 = @"
SELECT COUNT(*) FROM goods g 
WHERE g.dataFlag = 1 AND g.isSale = 1
AND (
  (g.Capacitance LIKE '%0.1uf%' OR g.goodsSn LIKE '%0.1uf%' OR g.goodsDesc LIKE '%0.1uf%')
  OR (g.Capacitance LIKE '%0.1uF%' OR g.goodsSn LIKE '%0.1uF%' OR g.goodsDesc LIKE '%0.1uF%')
  OR (g.Capacitance LIKE '%104%' OR g.goodsSn LIKE '%104%' OR g.goodsDesc LIKE '%104%')
  OR (g.Capacitance LIKE '%100nf%' OR g.goodsSn LIKE '%100nf%' OR g.goodsDesc LIKE '%100nf%')
  OR (g.Capacitance LIKE '%100nF%' OR g.goodsSn LIKE '%100nF%' OR g.goodsDesc LIKE '%100nF%')
)
AND (
  (g.Voltage LIKE '%50v%' OR g.goodsSn LIKE '%50v%' OR g.goodsDesc LIKE '%50v%')
  OR (g.Voltage LIKE '%50V%' OR g.goodsSn LIKE '%50V%' OR g.goodsDesc LIKE '%50V%')
)
"@
$cmd3.CommandText = $sql3
Write-Host ("Search '0.1uf 50v' count: " + $cmd3.ExecuteScalar())

# 列出同时含 104 和 50V 的产品（用于验证）
$cmd4 = $conn.CreateCommand()
$cmd4.CommandText = "SELECT goodsId, goodsSn, Capacitance, Voltage FROM goods WHERE dataFlag = 1 AND isSale = 1 AND (goodsSn LIKE '%104%' OR Capacitance LIKE '%100nF%')"
$reader = $cmd4.ExecuteReader()
Write-Host "Products with 104 in goodsSn or Capacitance=100nF:"
while ($reader.Read()) {
    Write-Host ("  id=" + $reader["goodsId"] + " sn=" + $reader["goodsSn"] + " cap=" + $reader["Capacitance"] + " vol=" + $reader["Voltage"])
}
$reader.Close()

$conn.Close()
