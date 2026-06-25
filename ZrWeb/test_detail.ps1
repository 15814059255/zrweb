Add-Type -AssemblyName "System.Data"
$connStr = "Server=www123ic.sqlserver.rds.aliyuncs.com,3433;Database=hjhdb;User ID=zr001;Password=zr001@123;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()
$cmd = $conn.CreateCommand()

# 查看所有 Capacitance=100nF 的产品
$cmd.CommandText = "SELECT goodsId, goodsSn, Capacitance, Resistance, Tolerance, Voltage, Dielectric, Packaging, isSale, dataFlag FROM goods WHERE Capacitance = '100nF'"
$reader = $cmd.ExecuteReader()
Write-Host "Products with Capacitance='100nF':"
while ($reader.Read()) {
    Write-Host ("  id=" + $reader["goodsId"] + " sn=" + $reader["goodsSn"] + " cap=" + $reader["Capacitance"] + " res=" + $reader["Resistance"] + " tol=" + $reader["Tolerance"] + " vol=" + $reader["Voltage"] + " die=" + $reader["Dielectric"] + " pkg=" + $reader["Packaging"])
}
$reader.Close()

# 查看所有 Voltage=50V 的产品
$cmd.CommandText = "SELECT goodsId, goodsSn, Capacitance, Voltage FROM goods WHERE Voltage = '50V' AND dataFlag = 1 AND isSale = 1"
$reader = $cmd.ExecuteReader()
Write-Host "Products with Voltage='50V' (active):"
while ($reader.Read()) {
    Write-Host ("  id=" + $reader["goodsId"] + " sn=" + $reader["goodsSn"] + " cap=" + $reader["Capacitance"] + " vol=" + $reader["Voltage"])
}
$reader.Close()

$conn.Close()
