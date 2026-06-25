Add-Type -AssemblyName "System.Data"
$connStr = "Server=www123ic.sqlserver.rds.aliyuncs.com,3433;Database=hjhdb;User ID=zr001;Password=zr001@123;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()
$cmd = $conn.CreateCommand()

# 检查 Capacitance=100nF 的产品
$cmd.CommandText = "SELECT goodsId, goodsSn, Capacitance, Voltage, isSale, dataFlag FROM goods WHERE Capacitance = '100nF'"
$reader = $cmd.ExecuteReader()
Write-Host "Products with Capacitance='100nF':"
while ($reader.Read()) {
    Write-Host ("  id=" + $reader["goodsId"] + " sn=" + $reader["goodsSn"] + " cap=" + $reader["Capacitance"] + " vol=" + $reader["Voltage"] + " isSale=" + $reader["isSale"] + " dataFlag=" + $reader["dataFlag"])
}
$reader.Close()

# 直接测试 LIKE
$cmd.CommandText = "SELECT COUNT(*) FROM goods WHERE dataFlag = 1 AND isSale = 1 AND Capacitance LIKE '%100nF%'"
Write-Host ("LIKE '%100nF%' count: " + $cmd.ExecuteScalar())

$cmd.CommandText = "SELECT COUNT(*) FROM goods WHERE dataFlag = 1 AND isSale = 1 AND Voltage LIKE '%50V%'"
Write-Host ("LIKE '%50V%' count: " + $cmd.ExecuteScalar())

$cmd.CommandText = "SELECT COUNT(*) FROM goods WHERE dataFlag = 1 AND isSale = 1 AND Capacitance LIKE '%100nF%' AND Voltage LIKE '%50V%'"
Write-Host ("Combined count: " + $cmd.ExecuteScalar())

# 检查 Capacitance 字段是否有空格或其他字符
$cmd.CommandText = "SELECT TOP 5 goodsId, LEN(Capacitance) as len, ASCII(SUBSTRING(Capacitance,1,1)) as c1, Capacitance FROM goods WHERE Capacitance IS NOT NULL AND Capacitance <> ''"
$reader = $cmd.ExecuteReader()
Write-Host "Capacitance field details:"
while ($reader.Read()) {
    Write-Host ("  id=" + $reader["goodsId"] + " len=" + $reader["len"] + " c1=" + $reader["c1"] + " val='" + $reader["Capacitance"] + "'")
}
$reader.Close()

$conn.Close()
