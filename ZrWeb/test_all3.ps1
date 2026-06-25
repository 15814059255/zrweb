Add-Type -AssemblyName "System.Data"
$connStr = "Server=www123ic.sqlserver.rds.aliyuncs.com,3433;Database=hjhdb;User ID=zr001;Password=zr001@123;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()
$cmd = $conn.CreateCommand()

$cmd.CommandText = "SELECT goodsId, goodsSn, Name, Brand, Capacitance, Resistance, Tolerance, Voltage, Dielectric, Packaging, isSale, dataFlag FROM goods WHERE dataFlag = 1 AND isSale = 1 ORDER BY goodsId"
$reader = $cmd.ExecuteReader()
Write-Host "===== All active products ====="
while ($reader.Read()) {
    Write-Host ("id=" + $reader["goodsId"] + " sn=" + $reader["goodsSn"] + " cap=" + $reader["Capacitance"] + " res=" + $reader["Resistance"] + " tol=" + $reader["Tolerance"] + " vol=" + $reader["Voltage"] + " die=" + $reader["Dielectric"] + " pkg=" + $reader["Packaging"])
}
$reader.Close()
$conn.Close()
