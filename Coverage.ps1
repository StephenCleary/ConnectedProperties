$testProjectLocations = @('test/UnitTests')
$outputLocation = 'testResults'
$dotnetTestArgs = '-c DebugCI'
iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/StephenCleary/BuildTools/07f24d767fb7de077a013439abdf0c141ce62b89/Coverage.ps1'))
