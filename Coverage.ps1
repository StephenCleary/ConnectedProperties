$testProjectLocations = @('test/UnitTests')
$outputLocation = 'testResults'
$dotnetTestArgs = '-configuration ReleaseCI'
iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/StephenCleary/BuildTools/bbd8d76c4a486fbb7f30e9ceb23e29da0e7e4730/Coverage.ps1'))
