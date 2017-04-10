$testProjectLocations = @('test/UnitTests')
$outputLocation = 'testResults'
$dotnetTestArgs = '-c Release'
iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/StephenCleary/BuildTools/599beba35b53f495d4df6e5c323573aa839137a3/Coverage.ps1'))
