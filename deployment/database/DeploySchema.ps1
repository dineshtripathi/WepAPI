#.\DeploySchema.ps1 -DbServer "flex-opdb.database.windows.net" -DatabaseName "Flex" -Username "xxx" -Password "xxxx" -fileName  "./CreateEntityDB.sql"
Param(
  [string]$DbServer,
  [string]$DatabaseName,
  [string]$Username,
  [string]$Password,
  [string]$fileName

)
[Environment]::CurrentDirectory = get-location
$connectionString = "Data Source=tcp:$DbServer,1433;Initial Catalog=$DatabaseName;User ID=$Username;Password=$Password;Connection Timeout=90;Encrypt=True;TrustServerCertificate=False"
$query = [IO.File]::ReadAllText($fileName)
$queries =  $query -split "GO\s+"
$connection = New-Object -TypeName System.Data.SqlClient.SqlConnection($connectionString)

$connection.Open()

$queries | ForEach-Object {

    if(![string]::IsNullOrWhitespace($_) )
    {
        Write-Host "Executing $_" -foregroundcolor cyan
        try
        {
            $command = New-Object -TypeName System.Data.SqlClient.SqlCommand($_, $connection)
            $rowCount = $command.ExecuteNonQuery()
            Write-Host "Rows affected $rowCount"
        }
        catch
        {
           Write-Host "Error executing query"
            Write-Host $_.Exception.Message
        }
    }
}
$connection.Close()
